using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessingClicker.Helpers.Blur
{
    /// <summary>
    /// Provides helper methods for enabling window blur (DWM / Acrylic / Accent).
    /// </summary>
    public static class BlurHelper
    {
        #region DWM Blur

        [DllImport("dwmapi.dll")]
        private static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_BLURBEHIND
        {
            public DwmBlurBehindFlags dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [Flags]
        private enum DwmBlurBehindFlags : uint
        {
            DWM_BB_ENABLE = 0x1,
            DWM_BB_BLURREGION = 0x2,
            DWM_BB_TRANSITIONONMAXIMIZED = 0x4
        }

        /// <summary>
        /// Enables simple DWM blur behind the window.
        /// </summary>
        public static void EnableDwmBlur(Form form)
        {
            var blur = new DWM_BLURBEHIND
            {
                fEnable = true,
                dwFlags = DwmBlurBehindFlags.DWM_BB_ENABLE,
                hRgnBlur = IntPtr.Zero,
                fTransitionOnMaximized = false
            };

            DwmEnableBlurBehindWindow(form.Handle, ref blur);
        }

        #endregion

        #region Accent Blur / Acrylic

        public enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_ENABLE_HOSTBACKDROP = 5,
            ACCENT_INVALID_STATE = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ACCENT_POLICY
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        private enum WINDOWCOMPOSITIONATTRIB
        {
            WCA_ACCENT_POLICY = 19
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWCOMPOSITIONATTRIBDATA
        {
            public WINDOWCOMPOSITIONATTRIB Attribute;
            public nint Data;
            public int SizeOfData;
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(nint hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);

        /// <summary>
        /// Enables modern Acrylic blur (Windows 10+) using Accent policy.
        /// </summary>
        public static void EnableAcrylic(Form form, uint gradientColor = 0x99FFFFFF)
        {
            var accent = new ACCENT_POLICY
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                AccentFlags = 2,
                GradientColor = (int)gradientColor,
                AnimationId = 0
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            try
            {
                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    SizeOfData = accentStructSize,
                    Data = accentPtr
                };

                SetWindowCompositionAttribute(form.Handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(accentPtr);
            }
        }

        /// <summary>
        /// Enables classic blur (non-acrylic) using Accent.
        /// </summary>
        public static void EnableClassicBlur(Form form)
        {
            var accent = new ACCENT_POLICY
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND,
                AccentFlags = 0,
                GradientColor = 0,
                AnimationId = 0
            };

            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            try
            {
                var data = new WINDOWCOMPOSITIONATTRIBDATA
                {
                    Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                    SizeOfData = accentStructSize,
                    Data = accentPtr
                };

                SetWindowCompositionAttribute(form.Handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(accentPtr);
            }
        }

        #endregion
    }
}