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

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkUrl_Click(object sender, EventArgs e)
        {
            Process.Start(URL);
        }

        private void AboutForm_KeyDown(object sender, KeyEventArgs e)
        {
            btnOk_Click(sender, e);
        }
    }
}
