using System;
using System.Windows.Forms;

namespace WindowTextExtractor.Forms
{
    partial class BorderWidthForm : Form
    {
        public int BorderWidth { get; private set; }

        public BorderWidthForm(int width)
        {
            InitializeComponent();
            BorderWidth = width;
            numericBorderWidth.Value = width;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            BorderWidth = (int)numericBorderWidth.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                ButtonOkClick(sender, e);
            }

            if (e.KeyValue == 27)
            {
                Close();
            }
        }
    }
}
