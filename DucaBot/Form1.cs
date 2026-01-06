using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InputInterceptorNS;
using Microsoft.VisualBasic;

namespace DucaBot
{
    public partial class Form1 : Form
    {
        private const string DefaultBaseHex = "03CC7D34"; //"03CD01B4";//"03C0A074";//run 03CC7848
        private const int PipeFailureThreshold = 10;
        private const int CreatureStableSeconds = 10;
        private const int AttackPollIntervalMs = 150;
        private const int RecordStraightIntervalTiles = 5;
        private const ulong OffsetPosX = 0x0;
        private const ulong OffsetPosY = 0x4;
        private const ulong OffsetPosZ = 0x8;
        // GoToOpponent fica 0x4EC bytes antes da base (base - 0x4EC = 0x03CC7848 quando base = 0x03CC7D34)
        private const int OffsetGoToOpponentBack = 0x4EC;
        private const string AttackingAddressHex = "03CC77B4"; //;"03CCFC34";
        private readonly DbkProxyClient? _client;
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Dictionary<string, ulong> _addresses = new();
        private readonly List<System.Diagnostics.Process> _processes = new();
        private System.Diagnostics.Process? _selectedProcess;

        private int _pid;
        private ulong _baseAddr;
        private int _posX;
        private int _posY;
        private short _posZ;
        private int _lastX;
        private int _lastY;
        private short _lastZ;
        private byte _monsterFlag;
        private byte _goToOpponentFlag;
        private byte _lastMonsterFlag = 0;
        private byte _lastGoToFlag = 0;
        private readonly List<(int X, int Y)> _offRouteTrail = new();
        private int _offRouteStartX;
        private int _offRouteStartY;
        private bool _offRouteTracking;
        private bool _pendingSpace;
        private bool _autoLootEnabled;
        private DateTime _lastMovementTime = DateTime.Now;
        private DateTime _monsterLastChange = DateTime.Now;
        private bool _recording;
        private AxisType _recordLastAxis = AxisType.None;
        private int _recordLastRecordedX;
        private int _recordLastRecordedY;
        private short _recordLastRecordedZ;
        private bool _autoWalking;
        private CancellationTokenSource? _autoCts;
        private string _lastErrorMsg = string.Empty;
        private DateTime _lastErrorTime = DateTime.MinValue;

        public Form1()
        {
            InitializeComponent();
            try
            {
                _client = new DbkProxyClient();
                statusLabel.Text = "Conectado ao pipe dbk_proxy";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Falha ao conectar: {ex.Message}";
                Logger.LogError("Form1 ctor", ex);
            }

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 100;
            _timer.Tick += Timer_Tick;

            delayText.Text = "100";
            _baseAddr = _client?.HexToUlong(DefaultBaseHex) ?? 0;
            // tenta carregar/instalar driver de teclado (InputInterceptor) logo no início
            try { InputInterceptorSender.EnsureDriverAndHook(); } catch { }
            LoadProcesses();
        }

        private void ApplyDelay()
        {
            if (int.TryParse(delayText.Text.Trim(), out var ms) && ms >= 10 && ms <= 2000)
                _timer.Interval = ms;
        }

        private void UpdatePipeWarning()
        {
            if (_client == null)
            {
                pipeRestartLabel.Visible = false;
                return;
            }

            var showWarning = _client.FailedConnectAttempts >= PipeFailureThreshold;
            pipeRestartLabel.Visible = showWarning;
            if (showWarning)
                statusLabel.Text = "Falha ao conectar ao pipe. Reinicie o BOT.";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_client == null || _addresses.Count == 0)
                return;

            if (!IsProcessAlive())
            {
                statusLabel.Text = "Processo encerrado. Selecione novamente.";
                _timer.Stop();
                _autoCts?.Cancel();
                return;
            }

