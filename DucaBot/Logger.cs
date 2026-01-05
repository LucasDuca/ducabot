using System;
using System.IO;
using System.Text;

namespace DucaBot
{
    internal static class Logger
    {
        private static readonly object _lock = new();
        private static readonly string _path = Path.Combine(AppContext.BaseDirectory, "duca_error.log");

        public static void LogInfo(string message)
        {
            try
            {
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}{Environment.NewLine}";
                lock (_lock)
                {
                    File.AppendAllText(_path, line);
                }
            }
            catch
            {
                // ignore
            }
        }

        public static void LogError(string context, Exception ex)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {context}");
                sb.AppendLine(ex.ToString());
                sb.AppendLine();
                lock (_lock)
                {
                    File.AppendAllText(_path, sb.ToString());
                }
            }
            catch
            {
                // evitar crash em caso de falha ao gravar log
            }
        }
    }
}
