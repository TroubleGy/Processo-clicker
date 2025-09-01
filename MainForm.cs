using ProcessingClicker.Helpers.Blur;
using ProcessingClicker.Helpers.Methods;
using ProcessingClicker.Helpers.Clicker;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ProcessoClickerGUI
{
    public partial class MainForm : Form
    {
        private readonly Clicker clicker = new();
        private HotkeyManager? hotkeys;
        private readonly System.Windows.Forms.Timer refreshTimer;

        private List<string> previousWindows = new(); // For comparison

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public MainForm()
        {
            InitializeComponent();

            // Enable acrylic blur
            BlurHelper.EnableAcrylic(this, 0xCC1C1C1C);

            this.BackColor = Color.FromArgb(28, 28, 28);
            DoubleBuffered = true;

            hotkeys = new HotkeyManager(this);

            // Create and start refresh timer
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 500; // 0.5 seconds
            refreshTimer.Tick += (s, e) => AutoRefreshTitles();
            refreshTimer.Start();

            attachButton.Click += attachButton_Click;
            clearButton.Click += clearButton_Click;
            applyIntervalButton.Click += applyIntervalButton_Click;
        }

        private void AutoRefreshTitles()
        {
            string? current = comboBox.SelectedItem?.ToString();
            var windows = NativeMethods.GetOpenWindows();

            // Convert dictionary to sorted list of strings
            List<string> currentWindows = windows.ToList();
            currentWindows.Sort();

            bool changed = !previousWindows.SequenceEqual(currentWindows);

            if (changed)
            {
                Debug.WriteLine("💡 Windows list changed, updating ComboBox...");

                comboBox.BeginUpdate();
                comboBox.Items.Clear();

                foreach (var win in currentWindows)
                    comboBox.Items.Add(win);

                // Try to restore selection
                if (!string.IsNullOrEmpty(current) && comboBox.Items.Contains(current))
                    comboBox.SelectedItem = current;

                comboBox.EndUpdate();

                // Remember current state
                previousWindows = currentWindows;
            }
        }

        private void attachButton_Click(object? sender, EventArgs e)
        {
            var title = comboBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please select a window to attach to.");
                return;
            }

            if (!clicker.IsAttached)
            {
                if (clicker.AttachToWindow(title))
                    SetStatus("Attached", Color.FromArgb(50, 168, 82));
                else
                    SetStatus("Window not found", Color.DarkRed);
            }
            else
            {
                clicker.Clear();
                SetStatus("Detached", Color.Gray);
            }
        }

        private void clearButton_Click(object? sender, EventArgs e)
        {
            clicker.Clear();
            SetStatus("Detached", Color.Gray);
        }

        private void applyIntervalButton_Click(object? sender, EventArgs e)
        {
            if (int.TryParse(intervalBox.Text, out int val))
            {
                clicker.SetInterval(val);
                SetStatus($"Interval set: {val} ms", Color.DodgerBlue);
            }
            else
            {
                SetStatus("Invalid interval", Color.DarkRed);
            }
        }

        private void SetStatus(string text, Color color)
        {
            labelStatus.Text = text;
            labelStatus.BackColor = color;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case HotkeyManager.HOTKEY_START_STOP:
                        if (!clicker.IsAttached)
                        {
                            MessageBox.Show("Clicker is not attached to any window.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }

                        clicker.ToggleClicking(); // ← без передачи позиции
                        SetStatus(clicker.StateInfo, Color.Teal);
                        break;
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            refreshTimer.Stop();
            hotkeys?.Dispose();
            clicker.Stop();
        }

        private void panelTitle_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        }
    }
}