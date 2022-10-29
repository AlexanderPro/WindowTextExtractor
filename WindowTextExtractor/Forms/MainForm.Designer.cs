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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtContent = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblTotalLines = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparatorOne = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTotalChars = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparatorTwo = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblImageSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparatorThree = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveInformationAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveTextAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveTextListAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveImageAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFont = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTextList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowTextList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowEmptyItems = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNotRepeated = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAlwaysOnTop = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAlwaysRefreshTabs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTarget = new System.Windows.Forms.Button();
            this.toolTipForButton = new System.Windows.Forms.ToolTip(this.components);
            this.pbContent = new System.Windows.Forms.PictureBox();
            this.tabContent = new System.Windows.Forms.TabControl();
            this.tabpInformation = new System.Windows.Forms.TabPage();
            this.gvInformation = new System.Windows.Forms.DataGridView();
            this.clmnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabpText = new System.Windows.Forms.TabPage();
            this.splitTextContainer = new System.Windows.Forms.SplitContainer();
            this.gvTextList = new System.Windows.Forms.DataGridView();
            this.dataGridColumnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabpImage = new System.Windows.Forms.TabPage();
            this.numericFps = new System.Windows.Forms.NumericUpDown();
            this.lblFps = new System.Windows.Forms.Label();
            this.btnShowHide = new System.Windows.Forms.Button();
            this.lblRefresh = new System.Windows.Forms.Label();
            this.cmbRefresh = new System.Windows.Forms.ComboBox();
            this.btnRecord = new System.Windows.Forms.Button();
            this.lblRecord = new System.Windows.Forms.Label();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.lblScale = new System.Windows.Forms.Label();
            this.numericScale = new System.Windows.Forms.NumericUpDown();
            this.cmbCaptureCursor = new System.Windows.Forms.ComboBox();
            this.lblCaptureCursor = new System.Windows.Forms.Label();
            this.tabpEnvironment = new System.Windows.Forms.TabPage();
            this.gvEnvironment = new System.Windows.Forms.DataGridView();
            this.menuItemSaveEnvironmentAs = new System.Windows.Forms.ToolStripMenuItem();
            this.clmnEnvironmentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmnEnvironmentValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbContent)).BeginInit();
            this.tabContent.SuspendLayout();
            this.tabpInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvInformation)).BeginInit();
            this.tabpText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitTextContainer)).BeginInit();
            this.splitTextContainer.Panel1.SuspendLayout();
            this.splitTextContainer.Panel2.SuspendLayout();
            this.splitTextContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvTextList)).BeginInit();
            this.tabpImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericFps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericScale)).BeginInit();
            this.tabpEnvironment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvEnvironment)).BeginInit();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtContent.Location = new System.Drawing.Point(0, 0);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtContent.Size = new System.Drawing.Size(512, 332);
            this.txtContent.TabIndex = 0;
            this.txtContent.MultilineChanged += new System.EventHandler(this.txtContent_MultilineChanged);
            this.txtContent.TextChanged += new System.EventHandler(this.txtContent_TextChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTotalLines,
            this.toolStripSeparatorOne,
            this.lblTotalChars,
            this.toolStripSeparatorTwo,
            this.lblImageSize,
            this.toolStripSeparatorThree});
            this.statusStrip.Location = new System.Drawing.Point(0, 490);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(734, 22);
            this.statusStrip.TabIndex = 15;
            // 
            // lblTotalLines
            // 
            this.lblTotalLines.Name = "lblTotalLines";
            this.lblTotalLines.Size = new System.Drawing.Size(65, 17);
            this.lblTotalLines.Text = "Total Lines:";
            // 
            // toolStripSeparatorOne
            // 
            this.toolStripSeparatorOne.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripSeparatorOne.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparatorOne.Name = "toolStripSeparatorOne";
            this.toolStripSeparatorOne.Size = new System.Drawing.Size(4, 17);
            // 
            // lblTotalChars
            // 
            this.lblTotalChars.Name = "lblTotalChars";
            this.lblTotalChars.Size = new System.Drawing.Size(68, 17);
            this.lblTotalChars.Text = "Total Chars:";
            // 
            // toolStripSeparatorTwo
            // 
            this.toolStripSeparatorTwo.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripSeparatorTwo.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparatorTwo.Name = "toolStripSeparatorTwo";
            this.toolStripSeparatorTwo.Size = new System.Drawing.Size(4, 17);
            // 
            // lblImageSize
            // 
            this.lblImageSize.Name = "lblImageSize";
            this.lblImageSize.Size = new System.Drawing.Size(66, 17);
            this.lblImageSize.Text = "Image Size:";
            // 
            // toolStripSeparatorThree
            // 
            this.toolStripSeparatorThree.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripSeparatorThree.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.toolStripSeparatorThree.Name = "toolStripSeparatorThree";
            this.toolStripSeparatorThree.Size = new System.Drawing.Size(4, 17);
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
            this.menuItemSaveInformationAs,
            this.menuItemSaveTextAs,
            this.menuItemSaveTextListAs,
            this.menuItemSaveImageAs,
            this.menuItemSaveEnvironmentAs,
            this.toolStripSeparator,
            this.menuItemExit});
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(37, 20);
            this.menuItemFile.Text = "File";
            // 
            // menuItemSaveInformationAs
            // 
            this.menuItemSaveInformationAs.Enabled = false;
            this.menuItemSaveInformationAs.Name = "menuItemSaveInformationAs";
            this.menuItemSaveInformationAs.Size = new System.Drawing.Size(194, 22);
            this.menuItemSaveInformationAs.Text = "Save Information As...";
            this.menuItemSaveInformationAs.Click += new System.EventHandler(this.menuItemSaveInformationAs_Click);
            // 
            // menuItemSaveTextAs
            // 
            this.menuItemSaveTextAs.Enabled = false;
            this.menuItemSaveTextAs.Name = "menuItemSaveTextAs";
            this.menuItemSaveTextAs.Size = new System.Drawing.Size(194, 22);
            this.menuItemSaveTextAs.Text = "Save Text As...";
            this.menuItemSaveTextAs.Click += new System.EventHandler(this.menuItemSaveTextAs_Click);
            // 
            // menuItemSaveTextListAs
            // 
            this.menuItemSaveTextListAs.Enabled = false;
            this.menuItemSaveTextListAs.Name = "menuItemSaveTextListAs";
            this.menuItemSaveTextListAs.Size = new System.Drawing.Size(194, 22);
            this.menuItemSaveTextListAs.Text = "Save Text List As...";
            this.menuItemSaveTextListAs.Click += new System.EventHandler(this.menuItemSaveTextListAs_Click);
            // 
            // menuItemSaveImageAs
            // 
            this.menuItemSaveImageAs.Enabled = false;
            this.menuItemSaveImageAs.Name = "menuItemSaveImageAs";
            this.menuItemSaveImageAs.Size = new System.Drawing.Size(194, 22);
            this.menuItemSaveImageAs.Text = "Save Image As...";
            this.menuItemSaveImageAs.Click += new System.EventHandler(this.menuItemSaveImageAs_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(191, 6);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(194, 22);
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemOptions
            // 
            this.menuItemOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFont,
            this.menuItemTextList,
            this.menuItemAlwaysOnTop,
            this.menuItemAlwaysRefreshTabs});
            this.menuItemOptions.Name = "menuItemOptions";
            this.menuItemOptions.Size = new System.Drawing.Size(61, 20);
            this.menuItemOptions.Text = "Options";
            // 
            // menuItemFont
            // 
            this.menuItemFont.Name = "menuItemFont";
            this.menuItemFont.Size = new System.Drawing.Size(300, 22);
            this.menuItemFont.Text = "Font...";
            this.menuItemFont.Click += new System.EventHandler(this.menuItemFont_Click);
            // 
            // menuItemTextList
            // 
            this.menuItemTextList.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemShowTextList,
            this.menuItemShowEmptyItems,
            this.menuItemNotRepeated});
            this.menuItemTextList.Name = "menuItemTextList";
            this.menuItemTextList.Size = new System.Drawing.Size(300, 22);
            this.menuItemTextList.Text = "Text List";
            // 
            // menuItemShowTextList
            // 
            this.menuItemShowTextList.Checked = true;
            this.menuItemShowTextList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemShowTextList.Name = "menuItemShowTextList";
            this.menuItemShowTextList.Size = new System.Drawing.Size(205, 22);
            this.menuItemShowTextList.Text = "Show Text List";
            this.menuItemShowTextList.Click += new System.EventHandler(this.menuItemShowTextList_Click);
            // 
            // menuItemShowEmptyItems
            // 
            this.menuItemShowEmptyItems.Name = "menuItemShowEmptyItems";
            this.menuItemShowEmptyItems.Size = new System.Drawing.Size(205, 22);
            this.menuItemShowEmptyItems.Text = "Show Empty Items";
            this.menuItemShowEmptyItems.Click += new System.EventHandler(this.menuItemChecked_Click);
            // 
            // menuItemNotRepeated
            // 
            this.menuItemNotRepeated.Checked = true;
            this.menuItemNotRepeated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemNotRepeated.Name = "menuItemNotRepeated";
            this.menuItemNotRepeated.Size = new System.Drawing.Size(205, 22);
            this.menuItemNotRepeated.Text = "Not Repeated New Items";
            this.menuItemNotRepeated.Click += new System.EventHandler(this.menuItemChecked_Click);
            // 
            // menuItemAlwaysOnTop
            // 
            this.menuItemAlwaysOnTop.Name = "menuItemAlwaysOnTop";
            this.menuItemAlwaysOnTop.Size = new System.Drawing.Size(300, 22);
            this.menuItemAlwaysOnTop.Text = "Always On Top";
            this.menuItemAlwaysOnTop.Click += new System.EventHandler(this.menuItemAlwaysOnTop_Click);
            // 
            // menuItemAlwaysRefreshTabs
            // 
            this.menuItemAlwaysRefreshTabs.Checked = true;
            this.menuItemAlwaysRefreshTabs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemAlwaysRefreshTabs.Name = "menuItemAlwaysRefreshTabs";
            this.menuItemAlwaysRefreshTabs.Size = new System.Drawing.Size(300, 22);
            this.menuItemAlwaysRefreshTabs.Text = "Always Refresh Tabs On Pointer Movement";
            this.menuItemAlwaysRefreshTabs.Click += new System.EventHandler(this.menuItemChecked_Click);
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
            // btnTarget
            // 
            this.btnTarget.Image = ((System.Drawing.Image)(resources.GetObject("btnTarget.Image")));
            this.btnTarget.Location = new System.Drawing.Point(12, 38);
            this.btnTarget.Name = "btnTarget";
            this.btnTarget.Size = new System.Drawing.Size(60, 60);
            this.btnTarget.TabIndex = 1;
            this.toolTipForButton.SetToolTip(this.btnTarget, "Find Window (drag over window)");
            this.btnTarget.UseVisualStyleBackColor = true;
            this.btnTarget.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTarget_MouseDown);
            // 
            // pbContent
            // 
            this.pbContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbContent.Location = new System.Drawing.Point(3, 3);
            this.pbContent.Name = "pbContent";
            this.pbContent.Size = new System.Drawing.Size(720, 351);
            this.pbContent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbContent.TabIndex = 4;
            this.pbContent.TabStop = false;
            // 
            // tabContent
            // 
            this.tabContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabContent.Controls.Add(this.tabpInformation);
            this.tabContent.Controls.Add(this.tabpText);
            this.tabContent.Controls.Add(this.tabpImage);
            this.tabContent.Controls.Add(this.tabpEnvironment);
            this.tabContent.Location = new System.Drawing.Point(0, 127);
            this.tabContent.Name = "tabContent";
            this.tabContent.SelectedIndex = 0;
            this.tabContent.Size = new System.Drawing.Size(734, 364);
            this.tabContent.TabIndex = 14;
            this.tabContent.SelectedIndexChanged += new System.EventHandler(this.tabContent_SelectedIndexChanged);
            // 
            // tabpInformation
            // 
            this.tabpInformation.Controls.Add(this.gvInformation);
            this.tabpInformation.Location = new System.Drawing.Point(4, 22);
            this.tabpInformation.Name = "tabpInformation";
            this.tabpInformation.Size = new System.Drawing.Size(726, 338);
            this.tabpInformation.TabIndex = 2;
            this.tabpInformation.Text = "Information";
            this.tabpInformation.UseVisualStyleBackColor = true;
            // 
            // gvInformation
            // 
            this.gvInformation.AllowUserToAddRows = false;
            this.gvInformation.AllowUserToDeleteRows = false;
            this.gvInformation.AllowUserToResizeColumns = false;
            this.gvInformation.AllowUserToResizeRows = false;
            this.gvInformation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvInformation.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gvInformation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gvInformation.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gvInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvInformation.ColumnHeadersVisible = false;
            this.gvInformation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnName,
            this.clmnValue});
            this.gvInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvInformation.Location = new System.Drawing.Point(0, 0);
            this.gvInformation.MultiSelect = false;
            this.gvInformation.Name = "gvInformation";
            this.gvInformation.ReadOnly = true;
            this.gvInformation.RowHeadersVisible = false;
            this.gvInformation.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvInformation.ShowCellErrors = false;
            this.gvInformation.ShowCellToolTips = false;
            this.gvInformation.ShowEditingIcon = false;
            this.gvInformation.ShowRowErrors = false;
            this.gvInformation.Size = new System.Drawing.Size(726, 338);
            this.gvInformation.TabIndex = 0;
            this.gvInformation.TabStop = false;
            // 
            // clmnName
            // 
            this.clmnName.FillWeight = 50F;
            this.clmnName.HeaderText = "Name";
            this.clmnName.Name = "clmnName";
            this.clmnName.ReadOnly = true;
            // 
            // clmnValue
            // 
            this.clmnValue.HeaderText = "Value";
            this.clmnValue.Name = "clmnValue";
            this.clmnValue.ReadOnly = true;
            // 
            // tabpText
            // 
            this.tabpText.Controls.Add(this.splitTextContainer);
            this.tabpText.Location = new System.Drawing.Point(4, 22);
            this.tabpText.Name = "tabpText";
            this.tabpText.Padding = new System.Windows.Forms.Padding(3);
            this.tabpText.Size = new System.Drawing.Size(726, 338);
            this.tabpText.TabIndex = 0;
            this.tabpText.Text = "Text";
            this.tabpText.UseVisualStyleBackColor = true;
            // 
            // splitTextContainer
            // 
            this.splitTextContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitTextContainer.Location = new System.Drawing.Point(3, 3);
            this.splitTextContainer.Name = "splitTextContainer";
            // 
            // splitTextContainer.Panel1
            // 
            this.splitTextContainer.Panel1.Controls.Add(this.txtContent);
            // 
            // splitTextContainer.Panel2
            // 
            this.splitTextContainer.Panel2.Controls.Add(this.gvTextList);
            this.splitTextContainer.Size = new System.Drawing.Size(720, 332);
            this.splitTextContainer.SplitterDistance = 512;
            this.splitTextContainer.TabIndex = 1;
            // 
            // gvTextList
            // 
            this.gvTextList.AllowUserToAddRows = false;
            this.gvTextList.AllowUserToDeleteRows = false;
            this.gvTextList.AllowUserToResizeColumns = false;
            this.gvTextList.AllowUserToResizeRows = false;
            this.gvTextList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvTextList.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gvTextList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gvTextList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gvTextList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvTextList.ColumnHeadersVisible = false;
            this.gvTextList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridColumnText});
            this.gvTextList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvTextList.Location = new System.Drawing.Point(0, 0);
            this.gvTextList.MultiSelect = false;
            this.gvTextList.Name = "gvTextList";
            this.gvTextList.ReadOnly = true;
            this.gvTextList.RowHeadersVisible = false;
            this.gvTextList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvTextList.ShowCellErrors = false;
            this.gvTextList.ShowCellToolTips = false;
            this.gvTextList.ShowEditingIcon = false;
            this.gvTextList.ShowRowErrors = false;
            this.gvTextList.Size = new System.Drawing.Size(204, 332);
            this.gvTextList.TabIndex = 1;
            this.gvTextList.TabStop = false;
            this.gvTextList.SelectionChanged += new System.EventHandler(this.gvTextList_SelectionChanged);
            // 
            // dataGridColumnText
            // 
            this.dataGridColumnText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridColumnText.FillWeight = 50F;
            this.dataGridColumnText.HeaderText = "Text";
            this.dataGridColumnText.Name = "dataGridColumnText";
            this.dataGridColumnText.ReadOnly = true;
            this.dataGridColumnText.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tabpImage
            // 
            this.tabpImage.Controls.Add(this.pbContent);
            this.tabpImage.Location = new System.Drawing.Point(4, 22);
            this.tabpImage.Name = "tabpImage";
            this.tabpImage.Padding = new System.Windows.Forms.Padding(3);
            this.tabpImage.Size = new System.Drawing.Size(726, 338);
            this.tabpImage.TabIndex = 1;
            this.tabpImage.Text = "Image";
            this.tabpImage.UseVisualStyleBackColor = true;
            // 
            // numericFps
            // 
            this.numericFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericFps.Location = new System.Drawing.Point(529, 101);
            this.numericFps.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericFps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericFps.Name = "numericFps";
            this.numericFps.Size = new System.Drawing.Size(88, 20);
            this.numericFps.TabIndex = 11;
            this.numericFps.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericFps.Visible = false;
            this.numericFps.ValueChanged += new System.EventHandler(this.numericFps_ValueChanged);
            // 
            // lblFps
            // 
            this.lblFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFps.AutoSize = true;
            this.lblFps.Location = new System.Drawing.Point(526, 85);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(30, 13);
            this.lblFps.TabIndex = 10;
            this.lblFps.Text = "FPS:";
            this.lblFps.Visible = false;
            // 
            // btnShowHide
            // 
            this.btnShowHide.Location = new System.Drawing.Point(78, 38);
            this.btnShowHide.Name = "btnShowHide";
            this.btnShowHide.Size = new System.Drawing.Size(60, 60);
            this.btnShowHide.TabIndex = 2;
            this.btnShowHide.UseVisualStyleBackColor = true;
            this.btnShowHide.Visible = false;
            this.btnShowHide.Click += new System.EventHandler(this.btnShowHide_Click);
            // 
            // lblRefresh
            // 
            this.lblRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRefresh.AutoSize = true;
            this.lblRefresh.Location = new System.Drawing.Point(423, 38);
            this.lblRefresh.Name = "lblRefresh";
            this.lblRefresh.Size = new System.Drawing.Size(47, 13);
            this.lblRefresh.TabIndex = 4;
            this.lblRefresh.Text = "Refresh:";
            this.lblRefresh.Visible = false;
            // 
            // cmbRefresh
            // 
            this.cmbRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRefresh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRefresh.FormattingEnabled = true;
            this.cmbRefresh.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cmbRefresh.Location = new System.Drawing.Point(426, 54);
            this.cmbRefresh.Name = "cmbRefresh";
            this.cmbRefresh.Size = new System.Drawing.Size(88, 21);
            this.cmbRefresh.TabIndex = 5;
            this.cmbRefresh.Visible = false;
            this.cmbRefresh.SelectedIndexChanged += new System.EventHandler(this.cmbRefresh_SelectedIndexChanged);
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(144, 38);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(60, 60);
            this.btnRecord.TabIndex = 3;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Visible = false;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // lblRecord
            // 
            this.lblRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecord.AutoSize = true;
            this.lblRecord.Location = new System.Drawing.Point(631, 38);
            this.lblRecord.Name = "lblRecord";
            this.lblRecord.Size = new System.Drawing.Size(91, 13);
            this.lblRecord.TabIndex = 8;
            this.lblRecord.Text = "Record stream to:";
            this.lblRecord.Visible = false;
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseFile.Location = new System.Drawing.Point(634, 52);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(88, 23);
            this.btnBrowseFile.TabIndex = 9;
            this.btnBrowseFile.Text = "Browse file ...";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Visible = false;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            // 
            // lblScale
            // 
            this.lblScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(631, 85);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(37, 13);
            this.lblScale.TabIndex = 12;
            this.lblScale.Text = "Scale:";
            this.lblScale.Visible = false;
            // 
            // numericScale
            // 
            this.numericScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericScale.DecimalPlaces = 2;
            this.numericScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericScale.Location = new System.Drawing.Point(634, 101);
            this.numericScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numericScale.Name = "numericScale";
            this.numericScale.Size = new System.Drawing.Size(88, 20);
            this.numericScale.TabIndex = 13;
            this.numericScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericScale.Visible = false;
            this.numericScale.ValueChanged += new System.EventHandler(this.numericScale_ValueChanged);
            // 
            // cmbCaptureCursor
            // 
            this.cmbCaptureCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCaptureCursor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCaptureCursor.FormattingEnabled = true;
            this.cmbCaptureCursor.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cmbCaptureCursor.Location = new System.Drawing.Point(529, 54);
            this.cmbCaptureCursor.Name = "cmbCaptureCursor";
            this.cmbCaptureCursor.Size = new System.Drawing.Size(88, 21);
            this.cmbCaptureCursor.TabIndex = 7;
            this.cmbCaptureCursor.Visible = false;
            this.cmbCaptureCursor.SelectedIndexChanged += new System.EventHandler(this.cmbCaptureCursor_SelectedIndexChanged);
            // 
            // lblCaptureCursor
            // 
            this.lblCaptureCursor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCaptureCursor.AutoSize = true;
            this.lblCaptureCursor.Location = new System.Drawing.Point(526, 38);
            this.lblCaptureCursor.Name = "lblCaptureCursor";
            this.lblCaptureCursor.Size = new System.Drawing.Size(79, 13);
            this.lblCaptureCursor.TabIndex = 6;
            this.lblCaptureCursor.Text = "Capture cursor:";
            this.lblCaptureCursor.Visible = false;
            // 
            // tabpEnvironment
            // 
            this.tabpEnvironment.Controls.Add(this.gvEnvironment);
            this.tabpEnvironment.Location = new System.Drawing.Point(4, 22);
            this.tabpEnvironment.Name = "tabpEnvironment";
            this.tabpEnvironment.Size = new System.Drawing.Size(726, 338);
            this.tabpEnvironment.TabIndex = 3;
            this.tabpEnvironment.Text = "Environment";
            this.tabpEnvironment.UseVisualStyleBackColor = true;
            // 
            // gvEnvironment
            // 
            this.gvEnvironment.AllowUserToAddRows = false;
            this.gvEnvironment.AllowUserToDeleteRows = false;
            this.gvEnvironment.AllowUserToResizeColumns = false;
            this.gvEnvironment.AllowUserToResizeRows = false;
            this.gvEnvironment.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvEnvironment.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gvEnvironment.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gvEnvironment.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gvEnvironment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvEnvironment.ColumnHeadersVisible = false;
            this.gvEnvironment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmnEnvironmentName,
            this.clmnEnvironmentValue});
            this.gvEnvironment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvEnvironment.Location = new System.Drawing.Point(0, 0);
            this.gvEnvironment.MultiSelect = false;
            this.gvEnvironment.Name = "gvEnvironment";
            this.gvEnvironment.ReadOnly = true;
            this.gvEnvironment.RowHeadersVisible = false;
            this.gvEnvironment.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvEnvironment.ShowCellErrors = false;
            this.gvEnvironment.ShowCellToolTips = false;
            this.gvEnvironment.ShowEditingIcon = false;
            this.gvEnvironment.ShowRowErrors = false;
            this.gvEnvironment.Size = new System.Drawing.Size(726, 338);
            this.gvEnvironment.TabIndex = 1;
            this.gvEnvironment.TabStop = false;
            // 
            // menuItemSaveEnvironmentAs
            // 
            this.menuItemSaveEnvironmentAs.Enabled = false;
            this.menuItemSaveEnvironmentAs.Name = "menuItemSaveEnvironmentAs";
            this.menuItemSaveEnvironmentAs.Size = new System.Drawing.Size(194, 22);
            this.menuItemSaveEnvironmentAs.Text = "Save Environment As...";
            this.menuItemSaveEnvironmentAs.Click += new System.EventHandler(this.menuItemSaveEnvironmentAs_Click);
            // 
            // clmnEnvironmentName
            // 
            this.clmnEnvironmentName.FillWeight = 50F;
            this.clmnEnvironmentName.HeaderText = "Name";
            this.clmnEnvironmentName.Name = "clmnEnvironmentName";
            this.clmnEnvironmentName.ReadOnly = true;
            // 
            // clmnEnvironmentValue
            // 
            this.clmnEnvironmentValue.HeaderText = "Value";
            this.clmnEnvironmentValue.Name = "clmnEnvironmentValue";
            this.clmnEnvironmentValue.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 512);
            this.Controls.Add(this.cmbCaptureCursor);
            this.Controls.Add(this.lblCaptureCursor);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.numericScale);
            this.Controls.Add(this.btnBrowseFile);
            this.Controls.Add(this.lblRecord);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.cmbRefresh);
            this.Controls.Add(this.lblRefresh);
            this.Controls.Add(this.lblFps);
            this.Controls.Add(this.numericFps);
            this.Controls.Add(this.btnShowHide);
            this.Controls.Add(this.tabContent);
            this.Controls.Add(this.btnTarget);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "WindowTextExtractor";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbContent)).EndInit();
            this.tabContent.ResumeLayout(false);
            this.tabpInformation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvInformation)).EndInit();
            this.tabpText.ResumeLayout(false);
            this.splitTextContainer.Panel1.ResumeLayout(false);
            this.splitTextContainer.Panel1.PerformLayout();
            this.splitTextContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitTextContainer)).EndInit();
            this.splitTextContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvTextList)).EndInit();
            this.tabpImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericFps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericScale)).EndInit();
            this.tabpEnvironment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvEnvironment)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalLines;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparatorOne;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalChars;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparatorTwo;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private System.Windows.Forms.ToolStripMenuItem menuItemOptions;
        private System.Windows.Forms.ToolStripMenuItem menuItemAlwaysOnTop;
        private System.Windows.Forms.ToolStripMenuItem menuItemFont;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemAbout;
        private System.Windows.Forms.Button btnTarget;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveTextAs;
        private System.Windows.Forms.ToolTip toolTipForButton;
        private System.Windows.Forms.PictureBox pbContent;
        private System.Windows.Forms.TabControl tabContent;
        private System.Windows.Forms.TabPage tabpText;
        private System.Windows.Forms.TabPage tabpImage;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveImageAs;
        private System.Windows.Forms.TabPage tabpInformation;
        private System.Windows.Forms.DataGridView gvInformation;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnValue;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveInformationAs;
        private System.Windows.Forms.ToolStripStatusLabel lblImageSize;
        private System.Windows.Forms.ToolStripStatusLabel toolStripSeparatorThree;
        private System.Windows.Forms.Button btnShowHide;
        private System.Windows.Forms.NumericUpDown numericFps;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.Label lblRefresh;
        private System.Windows.Forms.ComboBox cmbRefresh;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.NumericUpDown numericScale;
        private System.Windows.Forms.ComboBox cmbCaptureCursor;
        private System.Windows.Forms.Label lblCaptureCursor;
        private System.Windows.Forms.ToolStripMenuItem menuItemAlwaysRefreshTabs;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveTextListAs;
        private System.Windows.Forms.SplitContainer splitTextContainer;
        private System.Windows.Forms.DataGridView gvTextList;
        private System.Windows.Forms.ToolStripMenuItem menuItemTextList;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowTextList;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowEmptyItems;
        private System.Windows.Forms.ToolStripMenuItem menuItemNotRepeated;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridColumnText;
        private System.Windows.Forms.TabPage tabpEnvironment;
        private System.Windows.Forms.DataGridView gvEnvironment;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveEnvironmentAs;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnEnvironmentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmnEnvironmentValue;
    }
}

