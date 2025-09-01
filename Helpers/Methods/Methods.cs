using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ProcessingClicker.Helpers.Methods
{
    /// <summary>
    /// Collection of native WinAPI methods and window enumerator helpers.
    /// </summary>
    public static class NativeMethods
    {
        // --- WinAPI Constants ---
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;

        // --- Messaging, window interaction ---
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern nint FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

        [DllImport("user32.dll")]
        public static extern nint SendMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

        // --- Cursor position (🆕 добавлено) ---
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// Struct representing a POINT structure (WinAPI) for the cursor position.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        // --- Window enumeration ---
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(nint hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        private delegate bool EnumWindowsProc(nint hWnd, nint lParam);

        /// <summary>
        /// Returns titles of all visible open windows.
        /// </summary>
        public static List<string> GetOpenWindows()
        {
            List<string> windows = new();
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder title = new(256);
                    GetWindowText(hWnd, title, title.Capacity);
                    string name = title.ToString();
                    if (!string.IsNullOrWhiteSpace(name))
                        windows.Add(name);
                }
                return true;
            }, IntPtr.Zero);

            return windows;
        }
    }

    /// <summary>
    /// Manages registration and unregistration of global keyboard hotkeys.
    /// </summary>
    public class HotkeyManager : IDisposable
    {
        private readonly Form form;

        public const int HOTKEY_START_STOP = 1;     // F6

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, Keys vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public HotkeyManager(Form f)
        {
            form = f;

            // F6 => Start/Stop clicker
            RegisterHotKey(form.Handle, HOTKEY_START_STOP, 0, Keys.F6);
        }

        public void Dispose()
        {
            UnregisterHotKey(form.Handle, HOTKEY_START_STOP);
        }
    }
}