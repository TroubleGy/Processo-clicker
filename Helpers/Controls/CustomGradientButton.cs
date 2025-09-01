using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms; // WinForms Timer is from here

namespace ProcessoClickerGUI.Controls
{
    public class CustomGradientButton : Button
    {
        // === Public Properties ===

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("Button")]
        public string ButtonText { get; set; } = "Button";

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color GradientColor1 { get; set; } = Color.DodgerBlue;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "MediumSlateBlue")]
        public Color GradientColor2 { get; set; } = Color.MediumSlateBlue;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "White")]
        public Color TextColor { get; set; } = Color.White;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(10)]
        public int CornerRadius { get; set; } = 10;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(1)]
        public int BorderSize { get; set; } = 1;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BorderColor { get; set; } = Color.Transparent;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(45f)]
        public float GradientAngle { get; set; } = 45f;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        public Image? Icon { get; set; }

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(16)]
        public int IconSize { get; set; } = 16;

        [Category("Custom")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(6)]
        public int IconSpacing { get; set; } = 6;

        // === Private Fields ===

        private float hoverAnimation = 0f;
        private System.Windows.Forms.Timer animationTimer;
        private bool isHovered = false;
        private bool isPressed = false;

        // === Constructor ===

        public CustomGradientButton()
        {
            DoubleBuffered = true;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            ForeColor = TextColor;

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 15;
            animationTimer.Tick += (s, e) =>
            {
                float target = isHovered ? (isPressed ? 0.3f : 1f) : 0f;
                float step = 0.1f;

                if (Math.Abs(hoverAnimation - target) < step)
                {
                    hoverAnimation = target;
                    animationTimer.Stop();
                }
                else
                {
                    hoverAnimation += (hoverAnimation < target ? step : -step);
                }

                Invalidate();
            };
        }

        // === Drawing ===

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = ClientRectangle;

            using (GraphicsPath path = GetRoundedRectPath(rect, CornerRadius))
            {
                Color c1 = BlendColor(GradientColor1, Color.White, hoverAnimation * 0.15f);
                Color c2 = BlendColor(GradientColor2, Color.White, hoverAnimation * 0.15f);

                if (isPressed)
                {
                    c1 = ControlPaint.Dark(c1, 0.1f);
                    c2 = ControlPaint.Dark(c2, 0.1f);
                }

                using (var brush = new LinearGradientBrush(rect, c1, c2, GradientAngle))
                    g.FillPath(brush, path);

                if (BorderSize > 0)
                {
                    using (var pen = new Pen(BorderColor, BorderSize))
                        g.DrawPath(pen, path);
                }

                if (!Enabled)
                {
                    using (var overlay = new SolidBrush(Color.FromArgb(100, Color.Gray)))
                        g.FillPath(overlay, path);
                }

                DrawIconAndText(g);
            }
        }

        private void DrawIconAndText(Graphics g)
        {
            var bounds = ClientRectangle;
            var textSize = TextRenderer.MeasureText(ButtonText, Font);
            int spacing = Icon != null ? IconSpacing : 0;
            int totalWidth = (Icon != null ? IconSize : 0) + spacing + textSize.Width;

            int contentX = (bounds.Width - totalWidth) / 2;
            int contentY = (bounds.Height - Math.Max(IconSize, textSize.Height)) / 2;

            if (Icon != null)
            {
                var iconRect = new Rectangle(contentX, contentY + (textSize.Height - IconSize) / 2, IconSize, IconSize);
                g.DrawImage(Icon, iconRect);
                contentX += IconSize + IconSpacing;
            }

            var textRect = new Rectangle(contentX, 0, bounds.Width - contentX, bounds.Height);

            TextRenderer.DrawText(
                g,
                ButtonText,
                Font,
                textRect,
                Enabled ? TextColor : Color.DarkGray,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis
            );
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        private Color BlendColor(Color baseColor, Color blendColor, float amount)
        {
            int r = (int)(baseColor.R + (blendColor.R - baseColor.R) * amount);
            int g = (int)(baseColor.G + (blendColor.G - baseColor.G) * amount);
            int b = (int)(baseColor.B + (blendColor.B - baseColor.B) * amount);
            return Color.FromArgb(r, g, b);
        }

        // === Events ===

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            animationTimer.Start();
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            animationTimer.Start();
            Cursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            isPressed = true;
            animationTimer.Start();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isPressed = false;
            animationTimer.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }
    }

}
