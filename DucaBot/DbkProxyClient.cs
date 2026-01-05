using System;
using System.IO;
using System.IO.Pipes;

namespace DucaBot
{
    /// <summary>
    /// Cliente persistente para \\.\pipe\dbk_proxy. Mantém o pipe aberto e sincroniza acesso.
    /// </summary>
    public sealed class DbkProxyClient : IDisposable
    {
        private readonly string _pipeName;
        private readonly object _sync = new();
        private NamedPipeClientStream? _stream;
        private bool _disposed;
        private int _failedConnectAttempts;

        public DbkProxyClient(string pipeName = "dbk_proxy")
        {
            _pipeName = pipeName;
        }

        public int FailedConnectAttempts => _failedConnectAttempts;

        public ulong HexToUlong(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Endereço em hex é obrigatório", nameof(hex));

            hex = hex.Trim();
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                hex = hex[2..];

            return Convert.ToUInt64(hex, 16);
        }

        public ProxyResponse ReadMemory(ulong pid, ulong address, int size = 4)
        {
            if (size < 1) size = 1;
            if (size > 64) size = 64;

            lock (_sync)
            {
                EnsureConnected();
                return DoRead(pid, address, size);
            }
        }

        public (int value, ProxyResponse raw) ReadInt32FromHexPid(int pid, string hexAddress)
        {
            var addr = HexToUlong(hexAddress);
            var resp = ReadMemory((ulong)pid, addr, 4);
            if (resp.Status != 0)
                throw new InvalidOperationException($"DeviceIoControl retornou status {resp.Status}");
            int val = BitConverter.ToInt32(resp.Data, 0);
            return (val, resp);
        }

        private ProxyResponse DoRead(ulong pid, ulong address, int size)
        {
            try
            {
                // request (com padding de 4 bytes)
                Span<byte> req = stackalloc byte[24];
                BitConverter.TryWriteBytes(req[..8], pid);
                BitConverter.TryWriteBytes(req.Slice(8, 8), address);
                BitConverter.TryWriteBytes(req.Slice(16, 4), (uint)size);
                _stream!.Write(req);
                _stream.Flush();

                Span<byte> header = stackalloc byte[8];
                FillBuffer(_stream, header);
                uint status = BitConverter.ToUInt32(header[..4]);
                uint bytes = BitConverter.ToUInt32(header.Slice(4, 4));
                byte[] data = ReadExact(_stream, 64);

                return new ProxyResponse((int)status, (int)bytes, data);
            }
            catch (Exception ex)
            {
                Logger.LogError($"DbkProxyClient.ReadMemory pid={pid} addr=0x{address:X} size={size}", ex);
                ResetStream();
                throw;
            }
        }

        private void EnsureConnected()
        {
            if (_stream != null && _stream.IsConnected)
            {
                _failedConnectAttempts = 0;
                return;
            }

            ResetStream();
            _stream = new NamedPipeClientStream(
                ".",
                _pipeName,
                PipeDirection.InOut,
                PipeOptions.None,
                System.Security.Principal.TokenImpersonationLevel.Impersonation);
            try
            {
                _stream.Connect(3000);
                _stream.ReadMode = PipeTransmissionMode.Message;
                _failedConnectAttempts = 0;
            }
            catch
            {
                _failedConnectAttempts++;
                ResetStream();
                throw;
            }
        }

        private static void FillBuffer(Stream s, Span<byte> buffer)
        {
            int read = 0;
            while (read < buffer.Length)
            {
                int r = s.Read(buffer.Slice(read));
                if (r == 0) throw new EndOfStreamException();
                read += r;
            }
        }

        private static byte[] ReadExact(Stream s, int count)
        {
            var buffer = new byte[count];
            FillBuffer(s, buffer);
            return buffer;
        }

        private void ResetStream()
        {
            try { _stream?.Dispose(); } catch { }
            _stream = null;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            ResetStream();
        }
    }

    public sealed record ProxyResponse(int Status, int BytesRead, byte[] Data);
}
