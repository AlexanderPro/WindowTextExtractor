using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using WindowTextExtractor.Utils;
using WindowTextExtractor.Native;
using WindowTextExtractor.Native.Enums;

namespace WindowTextExtractor.Controls
{
    public partial class Magnifier : UserControl
    {
        private bool _draw;
        private decimal _factor;
        private Point _cursorPosition;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gets or sets border color.")]
        [DefaultValue("Black")]
        public Color BorderColor { get; set; }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gets or sets border width.")]
        [DefaultValue("1")]
        public Single BorderWidth { get; set; }

        public Magnifier()
        {
            InitializeComponent();
            _draw = false;
            _factor = 0;
            _cursorPosition = Point.Empty;
        }

        public void Draw(Point cursorPosition, decimal factor)
        {
            _cursorPosition = cursorPosition;
            _factor = factor;
            _draw = true;
            Invalidate();
        }

        public void Clear()
        {
            _draw = false;
            Invalidate();
        }

        [DebuggerStepThrough]
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_draw)
            {
                var scaleFactor = DPIUtils.ScaleFactor(this, _cursorPosition) / 100;
                var hdcMagnifier = e.Graphics.GetHdc();
                var hwndDesktop = User32.GetDesktopWindow();
                var hdcDesktop = User32.GetDC(hwndDesktop);
                var srcX = (int)Math.Round(((_cursorPosition.X - (Width * scaleFactor / _factor / 2) + 1)), 2);
                var srcY = (int)Math.Round(((_cursorPosition.Y - (Height * scaleFactor / _factor / 2) + 1)), 2);
                var srcWidth = (int)Math.Round(((Width * scaleFactor / _factor)), 2);
                var srcHeight = (int)Math.Round(((Height * scaleFactor / _factor)), 2);
                Gdi32.StretchBlt(hdcMagnifier, 0, 0, Width, Height, hdcDesktop, srcX, srcY, srcWidth, srcHeight, CopyPixelOperations.CaptureBlt | CopyPixelOperations.MergeCopy | CopyPixelOperations.Blackness);
                e.Graphics.ReleaseHdc(hdcMagnifier);
                User32.ReleaseDC(hwndDesktop, hdcDesktop);

                using var borderBrush = new SolidBrush(BorderColor);
                using var borderPen = new Pen(borderBrush, BorderWidth);
                var groupBox = this;
                var textSize = e.Graphics.MeasureString(groupBox.Text, groupBox.Font);
                var rect = new Rectangle(groupBox.ClientRectangle.X,
                                         groupBox.ClientRectangle.Y + (int)(textSize.Height / 2),
                                         groupBox.ClientRectangle.Width - 1,
                                         groupBox.ClientRectangle.Height - (int)(textSize.Height / 2) - 1);
                //e.Graphics.Clear(BackColor);
                e.Graphics.DrawLine(borderPen, rect.Location, new Point(rect.X, rect.Y + rect.Height));
                e.Graphics.DrawLine(borderPen, new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                e.Graphics.DrawLine(borderPen, new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height));
                e.Graphics.DrawLine(borderPen, new Point(rect.X, rect.Y), new Point(rect.X + groupBox.Padding.Left, rect.Y));
                e.Graphics.DrawLine(borderPen, new Point(rect.X + groupBox.Padding.Left + (int)(textSize.Width), rect.Y), new Point(rect.X + rect.Width, rect.Y));
            }
        }
    }
}
