using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowTextExtractor.Forms
{
    public partial class MainForm : Form, IMessageFilter
    {
        private bool _isButtonTargetMouseDown;
        private Cursor _targetCursor;
        private Cursor _currentCursor;

        public MainForm()
        {
            InitializeComponent();
            _isButtonTargetMouseDown = false;
            _targetCursor = new Cursor(Properties.Resources.Target.Handle);
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
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_LBUTTONUP = 0x0202;
            if (m.Msg == WM_LBUTTONUP)
            {
                if (_isButtonTargetMouseDown)
                {
                    _isButtonTargetMouseDown = false;
                    Cursor.Current = _currentCursor;
                }
            }
            return false;
        }
    }
}
