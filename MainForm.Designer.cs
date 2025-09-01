using System.Drawing;
using System.Runtime.InteropServices;
using ProcessoClickerGUI.Controls;

namespace ProcessoClickerGUI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox comboBox;
        private TextBox intervalBox;
        private CustomGradientButton attachButton;
        private CustomGradientButton clearButton;
        private CustomGradientButton applyIntervalButton;
        private Label labelInterval;
        private Label labelHelp;
        private Label labelStatus;
        private Panel panelTitle;
        private Label titleLabel;
        private Button buttonClose;
        private Button buttonMinimize;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            comboBox = new ComboBox();
            intervalBox = new TextBox();
            attachButton = new CustomGradientButton();
            clearButton = new CustomGradientButton();
            applyIntervalButton = new CustomGradientButton();
            labelInterval = new Label();
            labelHelp = new Label();
            labelStatus = new Label();
            panelTitle = new Panel();
            buttonClose = new Button();
            buttonMinimize = new Button();
            titleLabel = new Label();

            // Panel Title
            panelTitle.Size = new Size(540, 40);
            panelTitle.Dock = DockStyle.Top;
            panelTitle.BackColor = Color.FromArgb(30, 30, 30);
            panelTitle.MouseDown += panelTitle_MouseDown;

            titleLabel.Text = "ProcessoClicker";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(12, 10);

            buttonClose.Text = "✕";
            StyleTitleButton(buttonClose, new Point(500, 5));
            buttonClose.Click += (s, e) => this.Close();

            buttonMinimize.Text = "─";
            StyleTitleButton(buttonMinimize, new Point(465, 5));
            buttonMinimize.Click += (s, e) => WindowState = FormWindowState.Minimized;

            panelTitle.Controls.Add(titleLabel);
            panelTitle.Controls.Add(buttonMinimize);
            panelTitle.Controls.Add(buttonClose);

            // ComboBox
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("Segoe UI", 10F);
            comboBox.Location = new Point(30, 60);
            comboBox.Size = new Size(480, 28);
            comboBox.BackColor = Color.White;

            // Attach Button
            StyleGradientButton(attachButton, "ATTACH", Color.FromArgb(0, 122, 255), Color.FromArgb(0, 100, 200));
            attachButton.Location = new Point(30, 110);

            // Clear Button
            StyleGradientButton(clearButton, "CLEAR", Color.FromArgb(220, 70, 70), Color.FromArgb(240, 100, 100));
            clearButton.Location = new Point(150, 110);

            // Interval Label
            labelInterval.Text = "Interval (ms):";
            labelInterval.Location = new Point(30, 165);
            labelInterval.ForeColor = Color.White;
            labelInterval.Font = new Font("Segoe UI", 9.5F);
            labelInterval.Size = new Size(100, 25);
            labelInterval.TextAlign = ContentAlignment.MiddleRight;

            // Interval Input
            intervalBox.Location = new Point(140, 165);
            intervalBox.Size = new Size(80, 25);
            intervalBox.Font = new Font("Segoe UI", 10F);
            intervalBox.Text = "1000";
            intervalBox.BackColor = Color.FromArgb(35, 35, 35);
            intervalBox.ForeColor = Color.White;
            intervalBox.BorderStyle = BorderStyle.FixedSingle;
            intervalBox.TextAlign = HorizontalAlignment.Center;

            // Set Button
            StyleGradientButton(applyIntervalButton, "SET", Color.FromArgb(80, 200, 120), Color.FromArgb(60, 180, 100));
            applyIntervalButton.Location = new Point(235, 163);

            // Help Label
            labelHelp.Text = "F6 = Start/Stop clicker";
            labelHelp.Location = new Point(30, 205);
            labelHelp.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            labelHelp.ForeColor = Color.Silver;
            labelHelp.Size = new Size(480, 20);

            // Status Label
            labelStatus.Text = "Not attached";
            labelStatus.Location = new Point(30, 235);
            labelStatus.Size = new Size(480, 40);
            labelStatus.TextAlign = ContentAlignment.MiddleLeft;
            labelStatus.BackColor = Color.FromArgb(0, 160, 255);
            labelStatus.ForeColor = Color.White;
            labelStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            labelStatus.BorderStyle = BorderStyle.FixedSingle;

            // Form settings
            this.ClientSize = new Size(540, 320);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(24, 24, 24);
            this.Controls.AddRange(new Control[]
            {
                panelTitle, comboBox, attachButton, clearButton,
                labelInterval, intervalBox, applyIntervalButton,
                labelHelp, labelStatus
            });
            this.Text = "ProcessoClicker UI";
            this.Name = "MainForm";
            this.Font = new Font("Segoe UI", 10F);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 14, 14));

            ResumeLayout(false);
            PerformLayout();
        }

        private void StyleTitleButton(Button btn, Point location)
        {
            btn.Size = new Size(30, 30);
            btn.Location = location;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.ForeColor = Color.White;
            btn.BackColor = Color.Transparent;
            btn.TabStop = false;
        }

        private void StyleGradientButton(CustomGradientButton btn, string text, Color color1, Color color2)
        {
            btn.ButtonText = text;
            btn.Size = new Size(100, 34);
            btn.GradientColor1 = color1;
            btn.GradientColor2 = color2;
            btn.TextColor = Color.White;
            btn.CornerRadius = 10;
            btn.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);
    }
}