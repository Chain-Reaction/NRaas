namespace NRaasPacker
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
            this.ListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.s3SAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuUpdateVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewS3SA = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuImportSTBL = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuImportEnglish = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuRenameSTBL = new System.Windows.Forms.ToolStripMenuItem();
            this.TestAllSTBL = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.ExportAll = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewXML = new System.Windows.Forms.ToolStripMenuItem();
            this.lAYOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewLAYO = new System.Windows.Forms.ToolStripMenuItem();
            this.iMAGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewIMAG = new System.Windows.Forms.ToolStripMenuItem();
            this.iTUNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNewITUN = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.MasterSplit = new System.Windows.Forms.SplitContainer();
            this.ContentList = new System.Windows.Forms.ListView();
            this.NameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TagColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InstanceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FilenameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EditPreview = new XmlEditor();
            this.ToolBar = new System.Windows.Forms.ToolStrip();
            this.ButtonCompare = new System.Windows.Forms.ToolStripButton();
            this.MenuSearch = new System.Windows.Forms.ToolStripButton();
            this.XMLColoring = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.UpdateAll = new System.Windows.Forms.ToolStripButton();
            this.MenuSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuHelp = new System.Windows.Forms.ToolStripButton();
            this.ListMenu.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.MasterSplit.Panel1.SuspendLayout();
            this.MasterSplit.Panel2.SuspendLayout();
            this.MasterSplit.SuspendLayout();
            this.ToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListMenu
            // 
            this.ListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuDetails,
            this.MenuEdit,
            this.toolStripSeparator1,
            this.MenuExport,
            this.MenuImport,
            this.toolStripSeparator3,
            this.s3SAToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.xMLToolStripMenuItem,
            this.lAYOToolStripMenuItem,
            this.iMAGToolStripMenuItem,
            this.iTUNToolStripMenuItem,
            this.toolStripSeparator5,
            this.MenuDelete});
            this.ListMenu.Name = "ListMenu";
            this.ListMenu.Size = new System.Drawing.Size(155, 308);
            // 
            // MenuDetails
            // 
            this.MenuDetails.Name = "MenuDetails";
            this.MenuDetails.Size = new System.Drawing.Size(154, 24);
            this.MenuDetails.Text = "Details";
            this.MenuDetails.Click += new System.EventHandler(this.MenuDetails_Click);
            // 
            // MenuEdit
            // 
            this.MenuEdit.Name = "MenuEdit";
            this.MenuEdit.Size = new System.Drawing.Size(154, 24);
            this.MenuEdit.Text = "Edit";
            this.MenuEdit.Click += new System.EventHandler(this.MenuEdit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(151, 6);
            // 
            // MenuExport
            // 
            this.MenuExport.Name = "MenuExport";
            this.MenuExport.Size = new System.Drawing.Size(154, 24);
            this.MenuExport.Text = "Export";
            this.MenuExport.Click += new System.EventHandler(this.MenuExport_Click);
            // 
            // MenuImport
            // 
            this.MenuImport.Name = "MenuImport";
            this.MenuImport.Size = new System.Drawing.Size(154, 24);
            this.MenuImport.Text = "Import";
            this.MenuImport.Click += new System.EventHandler(this.MenuImport_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(151, 6);
            // 
            // s3SAToolStripMenuItem
            // 
            this.s3SAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuUpdateVersion,
            this.MenuNewS3SA});
            this.s3SAToolStripMenuItem.Name = "s3SAToolStripMenuItem";
            this.s3SAToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.s3SAToolStripMenuItem.Text = "S3SA";
            // 
            // MenuUpdateVersion
            // 
            this.MenuUpdateVersion.Name = "MenuUpdateVersion";
            this.MenuUpdateVersion.Size = new System.Drawing.Size(180, 24);
            this.MenuUpdateVersion.Text = "Update Version";
            this.MenuUpdateVersion.Click += new System.EventHandler(this.MenuUpdateVersion_Click);
            // 
            // MenuNewS3SA
            // 
            this.MenuNewS3SA.Name = "MenuNewS3SA";
            this.MenuNewS3SA.Size = new System.Drawing.Size(180, 24);
            this.MenuNewS3SA.Text = "Add New File";
            this.MenuNewS3SA.Click += new System.EventHandler(this.MenuNewS3SA_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuImportSTBL,
            this.toolStripSeparator7,
            this.MenuImportEnglish,
            this.MenuRenameSTBL,
            this.TestAllSTBL,
            this.toolStripSeparator8,
            this.ExportAll});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.toolsToolStripMenuItem.Text = "STBL";
            // 
            // MenuImportSTBL
            // 
            this.MenuImportSTBL.Name = "MenuImportSTBL";
            this.MenuImportSTBL.Size = new System.Drawing.Size(175, 24);
            this.MenuImportSTBL.Text = "Import All";
            this.MenuImportSTBL.Click += new System.EventHandler(this.MenuImportSTBL_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(172, 6);
            // 
            // MenuImportEnglish
            // 
            this.MenuImportEnglish.Name = "MenuImportEnglish";
            this.MenuImportEnglish.Size = new System.Drawing.Size(175, 24);
            this.MenuImportEnglish.Text = "Import English";
            this.MenuImportEnglish.Click += new System.EventHandler(this.MenuImportEnglish_Click);
            // 
            // MenuRenameSTBL
            // 
            this.MenuRenameSTBL.Name = "MenuRenameSTBL";
            this.MenuRenameSTBL.Size = new System.Drawing.Size(175, 24);
            this.MenuRenameSTBL.Text = "Rename All";
            this.MenuRenameSTBL.Click += new System.EventHandler(this.MenuRenameSTBL_Click);
            // 
            // TestAllSTBL
            // 
            this.TestAllSTBL.Name = "TestAllSTBL";
            this.TestAllSTBL.Size = new System.Drawing.Size(175, 24);
            this.TestAllSTBL.Text = "Test All";
            this.TestAllSTBL.Click += new System.EventHandler(this.TestAllSTBL_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(172, 6);
            // 
            // ExportAll
            // 
            this.ExportAll.Name = "ExportAll";
            this.ExportAll.Size = new System.Drawing.Size(174, 24);
            this.ExportAll.Text = "Export All";
            this.ExportAll.Click += new System.EventHandler(this.ExportAllSTBL_Click);
            // 
            // xMLToolStripMenuItem
            // 
            this.xMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewXML});
            this.xMLToolStripMenuItem.Name = "xMLToolStripMenuItem";
            this.xMLToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.xMLToolStripMenuItem.Text = "XML";
            // 
            // MenuNewXML
            // 
            this.MenuNewXML.Name = "MenuNewXML";
            this.MenuNewXML.Size = new System.Drawing.Size(167, 24);
            this.MenuNewXML.Text = "Add New File";
            this.MenuNewXML.Click += new System.EventHandler(this.MenuNewXML_Click);
            // 
            // lAYOToolStripMenuItem
            // 
            this.lAYOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewLAYO});
            this.lAYOToolStripMenuItem.Name = "lAYOToolStripMenuItem";
            this.lAYOToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.lAYOToolStripMenuItem.Text = "LAYO";
            // 
            // MenuNewLAYO
            // 
            this.MenuNewLAYO.Name = "MenuNewLAYO";
            this.MenuNewLAYO.Size = new System.Drawing.Size(167, 24);
            this.MenuNewLAYO.Text = "Add New File";
            this.MenuNewLAYO.Click += new System.EventHandler(this.MenuNewLAYO_Click);
            // 
            // iMAGToolStripMenuItem
            // 
            this.iMAGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewIMAG});
            this.iMAGToolStripMenuItem.Name = "iMAGToolStripMenuItem";
            this.iMAGToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.iMAGToolStripMenuItem.Text = "IMAG";
            // 
            // MenuNewIMAG
            // 
            this.MenuNewIMAG.Name = "MenuNewIMAG";
            this.MenuNewIMAG.Size = new System.Drawing.Size(167, 24);
            this.MenuNewIMAG.Text = "Add New File";
            this.MenuNewIMAG.Click += new System.EventHandler(this.MenuNewIMAG_Click);
            // 
            // iTUNToolStripMenuItem
            // 
            this.iTUNToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewITUN});
            this.iTUNToolStripMenuItem.Name = "iTUNToolStripMenuItem";
            this.iTUNToolStripMenuItem.Size = new System.Drawing.Size(154, 24);
            this.iTUNToolStripMenuItem.Text = "ITUN";
            // 
            // MenuNewITUN
            // 
            this.MenuNewITUN.Name = "MenuNewITUN";
            this.MenuNewITUN.Size = new System.Drawing.Size(167, 24);
            this.MenuNewITUN.Text = "Add New File";
            this.MenuNewITUN.Click += new System.EventHandler(this.MenuNewITUN_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(151, 6);
            // 
            // MenuDelete
            // 
            this.MenuDelete.Name = "MenuDelete";
            this.MenuDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.MenuDelete.Size = new System.Drawing.Size(154, 24);
            this.MenuDelete.Text = "Delete";
            this.MenuDelete.Click += new System.EventHandler(this.MenuDelete_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.MasterSplit);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1123, 585);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 39);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1123, 610);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // MasterSplit
            // 
            this.MasterSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MasterSplit.Location = new System.Drawing.Point(0, 0);
            this.MasterSplit.Name = "MasterSplit";
            this.MasterSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // MasterSplit.Panel1
            // 
            this.MasterSplit.Panel1.Controls.Add(this.ContentList);
            // 
            // MasterSplit.Panel2
            // 
            this.MasterSplit.Panel2.Controls.Add(this.EditPreview);
            this.MasterSplit.Size = new System.Drawing.Size(1123, 585);
            this.MasterSplit.SplitterDistance = 331;
            this.MasterSplit.TabIndex = 2;
            // 
            // ContentList
            // 
            this.ContentList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.TagColumn,
            this.InstanceColumn,
            this.FilenameColumn});
            this.ContentList.ContextMenuStrip = this.ListMenu;
            this.ContentList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentList.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContentList.FullRowSelect = true;
            this.ContentList.GridLines = true;
            this.ContentList.HideSelection = false;
            this.ContentList.Location = new System.Drawing.Point(0, 0);
            this.ContentList.Name = "ContentList";
            this.ContentList.Size = new System.Drawing.Size(1123, 331);
            this.ContentList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ContentList.TabIndex = 0;
            this.ContentList.UseCompatibleStateImageBehavior = false;
            this.ContentList.View = System.Windows.Forms.View.Details;
            this.ContentList.SelectedIndexChanged += new System.EventHandler(this.ContentList_SelectedIndexChanged);
            this.ContentList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ContentList_MouseDoubleClick);
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Name";
            this.NameColumn.Width = 500;
            // 
            // TagColumn
            // 
            this.TagColumn.Text = "Tag";
            // 
            // InstanceColumn
            // 
            this.InstanceColumn.Text = "Instance";
            this.InstanceColumn.Width = 175;
            // 
            // FilenameColumn
            // 
            this.FilenameColumn.Text = "Filename";
            this.FilenameColumn.Width = 900;
            // 
            // EditPreview
            // 
            this.EditPreview.AllowXmlFormatting = true;
            this.EditPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditPreview.Location = new System.Drawing.Point(0, 0);
            this.EditPreview.Margin = new System.Windows.Forms.Padding(4);
            this.EditPreview.Name = "EditPreview";
            this.EditPreview.ReadOnly = false;
            this.EditPreview.Size = new System.Drawing.Size(1123, 250);
            this.EditPreview.TabIndex = 1;
            // 
            // ToolBar
            // 
            this.ToolBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ButtonCompare,
            this.MenuSearch,
            this.XMLColoring,
            this.toolStripSeparator2,
            this.MenuOpen,
            this.toolStripSeparator6,
            this.UpdateAll,
            this.MenuSave,
            this.toolStripSeparator4,
            this.MenuHelp});
            this.ToolBar.Location = new System.Drawing.Point(0, 0);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.Size = new System.Drawing.Size(1123, 39);
            this.ToolBar.TabIndex = 3;
            this.ToolBar.Text = "toolStrip1";
            // 
            // ButtonCompare
            // 
            this.ButtonCompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ButtonCompare.Image = global::NRaasPacker.Properties.Resources.Compare;
            this.ButtonCompare.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ButtonCompare.Name = "ButtonCompare";
            this.ButtonCompare.Size = new System.Drawing.Size(36, 36);
            this.ButtonCompare.Text = "Compare Packages";
            this.ButtonCompare.Click += new System.EventHandler(this.ButtonCompare_Click);
            // 
            // MenuSearch
            // 
            this.MenuSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuSearch.Image = ((System.Drawing.Image)(resources.GetObject("MenuSearch.Image")));
            this.MenuSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuSearch.Name = "MenuSearch";
            this.MenuSearch.Size = new System.Drawing.Size(36, 36);
            this.MenuSearch.Text = "Search Packages";
            this.MenuSearch.Click += new System.EventHandler(this.MenuSearch_Click);
            // 
            // XMLColoring
            // 
            this.XMLColoring.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.XMLColoring.Image = ((System.Drawing.Image)(resources.GetObject("XMLColoring.Image")));
            this.XMLColoring.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.XMLColoring.Name = "XMLColoring";
            this.XMLColoring.Size = new System.Drawing.Size(36, 36);
            this.XMLColoring.Text = "Enable XML Formatting";
            this.XMLColoring.Visible = false;
            this.XMLColoring.Click += new System.EventHandler(this.XMLColoring_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // MenuOpen
            // 
            this.MenuOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuOpen.Image = ((System.Drawing.Image)(resources.GetObject("MenuOpen.Image")));
            this.MenuOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuOpen.Name = "MenuOpen";
            this.MenuOpen.Size = new System.Drawing.Size(36, 36);
            this.MenuOpen.Text = "&Open";
            this.MenuOpen.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 39);
            // 
            // UpdateAll
            // 
            this.UpdateAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.UpdateAll.Image = global::NRaasPacker.Properties.Resources.Button_software_update_icon;
            this.UpdateAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UpdateAll.Name = "UpdateAll";
            this.UpdateAll.Size = new System.Drawing.Size(36, 36);
            this.UpdateAll.Text = "Update All";
            this.UpdateAll.Click += new System.EventHandler(this.UpdateAll_Click);
            // 
            // MenuSave
            // 
            this.MenuSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuSave.Image = ((System.Drawing.Image)(resources.GetObject("MenuSave.Image")));
            this.MenuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.Size = new System.Drawing.Size(36, 36);
            this.MenuSave.Text = "&Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // MenuHelp
            // 
            this.MenuHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuHelp.Image = ((System.Drawing.Image)(resources.GetObject("MenuHelp.Image")));
            this.MenuHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Size = new System.Drawing.Size(36, 36);
            this.MenuHelp.Text = "He&lp";
            this.MenuHelp.Click += new System.EventHandler(this.MenuHelp_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(1123, 649);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.ToolBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "NRaas Packer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ListMenu.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.MasterSplit.Panel1.ResumeLayout(false);
            this.MasterSplit.Panel2.ResumeLayout(false);
            this.MasterSplit.ResumeLayout(false);
            this.ToolBar.ResumeLayout(false);
            this.ToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip ListMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuImport;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripMenuItem MenuDetails;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuExport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem s3SAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuUpdateVersion;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuImportSTBL;
        private System.Windows.Forms.ToolStripMenuItem MenuRenameSTBL;
        private System.Windows.Forms.ToolStrip ToolBar;
        private System.Windows.Forms.ToolStripButton MenuOpen;
        private System.Windows.Forms.ToolStripButton MenuSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem xMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuNewXML;
        private System.Windows.Forms.SplitContainer MasterSplit;
        private System.Windows.Forms.ListView ContentList;
        private System.Windows.Forms.ColumnHeader NameColumn;
        private System.Windows.Forms.ColumnHeader TagColumn;
        private System.Windows.Forms.ColumnHeader InstanceColumn;
        private System.Windows.Forms.ColumnHeader FilenameColumn;
        private System.Windows.Forms.ToolStripMenuItem MenuEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton MenuHelp;
        private System.Windows.Forms.ToolStripMenuItem lAYOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuNewLAYO;
        private System.Windows.Forms.ToolStripMenuItem iTUNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuNewITUN;
        private System.Windows.Forms.ToolStripMenuItem MenuDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton ButtonCompare;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem iMAGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuNewIMAG;
        private System.Windows.Forms.ToolStripMenuItem MenuNewS3SA;
        private System.Windows.Forms.ToolStripMenuItem TestAllSTBL;
        private System.Windows.Forms.ToolStripButton MenuSearch;
        private XmlEditor EditPreview;
        private System.Windows.Forms.ToolStripButton XMLColoring;
        private System.Windows.Forms.ToolStripMenuItem MenuImportEnglish;
        private System.Windows.Forms.ToolStripButton UpdateAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem ExportAll;
    }
}

