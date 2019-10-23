using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using WindowTextExtractor.Extensions;

namespace WindowTextExtractor.Forms
{
    public partial class MainForm : Form, IMessageFilter
    {
        private readonly int _processId;
        private bool _isButtonTargetMouseDown;
        private Cursor _targetCursor;
        private Cursor _currentCursor;

        public MainForm()
        {
            InitializeComponent();
            _isButtonTargetMouseDown = false;
            _targetCursor = new Cursor(Properties.Resources.Target.Handle);
            _processId = Process.GetCurrentProcess().Id;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Application.AddMessageFilter(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.RemoveMessageFilter(this);
        }

        private void btnTarget_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_isButtonTargetMouseDown)
            {
                _isButtonTargetMouseDown = true;
                _currentCursor = Cursor.Current;
                Cursor.Current = _targetCursor;
                if (!TopMost)
                {
                    SendToBack();
                }
            }
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void txtContent_MultilineChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuItemFont_Click(object sender, EventArgs e)
        {
            var dialog = new FontDialog();
            dialog.ShowHelp = false;
            dialog.ShowColor = false;
            dialog.Font = txtContent.Font;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                txtContent.Font = dialog.Font;
            }
        }

        private void menuItemAlwaysOnTop_Click(object sender, EventArgs e)
        {
            TopMost = !TopMost;
            menuItemAlwaysOnTop.Checked = TopMost;
        }

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_LBUTTONUP = 0x0202;
            const int WM_MOUSEMOVE = 0x0200;
            if (_isButtonTargetMouseDown)
            {
                switch (m.Msg)
                {
                    case WM_LBUTTONUP :
                        {
                            _isButtonTargetMouseDown = false;
                            Cursor.Current = _currentCursor;
                            if (!TopMost)
                            {
                                BringToFront();
                            }
                        } break;

                    case WM_MOUSEMOVE :
                        {
                            var cursorPosition = System.Windows.Forms.Cursor.Position;
                            var element = AutomationElement.FromPoint(new System.Windows.Point(cursorPosition.X, cursorPosition.Y));
                            if (element != null && element.Current.ProcessId != _processId)
                            {
                                var text = element.GetText();
                                txtContent.Text = text;
                                txtContent.ScrollTextToEnd();
                                UpdateStatusBar();
                            }
                        } break;
                }
            }
            return false;
        }

        private void UpdateStatusBar()
        {
            lblTotalChars.Text = "Total Chars: " + txtContent.Text.Length;
            lblTotalLines.Text = "Total Lines: " + txtContent.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
        }
    }
}