            try
            {
                _posX = ReadInt32("posx");
                _posY = ReadInt32("posy");
                _posZ = ReadInt16("posz");
                _monsterFlag = ReadByte("monster");
                _goToOpponentFlag = ReadByte("gotoOpponent");

                posXValue.Text = _posX.ToString();
                posYValue.Text = _posY.ToString();
                posZValue.Text = _posZ.ToString();
                monsterValue.Text = _monsterFlag == 0 ? "NO" : "YES";
                creatureRawValue.Text = _monsterFlag.ToString();
                goToValue.Text = _goToOpponentFlag.ToString();
                if (goToCheck.Checked && _goToOpponentFlag == 0 && _lastGoToFlag == 0)
                {
                    Logger.LogInfo("GoToOpponent=0 e check marcado: enviando ScrollLock para ativar perseguiÇõÇœo");
                    PressKey(VirtualKey.ScrollLock, 1, 1);
                }
                _lastGoToFlag = _goToOpponentFlag;
                if (_monsterFlag != _lastMonsterFlag)
                {
                    if (_lastMonsterFlag != 0 && _monsterFlag == 0)
                        _pendingSpace = true;
                    Logger.LogInfo($"Attacking flag mudou: {_lastMonsterFlag} -> {_monsterFlag}");
                    _monsterLastChange = DateTime.Now;
                    _lastMonsterFlag = _monsterFlag;
                }

                if (_posX != _lastX || _posY != _lastY)
                    _lastMovementTime = DateTime.Now;

                if (_recording && (_posX != _lastX || _posY != _lastY || _posZ != _lastZ))
                {
                    var moveDx = _posX - _lastX;
                    var moveDy = _posY - _lastY;
                    TrackRecording(moveDx, moveDy);
                }

                _lastX = _posX;
                _lastY = _posY;
                _lastZ = _posZ;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Erro ao ler";
                var msg = ex.Message ?? ex.GetType().Name;
                var now = DateTime.Now;
                if (!msg.Equals(_lastErrorMsg, StringComparison.OrdinalIgnoreCase) ||
                    (now - _lastErrorTime).TotalSeconds > 2)
                {
                    Logger.LogError("Timer_Tick", ex);
                    _lastErrorMsg = msg;
                    _lastErrorTime = now;
                }
            }
            finally
            {
                UpdatePipeWarning();
                // Dispara Home mesmo fora do auto-walk quando o ataque encerra
                if (_pendingSpace && _monsterFlag == 0)
                    FlushPendingSpace(CancellationToken.None);
            }
        }

        private int ReadInt32(string key)
        {
            var addr = _addresses[key];
            var resp = _client!.ReadMemory((ulong)_pid, addr, 4);
            if (resp.Status != 0)
                throw new InvalidOperationException($"Status {resp.Status} para {key}");
            return BitConverter.ToInt32(resp.Data, 0);
        }

        private short ReadInt16(string key)
        {
            var addr = _addresses[key];
            var resp = _client!.ReadMemory((ulong)_pid, addr, 2);
            if (resp.Status != 0)
                throw new InvalidOperationException($"Status {resp.Status} para {key}");
            return BitConverter.ToInt16(resp.Data, 0);
        }

        private byte ReadByte(string key)
        {
            var addr = _addresses[key];
            var resp = _client!.ReadMemory((ulong)_pid, addr, 1);
            if (resp.Status != 0)
                throw new InvalidOperationException($"Status {resp.Status} para {key}");
            return resp.Data[0];
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
            _recording = !_recording;
            recordButton.Text = _recording ? "Gravando..." : "Record Walk";
            recordButton.BackColor = _recording
                ? System.Drawing.Color.FromArgb(200, 80, 80)
                : System.Drawing.Color.FromArgb(52, 92, 170);
            if (_recording)
            {
                _recordLastAxis = AxisType.None;
                _recordLastRecordedX = _posX;
                _recordLastRecordedY = _posY;
                _recordLastRecordedZ = _posZ;
                AddRecordedWaypoint(_posX, _posY, _posZ);
            }
        }

