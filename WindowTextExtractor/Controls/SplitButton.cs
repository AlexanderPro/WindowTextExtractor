// Copyright © 2014 Sony Computer Entertainment America LLC. See License.txt.
// Copyright © 2017 Michael D. Corbett.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WindowTextExtractor.Controls
{
    /// <summary>
    /// Represents a combination of a standard button on the left and a drop-down button on the right
    /// that is not limited to ToolStrip</summary>
    /// <remarks>ToolStripSplitButton is managed only by ToolStrip</remarks>
    public class SplitButton : Button
    {
        private const int PushButtonWidth = 14;
        private static readonly int BorderSize = SystemInformation.Border3DSize.Width * 2;
        private ContextMenuStrip _contextMenuStrip;
        private bool _dropDownButton;
        private Rectangle _dropDownRectangle;
        private bool _showSplit = true;
        private bool _skipNextOpen;
        private PushButtonState _state;

        /// <summary>
        /// Constructor</summary>
        public SplitButton()
        {
        }

        /// <summary>
        /// Gets or sets the ContextMenuStrip associated with the control.
        /// </summary>
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return _contextMenuStrip;
            }

            set
            {
                if (value != _contextMenuStrip)
                {
                    _contextMenuStrip = value;
                }
            }
        }

        /// <summary>
        /// Sets whether to show button as a dropdown button</summary>
        [DefaultValue(false)]
        public bool DropDownButton
        {
            get
            {
                return _dropDownButton;
            }

            set
            {
                if (value != _dropDownButton)
                {
                    _dropDownButton = value;

                    Invalidate();
                    if (Parent != null)
                    {
                        Parent.PerformLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Sets whether to show split</summary>
        [DefaultValue(true)]
        public bool ShowSplit
        {
            set
            {
                if (value != _showSplit)
                {
                    _showSplit = value;
                    Invalidate();
                    if (Parent != null)
                    {
                        Parent.PerformLayout();
                    }
                }
            }
        }

        private PushButtonState MState
        {
            get { return _state; }

            set
            {
                if (!_state.Equals(value))
                {
                    _state = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets preferred split button size</summary>
        /// <param name="proposedSize">Suggested size</param>
        /// <returns>Preferred size</returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);
            if (_showSplit && !string.IsNullOrEmpty(Text) &&
                TextRenderer.MeasureText(Text, Font).Width + PushButtonWidth > preferredSize.Width)
            {
                return preferredSize + new Size(PushButtonWidth + BorderSize * 2, 0);
            }
            return preferredSize;
        }

        /// <summary>
        /// Tests if key is a regular input key or a special key that requires preprocessing</summary>
        /// <param name="keyData">Key to test</param>
        /// <returns>True iff key is input key</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData.Equals(Keys.Down) && _showSplit)
            {
                return true;
            }
            else
            {
                return base.IsInputKey(keyData);
            }
        }

        /// <summary>
        /// Raises the GotFocus event</summary>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        protected override void OnGotFocus(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnGotFocus(e);
                return;
            }

            if (!MState.Equals(PushButtonState.Pressed) && !MState.Equals(PushButtonState.Disabled))
            {
                MState = PushButtonState.Default;
            }
        }

        /// <summary>
        /// Raises the KeyDown event</summary>
        /// <param name="kevent">KeyEventArgs that contains the event data</param>
        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (_showSplit)
            {
                if (kevent.KeyCode.Equals(Keys.Down))
                {
                    ShowContextMenuStrip();
                }
                else if (kevent.KeyCode.Equals(Keys.Space) && kevent.Modifiers == Keys.None)
                {
                    MState = PushButtonState.Pressed;
                }
            }

            base.OnKeyDown(kevent);
        }

        /// <summary>
        /// Raises the KeyUp event</summary>
        /// <param name="kevent">KeyEventArgs that contains the event data</param>
        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode.Equals(Keys.Space))
            {
                if (MouseButtons == MouseButtons.None)
                {
                    MState = PushButtonState.Normal;
                }
            }
            base.OnKeyUp(kevent);
        }

        /// <summary>
        /// Raises the LostFocus event</summary>
        /// <param name="e">EventArgs that contains the event data</param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnLostFocus(e);
                return;
            }
            if (!MState.Equals(PushButtonState.Pressed) && !MState.Equals(PushButtonState.Disabled))
            {
                MState = PushButtonState.Normal;
            }
        }

        /// <summary>
        /// Raises the MouseDown event</summary>
        /// <param name="e">MouseEventArgs that contains the event data</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_showSplit)
            {
                base.OnMouseDown(e);
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                _skipNextOpen = false;
            }
            else if ((_dropDownRectangle.Contains(e.Location) || _dropDownButton))
            {
                ShowContextMenuStrip();
            }

            MState = PushButtonState.Pressed;
        }

        /// <summary>
        /// Raises the MouseEnter event</summary>
        /// <param name="e">EventArgs that contains the event data</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnMouseEnter(e);
                return;
            }

            if (!MState.Equals(PushButtonState.Pressed) && !MState.Equals(PushButtonState.Disabled))
            {
                MState = PushButtonState.Hot;
            }
        }

        /// <summary>
        /// Raises the MouseLeave event</summary>
        /// <param name="e">EventArgs that contains the event data</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnMouseLeave(e);
                return;
            }

            if (!MState.Equals(PushButtonState.Pressed) && !MState.Equals(PushButtonState.Disabled))
            {
                if (Focused)
                {
                    MState = PushButtonState.Default;
                }
                else
                {
                    MState = PushButtonState.Normal;
                }
            }
        }

        /// <summary>
        /// Raises the MouseUp event</summary>
        /// <param name="mevent">MouseEventArgs that contains the event data</param>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (!_showSplit)
            {
                base.OnMouseUp(mevent);
                return;
            }

            if (ContextMenuStrip == null || !ContextMenuStrip.Visible)
            {
                SetButtonDrawState();
                if ((Bounds.Contains(Parent.PointToClient(Cursor.Position)) &&
                    !(_dropDownRectangle.Contains(mevent.Location)) && _dropDownButton))
                {
                    OnClick(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Raises the Paint event</summary>
        /// <param name="pevent">PaintEventArgs that contains the event data</param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!_showSplit)
            {
                return;
            }

            Graphics g = pevent.Graphics;
            Rectangle bounds = ClientRectangle;

            // Draw the button background as according to the current state.
            if (MState != PushButtonState.Pressed && IsDefault && !Application.RenderWithVisualStyles)
            {
                Rectangle backgroundBounds = bounds;
                backgroundBounds.Inflate(-1, -1);
                ButtonRenderer.DrawButton(g, backgroundBounds, MState);

                // Button renderer doesn't draw the black frame when themes are off =(.
                g.DrawRectangle(SystemPens.WindowFrame, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                ButtonRenderer.DrawButton(g, bounds, MState);
            }

            // Calculate the current dropdown rectangle.
            _dropDownRectangle = new Rectangle(bounds.Right - PushButtonWidth - 1, BorderSize, PushButtonWidth,
                                              bounds.Height - BorderSize * 2);

            int internalBorder = BorderSize;
            Rectangle focusRect;

            // When DropDownButton is enabled, the focus rectangle spans the entire control.
            if (_dropDownButton)
            {
                focusRect = bounds;
                focusRect.Inflate(-3, -3);
            }

            // While normally, the focus rectangle only covers the non-dropdown button area.
            else
            {
                focusRect = new Rectangle(internalBorder,
                                          internalBorder,
                                          bounds.Width - _dropDownRectangle.Width - internalBorder,
                                          bounds.Height - (internalBorder * 2));
            }

            bool drawSplitLine = ((MState == PushButtonState.Hot || MState == PushButtonState.Pressed ||
                                  !Application.RenderWithVisualStyles) && !_dropDownButton);

            if (RightToLeft == RightToLeft.Yes)
            {
                _dropDownRectangle.X = bounds.Left + 1;
                focusRect.X = _dropDownRectangle.Right;
                if (drawSplitLine)
                {
                    // Draw two lines at the edge of the dropdown button.
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Left + PushButtonWidth, BorderSize,
                               bounds.Left + PushButtonWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Left + PushButtonWidth + 1, BorderSize,
                               bounds.Left + PushButtonWidth + 1, bounds.Bottom - BorderSize);
                }
            }
            else
            {
                if (drawSplitLine)
                {
                    // Draw two lines at the edge of the dropdown button.
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Right - PushButtonWidth, BorderSize,
                               bounds.Right - PushButtonWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Right - PushButtonWidth - 1, BorderSize,
                               bounds.Right - PushButtonWidth - 1, bounds.Bottom - BorderSize);
                }
            }

            // Draw an arrow in the correct location.
            PaintArrow(g, _dropDownRectangle);

            // Figure out how to draw the text
            TextFormatFlags formatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            // If we don't use mnemonic, set formatFlag to NoPrefix as this will show ampersand.
            if (!UseMnemonic)
            {
                formatFlags = formatFlags | TextFormatFlags.NoPrefix;
            }
            else if (!ShowKeyboardCues)
            {
                formatFlags = formatFlags | TextFormatFlags.HidePrefix;
            }

            if (!string.IsNullOrEmpty(Text))
            {
                TextRenderer.DrawText(g, Text, Font, focusRect, SystemColors.ControlText, formatFlags);
            }

            // Draw the focus rectangle.
            if (MState != PushButtonState.Pressed && Focused)
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        private void ContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            var cms = sender as ContextMenuStrip;
            if (cms != null)
            {
                cms.Closing -= ContextMenuStrip_Closing;
            }

            SetButtonDrawState();

            if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            {
                _skipNextOpen = (_dropDownRectangle.Contains(PointToClient(Cursor.Position)) ||
                                (ClientRectangle.Contains(PointToClient(Cursor.Position)) &&
                                _dropDownButton));
            }
        }

        private void PaintArrow(Graphics g, Rectangle dropDownRect)
        {
            var middle = new Point(Convert.ToInt32(dropDownRect.Left + dropDownRect.Width / 2),
                                   Convert.ToInt32(dropDownRect.Top + dropDownRect.Height / 2));

            // If the width is odd - favor pushing it over one pixel right.
            middle.X += (dropDownRect.Width % 2);

            var arrow = new[]
                            {
                                new Point(middle.X - 2, middle.Y - 1), new Point(middle.X + 3, middle.Y - 1),
                                new Point(middle.X, middle.Y + 2)
                            };

            g.FillPolygon(SystemBrushes.ControlText, arrow);
        }

        private void SetButtonDrawState()
        {
            if (Bounds.Contains(Parent.PointToClient(Cursor.Position)))
            {
                MState = PushButtonState.Hot;
            }
            else if (Focused)
            {
                MState = PushButtonState.Default;
            }
            else
            {
                MState = PushButtonState.Normal;
            }
        }

        private void ShowContextMenuStrip()
        {
            if (_skipNextOpen)
            {
                // we were called because we're closing the context menu strip
                // when clicking the dropdown button.
                _skipNextOpen = false;
                return;
            }
            MState = PushButtonState.Pressed;

            if (ContextMenuStrip != null)
            {
                ContextMenuStrip.Closing += ContextMenuStrip_Closing;
                ContextMenuStrip.Show(this, new Point(0, Height), ToolStripDropDownDirection.BelowRight);
            }
        }
    }
}