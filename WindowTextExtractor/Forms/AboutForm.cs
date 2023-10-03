using System;
using System.Windows.Forms;
using System.Diagnostics;
using WindowTextExtractor.Utils;

namespace WindowTextExtractor.Forms
{
    partial class AboutForm : Form
    {
        private const string URL = "https://github.com/AlexanderPro/WindowTextExtractor";

        public AboutForm()
        {
            InitializeComponent();
            Text = $"About {AssemblyUtils.AssemblyProductName}";
            lblProductName.Text = $"{AssemblyUtils.AssemblyProductName} v{AssemblyUtils.AssemblyProductVersion}";
            lblCopyright.Text = $"{AssemblyUtils.AssemblyCopyright}-{DateTime.Now.Year} {AssemblyUtils.AssemblyCompany}";
            linkUrl.Text = URL;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkUrlClick(object sender, EventArgs e)
        {
            Process.Start(URL);
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
