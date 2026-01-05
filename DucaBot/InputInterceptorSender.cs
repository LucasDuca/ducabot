using System;
using System.Threading;
using System.Windows.Forms;
using InputInterceptorNS;

namespace DucaBot
{
    internal static class InputInterceptorSender
    {
        private static readonly object _lock = new();
        private static KeyboardHook? _hook;
        private static bool _driverInstallPromptShown;

        public static void EnsureDriverAndHook()
        {
            lock (_lock)
            {
                EnsureReady();
            }
        }

        public static bool SendKey(VirtualKey key)
        {
            lock (_lock)
            {
                if (!EnsureReady())
                {
                    Logger.LogInfo($"SendKey {key} abortado: hook inativo");
                    return false;
                }

                KeyCode code = key switch
                {
                    VirtualKey.Left => KeyCode.Left,
                    VirtualKey.Right => KeyCode.Right,
                    VirtualKey.Up => KeyCode.Up,
                    VirtualKey.Down => KeyCode.Down,
                    VirtualKey.Numpad9 => KeyCode.Numpad9,
                    VirtualKey.Home => KeyCode.Home,
                    VirtualKey.Space => KeyCode.Space,
                    _ => 0
                };

                if (code == 0)
                    return false;

                var ok = SimulateExtendedPress(code, 30);
                Logger.LogInfo(ok
                    ? $"InputInterceptor SendKey {key} OK (Active={_hook?.Active} CanSimulate={_hook?.CanSimulateInput})"
                    : $"InputInterceptor SendKey {key} falhou (Active={_hook?.Active} CanSimulate={_hook?.CanSimulateInput})");
                return ok;
            }
        }

        private static bool EnsureReady()
        {
            if (_hook != null && _hook.CanSimulateInput && _hook.Active)
                return true;

            if (!InputInterceptor.Initialized)
            {
                if (!InputInterceptor.Initialize())
                {
                    Logger.LogInfo("InputInterceptor Initialize falhou");
                    return false;
                }
                Logger.LogInfo("InputInterceptor inicializado");
            }

            if (!InputInterceptor.CheckDriverInstalled())
            {
                Logger.LogInfo("InputInterceptor driver não instalado. Rode como administrador e instale.");
                if (InputInterceptor.CheckAdministratorRights())
                {
                    Logger.LogInfo("Tentando instalar driver do InputInterceptor...");
                    if (!InputInterceptor.InstallDriver())
                    {
                        Logger.LogInfo("Falha ao instalar driver do InputInterceptor.");
                        return false;
                    }
                    Logger.LogInfo("Driver do InputInterceptor instalado (pode exigir reboot do Windows).");
                    if (!_driverInstallPromptShown)
                    {
                        _driverInstallPromptShown = true;
                        MessageBox.Show(
                            "Driver de teclado instalado. Reinicie o computador para aplicar.",
                            "Reinício necessário",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    Logger.LogInfo("Sem privilégio de administrador para instalar o driver.");
                    return false;
                }
            }
            else
            {
                Logger.LogInfo("Driver do InputInterceptor já instalado.");
            }

            _hook?.Dispose();
            _hook = new KeyboardHook(KeyboardFilter.All);
            Logger.LogInfo($"KeyboardHook criado. CanSimulateInput={_hook.CanSimulateInput}, Active={_hook.Active}");
            return _hook.CanSimulateInput && _hook.Active;
        }

        private static bool SimulateExtendedPress(KeyCode code, int delayMs)
        {
            if (_hook == null || !_hook.CanSimulateInput)
                return false;

            bool needsE0 = code is KeyCode.Left or KeyCode.Right or KeyCode.Up or KeyCode.Down or KeyCode.Home;
            var down = _hook.SetKeyState(code, needsE0 ? KeyState.Down | KeyState.E0 : KeyState.Down);
            if (!down) return false;
            Thread.Sleep(delayMs);
            return _hook.SetKeyState(code, needsE0 ? KeyState.Up | KeyState.E0 : KeyState.Up);
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                _hook?.Dispose();
                _hook = null;
                if (InputInterceptor.Initialized && !InputInterceptor.Disposed)
                    InputInterceptor.Dispose();
            }
        }
    }
}
