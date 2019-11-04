namespace WindowTextExtractor.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtContent = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblTotalLines = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparatorLeft = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTotalChars = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparatorRight = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAlwaysOnTop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFont = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTargetText = new System.Windows.Forms.Button();
            this.btnTargetPassword = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.Location = new System.Drawing.Point(0, 112);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(734, 376);
            this.txtContent.TabIndex = 3;
            this.txtContent.MultilineChanged += new System.EventHandler(this.txtContent_MultilineChanged);
            this.txtContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTotalLines,
            this.toolStripSeparatorLeft,
            this.lblTotalChars,
            this.toolStripSeparatorRight});
            this.statusStrip.Location = new System.Drawing.Point(0, 490);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(734, 22);
            this.statusStrip.TabIndex = 4;
            // 
            // lblTotalLines
            // 
            this.lblTotalLines.Name = "lblTotalLines";
            this.lblTotalLines.Size = new System.Drawing.Size(66, 17);
            this.lblTotalLines.Text = "Total Lines:";
            // 
            // toolStripSeparatorLeft
            // 
            this.toolStripSeparatorLeft.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripSeparatorLeft.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparatorLeft.Name = "toolStripSeparatorLeft";
            this.toolStripSeparatorLeft.Size = new System.Drawing.Size(4, 17);
            // 
            // lblTotalChars
            // 
            this.lblTotalChars.Name = "lblTotalChars";
            this.lblTotalChars.Size = new System.Drawing.Size(69, 17);
            this.lblTotalChars.Text = "Total Chars:";
            // 
            // toolStripSeparatorRight
            // 
            this.toolStripSeparatorRight.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripSeparatorRight.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparatorRight.Name = "toolStripSeparatorRight";
            this.toolStripSeparatorRight.Size = new System.Drawing.Size(4, 17);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.menuItemOptions,
            this.menuItemHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(734, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // menuItemFile
            // 
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExit});
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(37, 20);
            this.menuItemFile.Text = "File";
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(92, 22);
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemOptions
            // 
            this.menuItemOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAlwaysOnTop,
            this.menuItemFont});
            this.menuItemOptions.Name = "menuItemOptions";
            this.menuItemOptions.Size = new System.Drawing.Size(61, 20);
            this.menuItemOptions.Text = "Options";
            // 
            // menuItemAlwaysOnTop
            // 
            this.menuItemAlwaysOnTop.Name = "menuItemAlwaysOnTop";
            this.menuItemAlwaysOnTop.Size = new System.Drawing.Size(153, 22);
            this.menuItemAlwaysOnTop.Text = "Always On Top";
            this.menuItemAlwaysOnTop.Click += new System.EventHandler(this.menuItemAlwaysOnTop_Click);
            // 
            // menuItemFont
            // 
            this.menuItemFont.Name = "menuItemFont";
            this.menuItemFont.Size = new System.Drawing.Size(153, 22);
            this.menuItemFont.Text = "Font...";
            this.menuItemFont.Click += new System.EventHandler(this.menuItemFont_Click);
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAbout});
            this.menuItemHelp.Name = "menuItemHelp";
            this.menuItemHelp.Size = new System.Drawing.Size(44, 20);
            this.menuItemHelp.Text = "Help";
            // 
            // menuItemAbout
            // 
            this.menuItemAbout.Name = "menuItemAbout";
            this.menuItemAbout.Size = new System.Drawing.Size(107, 22);
            this.menuItemAbout.Text = "About";
            this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
            // 
            // btnTargetText
            // 
            this.btnTargetText.Image = ((System.Drawing.Image)(resources.GetObject("btnTargetText.Image")));
            this.btnTargetText.Location = new System.Drawing.Point(12, 38);
            this.btnTargetText.Name = "btnTargetText";
            this.btnTargetText.Size = new System.Drawing.Size(60, 60);
            this.btnTargetText.TabIndex = 1;
            this.btnTargetText.UseVisualStyleBackColor = true;
            this.btnTargetText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTargetText_MouseDown);
            // 
            // btnTargetPassword
            // 
            this.btnTargetPassword.Image = ((System.Drawing.Image)(resources.GetObject("btnTargetPassword.Image")));
            this.btnTargetPassword.Location = new System.Drawing.Point(78, 38);
            this.btnTargetPassword.Name = "btnTargetPassword";
            this.btnTargetPassword.Size = new System.Drawing.Size(60, 60);
            this.btnTargetPassword.TabIndex = 2;
            this.btnTargetPassword.UseVisualStyleBackColor = true;
            this.btnTargetPassword.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTargetPassword_MouseDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 512);
            this.Controls.Add(this.btnTargetPassword);
            this.Controls.Add(this.btnTargetText);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.txtContent);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "WindowTextExtractor";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalLines;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparatorLeft;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalChars;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparatorRight;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemOptions;
        private System.Windows.Forms.ToolStripMenuItem menuItemAlwaysOnTop;
        private System.Windows.Forms.ToolStripMenuItem menuItemFont;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemAbout;
        private System.Windows.Forms.Button btnTargetText;
        private System.Windows.Forms.Button btnTargetPassword;
    }
}

