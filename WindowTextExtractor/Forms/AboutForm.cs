﻿using System;
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
            Text = "About " + AssemblyUtils.AssemblyProductName;
            lblProductName.Text = string.Format("{0} v{1}", AssemblyUtils.AssemblyProductName, AssemblyUtils.AssemblyProductVersion);
            lblCopyright.Text = string.Format("{0}-{1} {2}", AssemblyUtils.AssemblyCopyright, DateTime.Now.Year, AssemblyUtils.AssemblyCompany);
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
