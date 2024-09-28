using System;
using System.Windows.Forms;

namespace WindowTextExtractor.Forms
{
    partial class MagnifierForm : Form
    {
        public decimal Factor { get; private set; }

        public MagnifierForm(decimal factor)
        {
            InitializeComponent();
            Factor = factor;
            numericFactor.Value = factor;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            Factor = (int)numericFactor.Value;
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