        private async void autoWalkButton_Click(object sender, EventArgs e)
        {
            if (_autoWalking)
            {
                _autoCts?.Cancel();
                return;
            }

            if (historyGrid.Rows.Count == 0)
            {
                MessageBox.Show("Nenhuma posição na grid para percorrer.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _autoCts = new CancellationTokenSource();
            _autoWalking = true;
            autoWalkButton.Text = "Parar";
            autoWalkButton.BackColor = System.Drawing.Color.FromArgb(200, 80, 80);

            try
            {
                await Task.Run(() => AutoWalkLoop(_autoCts.Token));
            }
            catch (Exception ex)
            {
                Logger.LogError("AutoWalk", ex);
            }
            finally
            {
                _autoWalking = false;
                autoWalkButton.Text = "Auto Walk";
                autoWalkButton.BackColor = System.Drawing.Color.FromArgb(46, 180, 120);
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            LoadProcesses();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var path = GetPathSnapshot();
            if (path.Count == 0)
            {
                MessageBox.Show("Nenhuma posição para salvar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv|Texto (*.txt)|*.txt|Todos (*.*)|*.*",
                FileName = "hunt.csv"
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var lines = new List<string> { "posx,posy,posz,interval" };
                foreach (var step in path)
                    lines.Add($"{step.X},{step.Y},{step.Z},{step.IntervalMs}");
                File.WriteAllLines(dlg.FileName, lines);
                statusLabel.Text = "Hunt salva.";
            }
            catch (Exception ex)
            {
                Logger.LogError("saveButton_Click", ex);
                MessageBox.Show("Erro ao salvar a hunt. Veja o log.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Filter = "CSV/Texto (*.csv;*.txt)|*.csv;*.txt|Todos (*.*)|*.*"
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var lines = File.ReadAllLines(dlg.FileName);
                historyGrid.Rows.Clear();
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("posx", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (TryParseLine(line, out var x, out var y, out var z, out var interval))
                        historyGrid.Rows.Add(x, y, z, interval);
                }
                statusLabel.Text = "Hunt importada.";
            }
            catch (Exception ex)
            {
                Logger.LogError("importButton_Click", ex);
                MessageBox.Show("Erro ao importar a hunt. Veja o log.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addCurrentButton_Click(object sender, EventArgs e)
        {
            if (_client == null || _addresses.Count == 0)
            {
                MessageBox.Show("Selecione um processo antes de adicionar à grid.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            historyGrid.Rows.Add(_posX, _posY, _posZ, _timer.Interval);
            statusLabel.Text = "Posição atual adicionada.";
        }

        
        private void clearGridButton_Click(object sender, EventArgs e)
        {
            historyGrid.Rows.Clear();
            statusLabel.Text = "Grid limpa.";
        }

        private void goToCheck_CheckedChanged(object sender, EventArgs e)
        {
            // evita disparar imediatamente se o valor já estiver diferente
            _lastGoToFlag = _goToOpponentFlag;
        }

private void autoLootCheck_CheckedChanged(object sender, EventArgs e)
        {
            _autoLootEnabled = autoLootCheck.Checked;
            autoLootLabel.Text = _autoLootEnabled ? "Auto Loot: ON" : "Auto Loot: OFF";
        }

        private void setIntervalsButton_Click(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Defina o intervalo (ms) para todas as linhas:", "Intervalo para toda a grid", delayText.Text.Trim());
            if (string.IsNullOrWhiteSpace(input))
                return;
            if (!int.TryParse(input, out var interval) || interval < 10 || interval > 2000)
            {
                MessageBox.Show("Informe um valor entre 10 e 2000 ms.", "Valor inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in historyGrid.Rows)
            {
                if (!row.IsNewRow)
                    row.Cells[3].Value = interval;
            }
            statusLabel.Text = $"Intervalos atualizados para {interval} ms.";
        }

        private void processCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_client == null) return;
            if (processCombo.SelectedIndex < 0 || processCombo.SelectedIndex >= _processes.Count)
                return;

            var proc = _processes[processCombo.SelectedIndex];
            _pid = proc.Id;
            _selectedProcess = proc;
            statusLabel.Text = $"Selecionado PID {_pid}";

            try
            {
                _baseAddr = _client.HexToUlong(DefaultBaseHex);
                _addresses.Clear();
                _addresses["posx"] = _baseAddr + OffsetPosX;
                _addresses["posy"] = _baseAddr + OffsetPosY;
                _addresses["posz"] = _baseAddr + OffsetPosZ;
                _addresses["gotoOpponent"] = _baseAddr - (ulong)OffsetGoToOpponentBack;
                _addresses["monster"] = _client.HexToUlong(AttackingAddressHex);
                ApplyDelay();
                _timer.Start();
            }
            catch (Exception ex)
            {
                Logger.LogError("processCombo_SelectedIndexChanged", ex);
                MessageBox.Show("Erro ao iniciar leitura. Veja o log.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProcesses()
        {
            try
            {
                processCombo.Items.Clear();
                _processes.Clear();

                var all = System.Diagnostics.Process.GetProcesses();
                foreach (var p in all)
                {
                    try
                    {
                        if (p.ProcessName.IndexOf("rubinot", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _processes.Add(p);
                            processCombo.Items.Add($"{p.ProcessName} (PID {p.Id})");
                        }
                    }
                    catch { }
                }

                if (processCombo.Items.Count == 0)
                {
                    processCombo.Items.Add("Nenhum processo 'rubinot' encontrado");
                    processCombo.SelectedIndex = 0;
                }
                else
                {
                    processCombo.SelectedIndexChanged -= processCombo_SelectedIndexChanged;
                    processCombo.SelectedIndex = 0;
                    processCombo.SelectedIndexChanged += processCombo_SelectedIndexChanged;
                    processCombo_SelectedIndexChanged(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("LoadProcesses", ex);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _timer?.Dispose();
            _client?.Dispose();
            _autoCts?.Cancel();
            InputInterceptorSender.Dispose();
        }

        private bool IsProcessAlive()
        {
            try
            {
                if (_selectedProcess == null)
                    return false;
                _selectedProcess.Refresh();
                return !_selectedProcess.HasExited;
            }
            catch
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private record Step(int X, int Y, short Z, int IntervalMs);
        private void WaitWhileAttacking(CancellationToken token, int keyDelay)
        {
            var lastRecordedX = _posX;
            var lastRecordedY = _posY;
            while (!token.IsCancellationRequested)
            {
                if (_monsterFlag == 0)
                {
                    // ataque encerrou: garante envio do Home antes de voltar a andar
                    _pendingSpace = true;
                    FlushPendingSpace(token);
                    if (_offRouteTracking && _offRouteTrail.Count > 0)
                        ReturnAlongTrail(keyDelay, token);
                    _offRouteTracking = false;
                    _offRouteTrail.Clear();
                    return;
                }

                var stableSeconds = (DateTime.Now - _monsterLastChange).TotalSeconds;
                if (stableSeconds >= CreatureStableSeconds)
                    break; // criatura n?o mudou por tempo limite, pode seguir caminhando

                if (_offRouteTracking && (_posX != lastRecordedX || _posY != lastRecordedY))
                {
                    _offRouteTrail.Add((_posX, _posY));
                    lastRecordedX = _posX;
                    lastRecordedY = _posY;
                }

                if (!SleepWithCancel(AttackPollIntervalMs, token))
                    break;
            }
            FlushPendingSpace(token);
        }

        private void HighlightRow(int index)
        {
            void Select()
            {
                if (index < 0 || index >= historyGrid.Rows.Count)
                    return;
                historyGrid.ClearSelection();
                var row = historyGrid.Rows[index];
                row.Selected = true;
                if (row.Cells.Count > 0)
                    historyGrid.CurrentCell = row.Cells[0];
            }

            if (historyGrid.InvokeRequired)
                historyGrid.BeginInvoke((Action)(Select));
            else
                Select();
        }

        private void FlushPendingSpace(CancellationToken token)
        {
            if (_pendingSpace && _monsterFlag == 0 && !token.IsCancellationRequested)
            {
                Logger.LogInfo("Attacking encerrou: enviando Home antes de andar");
                PressKey(VirtualKey.Home, 1, 1);
                _pendingSpace = false;
            }
        }

        private void AutoWalkLoop(CancellationToken token)
        {
            Logger.LogInfo("AutoWalkLoop iniciado");
            while (!token.IsCancellationRequested)
            {
                var path = GetPathSnapshot();
                if (path.Count == 0)
                {
                    Thread.Sleep(200);
                    continue;
                }

                for (int i = 0; i < path.Count && !token.IsCancellationRequested; i++)
                {
                    var step = path[i];
                    try
                    {
                        HighlightRow(i);
                        FlushPendingSpace(token);
                        if (token.IsCancellationRequested)
                            break;

                        Logger.LogInfo($"AutoWalk step idx={i} target=({step.X},{step.Y},{step.Z}) interval={step.IntervalMs} cur=({_posX},{_posY},{_posZ})");
                        NavigateTo(step, token);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("AutoWalkLoop", ex);
                    }
                }
            }
        }

        private List<Step> GetPathSnapshot()
        {
            var list = new List<Step>();
            try
            {
                if (historyGrid.InvokeRequired)
                {
                    historyGrid.Invoke(new Action(() => BuildList(list)));
                }
                else
                {
                    BuildList(list);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("GetPathSnapshot", ex);
            }
            return list;

            void BuildList(List<Step> target)
            {
                foreach (DataGridViewRow row in historyGrid.Rows)
                {
                    if (TryParseRow(row, out var x, out var y, out var z, out var interval))
                        target.Add(new Step(x, y, z, interval));
                }
            }
        }

        private bool TryParseRow(DataGridViewRow row, out int x, out int y, out short z, out int interval)
        {
            x = y = 0;
            z = 0;
            interval = 100;
            try
            {
                int.TryParse(row.Cells[0].Value?.ToString() ?? "0", out x);
                int.TryParse(row.Cells[1].Value?.ToString() ?? "0", out y);
                short.TryParse(row.Cells[2].Value?.ToString() ?? "0", out z);
                int.TryParse(row.Cells[3].Value?.ToString() ?? delayText.Text, out interval);
                interval = Clamp(interval, 10, 2000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryParseLine(string line, out int x, out int y, out short z, out int interval)
        {
            x = y = 0;
            z = 0;
            interval = 100;
            var parts = line.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length < 4) return false;
            return int.TryParse(parts[0], out x)
                   && int.TryParse(parts[1], out y)
                   && short.TryParse(parts[2], out z)
                   && int.TryParse(parts[3], out interval);
        }

        private void NavigateTo(Step step, CancellationToken token)
        {
            int keyDelay = 50;
            int.TryParse(keyDelayText.Text.Trim(), out keyDelay);
            keyDelay = Clamp(keyDelay, 10, 500);
            // usar último snapshot de posições e ajustar localmente
            int cx = _posX;
            int cy = _posY;

            int dx = step.X - cx;
            int dy = step.Y - cy;
            int movesX = Math.Abs(dx);
            int movesY = Math.Abs(dy);

            Logger.LogInfo($"NavigateTo target=({step.X},{step.Y}) start=({cx},{cy}) dx={dx} dy={dy} movesX={movesX} movesY={movesY} keyDelay={keyDelay} interval={step.IntervalMs}");

            _offRouteTracking = true;
            _offRouteStartX = _posX;
            _offRouteStartY = _posY;
            _offRouteTrail.Clear();

            FlushPendingSpace(token);
            for (int i = 0; i < movesX && !token.IsCancellationRequested; i++)
            {
                var dir = dx > 0 ? VirtualKey.Right : VirtualKey.Left;
                WaitWhileAttacking(token, keyDelay);
                if (token.IsCancellationRequested) break;
                PressKey(dir, i + 1, movesX);
                cx += dx > 0 ? 1 : -1;
                if (!SleepWithCancel(keyDelay, token)) break;
            }
            for (int i = 0; i < movesY && !token.IsCancellationRequested; i++)
            {
                var dir = dy > 0 ? VirtualKey.Down : VirtualKey.Up;
                WaitWhileAttacking(token, keyDelay);
                if (token.IsCancellationRequested) break;
                PressKey(dir, i + 1, movesY);
                cy += dy > 0 ? 1 : -1;
                if (!SleepWithCancel(keyDelay, token)) break;
            }

            FlushPendingSpace(token);
            _offRouteTracking = false;
            _offRouteTrail.Clear();
            SleepWithCancel(step.IntervalMs, token);
        }

        private static int Clamp(int val, int min, int max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        private void ReturnAlongTrail(int keyDelay, CancellationToken token)
        {
            // retorna pelo caminho percorrido fora da rota, do ponto atual até o início da rota
            for (int i = _offRouteTrail.Count - 1; i >= 0 && !token.IsCancellationRequested; i--)
            {
                var target = _offRouteTrail[i];
                MoveToPosition(target.X, target.Y, keyDelay, token);
            }
            if (!token.IsCancellationRequested)
                MoveToPosition(_offRouteStartX, _offRouteStartY, keyDelay, token);
        }

        private void MoveToPosition(int targetX, int targetY, int keyDelay, CancellationToken token)
        {
            int cx = _posX;
            int cy = _posY;
            int dx = targetX - cx;
            int dy = targetY - cy;

            for (int i = 0; i < Math.Abs(dx) && !token.IsCancellationRequested; i++)
            {
                var dir = dx > 0 ? VirtualKey.Right : VirtualKey.Left;
                PressKey(dir, i + 1, Math.Abs(dx));
                if (!SleepWithCancel(keyDelay, token)) break;
            }
            for (int i = 0; i < Math.Abs(dy) && !token.IsCancellationRequested; i++)
            {
                var dir = dy > 0 ? VirtualKey.Down : VirtualKey.Up;
                PressKey(dir, i + 1, Math.Abs(dy));
                if (!SleepWithCancel(keyDelay, token)) break;
            }
        }

        private void TrackRecording(int moveDx, int moveDy)
        {
            var axisNow = AxisType.None;
            if (moveDx != 0 && moveDy == 0)
                axisNow = AxisType.X;
            else if (moveDy != 0 && moveDx == 0)
                axisNow = AxisType.Y;
            else if (moveDx != 0 && moveDy != 0)
                axisNow = AxisType.Diagonal;

            if (axisNow == AxisType.None)
                return;

            if (_recordLastAxis != AxisType.None && _recordLastAxis != axisNow && axisNow != AxisType.Diagonal)
            {
                // adiciona antes de virar o eixo
                AddRecordedWaypoint(_lastX, _lastY, _lastZ);
                _recordLastRecordedX = _lastX;
                _recordLastRecordedY = _lastY;
                _recordLastRecordedZ = _lastZ;
            }

            if (axisNow == AxisType.X)
            {
                if (Math.Abs(_posX - _recordLastRecordedX) >= RecordStraightIntervalTiles)
                {
                    AddRecordedWaypoint(_posX, _posY, _posZ);
                    _recordLastRecordedX = _posX;
                    _recordLastRecordedY = _posY;
                    _recordLastRecordedZ = _posZ;
                }
            }
            else if (axisNow == AxisType.Y)
            {
                if (Math.Abs(_posY - _recordLastRecordedY) >= RecordStraightIntervalTiles)
                {
                    AddRecordedWaypoint(_posX, _posY, _posZ);
                    _recordLastRecordedX = _posX;
                    _recordLastRecordedY = _posY;
                    _recordLastRecordedZ = _posZ;
                }
            }
            else if (axisNow == AxisType.Diagonal)
            {
                // em diagonais, registra imediatamente para nÃ£o perder a curva
                AddRecordedWaypoint(_posX, _posY, _posZ);
                _recordLastRecordedX = _posX;
                _recordLastRecordedY = _posY;
                _recordLastRecordedZ = _posZ;
            }

            _recordLastAxis = axisNow;
        }

        private void AddRecordedWaypoint(int x, int y, short z)
        {
            historyGrid.Rows.Add(x, y, z, _timer.Interval);
        }

        private bool SleepWithCancel(int milliseconds, CancellationToken token)
        {
            if (milliseconds <= 0)
                return !token.IsCancellationRequested;
            return !token.WaitHandle.WaitOne(milliseconds);
        }

        private enum AxisType
        {
            None,
            X,
            Y,
            Diagonal
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private void PressKey(VirtualKey key, int index, int total)
        {
            try
            {
                Logger.LogInfo($"PressKey {key} ({index}/{total})");

                // 0) tenta via InputInterceptor (driver) para burlar bloqueio de SendInput
                if (InputInterceptorSender.SendKey(key))
                {
                    Logger.LogInfo($"PressKey {key} via InputInterceptor ok");
                    return;
                }

                bool isExtended = key is VirtualKey.Left or VirtualKey.Right or VirtualKey.Up or VirtualKey.Down or VirtualKey.Home;
                ushort scan = key switch
                {
                    VirtualKey.Left => (ushort)ScanCode.Left,
                    VirtualKey.Right => (ushort)ScanCode.Right,
                    VirtualKey.Up => (ushort)ScanCode.Up,
                    VirtualKey.Down => (ushort)ScanCode.Down,
                    VirtualKey.Space => (ushort)ScanCode.Space,
                    VirtualKey.Numpad9 => (ushort)ScanCode.Numpad9,
                    VirtualKey.Home => (ushort)ScanCode.Home,
                    VirtualKey.ScrollLock => (ushort)ScanCode.ScrollLock,
                    _ => 0
                };

                // 1) Tenta scancode com SendInput (mais universal em janelas sem foco)
                var inputs = new INPUT[2];
                inputs[0].type = INPUT_KEYBOARD;
                inputs[0].U.ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = scan,
                    dwFlags = KEYEVENTF_SCANCODE | (isExtended ? KEYEVENTF_EXTENDEDKEY : 0),
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero
                };
                inputs[1].type = INPUT_KEYBOARD;
                inputs[1].U.ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = scan,
                    dwFlags = KEYEVENTF_SCANCODE | (isExtended ? KEYEVENTF_EXTENDEDKEY : 0) | KEYEVENTF_KEYUP,
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero
                };
                var sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
                if (sent == inputs.Length)
                    return;

                Logger.LogInfo($"PressKey {key} scancode falhou (SendInput retornou {sent}), tentando VK");

                // 2) Tenta VK puro com SendInput
                ushort vk = key switch
                {
                    VirtualKey.Left => 0x25,
                    VirtualKey.Right => 0x27,
                    VirtualKey.Up => 0x26,
                    VirtualKey.Down => 0x28,
                    VirtualKey.Space => 0x20,
                    VirtualKey.Numpad9 => 0x69,
                    VirtualKey.Home => 0x24,
                    VirtualKey.ScrollLock => 0x91,
                    _ => (ushort)0
                };

                if (key == VirtualKey.Numpad9 || key == VirtualKey.Home || key == VirtualKey.ScrollLock)
                {
                    // Numpad9 direto com keybd_event para evitar bloqueio de SendInput
                    keybd_event((byte)vk, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(10);
                    keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    return;
                }

                inputs[0].U.ki = new KEYBDINPUT
                {
                    wVk = vk,
                    wScan = 0,
                    dwFlags = isExtended ? KEYEVENTF_EXTENDEDKEY : 0,
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero
                };
                inputs[1].U.ki = new KEYBDINPUT
                {
                    wVk = vk,
                    wScan = 0,
                    dwFlags = (isExtended ? KEYEVENTF_EXTENDEDKEY : 0) | KEYEVENTF_KEYUP,
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero
                };

                sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
                if (sent == inputs.Length)
                    return;

                Logger.LogInfo($"PressKey {key} VK falhou (SendInput retornou {sent}), fallback keybd_event");

                // 3) Último fallback: keybd_event
                const uint KEYEVENTF_KEYUP_CONST = 0x0002;
                const uint KEYEVENTF_EXTENDED = 0x0001;
                keybd_event((byte)vk, 0, KEYEVENTF_EXTENDED, UIntPtr.Zero);
                keybd_event((byte)vk, 0, KEYEVENTF_EXTENDED | KEYEVENTF_KEYUP_CONST, UIntPtr.Zero);
            }
            catch (Exception ex)
            {
                Logger.LogError($"PressKey {key}", ex);
            }
        }
    }
}

namespace DucaBot
{
    internal enum VirtualKey : ushort
    {
        Left = 0x25,
        Up = 0x26,
        Right = 0x27,
        Down = 0x28,
        Space = 0x20,
        Numpad9 = 0x69,
        Home = 0x24,
        ScrollLock = 0x91
    }

    internal enum ScanCode : ushort
    {
        Left = 0x4B,
        Right = 0x4D,
        Up = 0x48,
        Down = 0x50,
        Space = 0x39,
        Numpad9 = 0x49,
        Home = 0x47,
        ScrollLock = 0x46
    }
}
