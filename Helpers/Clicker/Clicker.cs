using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ProcessingClicker.Helpers.Methods;
using System.Diagnostics;

namespace ProcessingClicker.Helpers.Clicker
{
    public class Clicker
    {
        private IntPtr targetWindowHandle = IntPtr.Zero;
        private bool isAttached = false;
        private bool isClicking = false;

        private int clickIntervalMs = 1000;
        private Point? clickPosition = null; 
        private string? windowTitle;
        private CancellationTokenSource? cancellationTokenSource;

        public bool IsAttached => isAttached;

        public string StateInfo =>
            !isAttached ? "Not attached."
          : isClicking ? $"Clicking at {clickPosition} on '{windowTitle}'"
          : $"Ready to click at {clickPosition} on '{windowTitle}'";

        public bool AttachToWindow(string title)
        {
            targetWindowHandle = NativeMethods.FindWindow(null, title);
            isAttached = targetWindowHandle != IntPtr.Zero;

            if (isAttached)
            {
                windowTitle = title;

                // Save current cursor position immediately on Attach
                clickPosition = GetCursorPosition();
                Debug.WriteLine($"📌 Click position saved at {clickPosition.Value}");
            }

            return isAttached;
        }

        public void Clear()
        {
            Stop();
            targetWindowHandle = IntPtr.Zero;
            isAttached = false;
            windowTitle = null;
            clickPosition = null;
        }

        public void SetInterval(int milliseconds)
        {
            clickIntervalMs = Math.Max(1, milliseconds);
        }

        public void StopClicking()
        {
            if (isClicking)
                Stop();
        }

        public void ToggleClicking()
        {
            if (!isAttached)
                return;

            if (isClicking)
            {
                Debug.WriteLine("🛑 Clicker: Stop requested");
                StopClicking();
            }
            else
            {
                Debug.WriteLine("▶️ Clicker: Start requested");
                StartClicking();
            }
        }

        public void StartClicking()
        {
            if (!isAttached || isClicking)
                return;

            if (clickPosition == null)
            {
                clickPosition = GetCursorPosition();
                Debug.WriteLine($"📌 Click position set: {clickPosition.Value}");
            }

            Start();
        }

        private async void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            isClicking = true;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (clickPosition.HasValue)
                        PerformClick(clickPosition.Value);

                    await Task.Delay(clickIntervalMs, token).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Clicker crash: {ex.Message}");
            }
            finally
            {
                isClicking = false;
            }
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            isClicking = false;
        }

        private void PerformClick(Point position)
        {
            if (targetWindowHandle == IntPtr.Zero) return;

            IntPtr lParam = (IntPtr)((position.Y << 16) | (position.X & 0xFFFF));

            SendMessage(targetWindowHandle, NativeMethods.WM_LBUTTONDOWN, (IntPtr)1, lParam);
            Thread.Sleep(50);
            SendMessage(targetWindowHandle, NativeMethods.WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        private Point GetCursorPosition()
        {
            NativeMethods.POINT p;
            NativeMethods.GetCursorPos(out p);
            return new Point(p.X, p.Y);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }

}
