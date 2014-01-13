using NRaasPacker.ListItems;
using NRaasPacker.MenuUpdaters;
using s3pi.Interfaces;
using s3pi.Package;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker
{
    public partial class MainForm : Form
    {
        static string sFilename;
        static string sPackageName;

        static MainForm sMainForm;

        static StringBuilder sLog = new StringBuilder();
        static bool sLogError = false;

        static bool sPreviewChanged;

        IPreviewListItem mActivePreview = null;

        IPackage mPackage = null;

        static bool sChanged = false;

        public MainForm()
        {
            sMainForm = this;

            InitializeComponent();

            Properties.Settings.Default.Reload();

            OnPackageChanged += OnListElements;

            MasterSplit.Panel2Collapsed = true;

            EditPreview.AllowXmlFormatting = NRaasPacker.Properties.Settings.Default.XMLColoring;
        }
        public MainForm(params string[] args)
            : this()
        {
            if (args.Length > 0)
            {
                Filename = args[0];
            }
        }

        public delegate void PackageChanged(IPackage package);
        public PackageChanged OnPackageChanged;

        IPackage CurrentPackage
        {
            get { return mPackage; }
            set
            {
                if (mPackage == value) return;

                if (!SavePackage(true)) return;

                if (mPackage != null)
                {
                    Package.ClosePackage(0, mPackage);
                }

                mPackage = value;

                if (OnPackageChanged != null)
                {
                    OnPackageChanged(mPackage);
                }
            }
        }

        public static void Log(string text, bool error)
        {
            sLog.Append(text + System.Environment.NewLine);
            if (error)
            {
                sLogError = true;
            }
        }

        public static void ExportLog()
        {
            ExportLog(sFilename + ".log");
        }
        public static void ExportLog(string filename)
        {
            string results = sLog.ToString();
            if (!sLogError) return;

            using (StreamWriter file = new StreamWriter(filename, false, Encoding.UTF8))
            {
                file.Write(results);
            }

            sLog = new StringBuilder();
            sLogError = false;

            if (MessageBox.Show("A log named " + filename + " has been created, do you wish to open it now ?", "Errors", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(filename);
            }
        }

        public static string PackagePrefix
        {
            get
            {
                string prefix = PackageName;
                if (string.IsNullOrEmpty(prefix)) return "";

                int index = prefix.IndexOf('_');
                if (index < 0) return "";

                return prefix.Substring(0, index);
            }
        }

        public static string PackageName
        {
            get
            {
                return sPackageName;
            }
        }

        public static string PublicFilename
        {
            get
            {
                return sFilename;
            }
        }

        private string Filename
        {
            get
            {
                return PublicFilename;
            }
            set
            {
                if (!File.Exists(value)) return;

                sFilename = value;

                sPackageName = Path.GetFileNameWithoutExtension(sFilename);

                Text = sFilename;
                Application.DoEvents();

                bool success = false;

                try
                {
                    CurrentPackage = Package.OpenPackage(0, sFilename, true);
                    success = true;
                }
                catch
                {}

                if (!success)
                {
                    try
                    {
                        CurrentPackage = Package.OpenPackage(0, sFilename, false);
                        success = true;

                        MessageBox.Show("File opened in read-only mode.", "Load", MessageBoxButtons.OK);
                    }
                    catch
                    {}
                }

                if (!success)
                {
                    MessageBox.Show("File could not be opened.", "Load", MessageBoxButtons.OK);
                }
            }
        }

        public static IResource CreateKeyResource(IPackage package)
        {
            IResourceIndexEntry key = package.Find(value => value.ResourceType == (uint)ResourceType._KEY);

            if (key == null)
            {
                return null;
            }
            else
            {
                return ResourceHandlers.CreateResource(key, package);
            }
        }

        private void OnListElements(IPackage package)
        {
            try
            {
                sChanged = false;

                Application.UseWaitCursor = true;
                ContentList.BeginUpdate();
                ContentList.Enabled = false;

                ContentList.Items.Clear();

                IList<IResourceIndexEntry> list = package.FindAll(value => { return true; });
                foreach (IResourceIndexEntry entry in list)
                {
                    ListItem item = ListItem.CreateHandler(entry, package);
                    if (item == null) continue;

                    ContentList.Items.Add(item);
                }
            }
            finally
            {
                ContentList.Enabled = true;
                ContentList.EndUpdate();
                Application.UseWaitCursor = false;
            }
        }

        public static void IssueError(Exception exception, string message)
        {
            MessageBox.Show(message + "\n\n" + exception, "Error", MessageBoxButtons.OK);
        }

        private void MenuOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();;

            dialog.FileName = "";
            dialog.Filter = "DBPF Packages|*.package|All Files|*.*";
            dialog.FilterIndex = 1;
            dialog.InitialDirectory = PathStore.GetPath("", "package");
            if (dialog.ShowDialog() != DialogResult.OK) return;

            Filename = dialog.FileName;

            PathStore.SetPath("", "package", Path.GetDirectoryName(dialog.FileName));
        }

        private bool SavePackage(bool prompt)
        {
            if (CurrentPackage == null) return true;

            SavePreview();

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (sChanged)
                {
                    DialogResult result = DialogResult.Yes;
                    if (prompt)
                    {
                        result = MessageBox.Show("Do you wish to save your changes?", "Save", MessageBoxButtons.YesNoCancel);
                    }

                    switch (result)
                    {
                        case DialogResult.Yes:
                            IResourceIndexEntry keyEntry = CurrentPackage.Find(key => key.ResourceType == (uint)ResourceType._KEY);
                            if (keyEntry != null)
                            {
                                Dictionary<ulong, bool> existing = new Dictionary<ulong, bool>();

                                IList<IResourceIndexEntry> list = CurrentPackage.FindAll(value => { return true; });
                                foreach (IResourceIndexEntry entry in list)
                                {
                                    if (existing.ContainsKey(entry.Instance)) continue;

                                    existing.Add(entry.Instance, true);
                                }

                                NameMapResource.NameMapResource keyResource = CreateKeyResource(CurrentPackage) as NameMapResource.NameMapResource;

                                List<ulong> badKeys = new List<ulong>();

                                foreach (KeyValuePair<ulong, string> element in keyResource)
                                {
                                    if (!existing.ContainsKey(element.Key))
                                    {
                                        badKeys.Add(element.Key);
                                    }
                                }

                                foreach (ulong key in badKeys)
                                {
                                    keyResource.Remove(key);
                                }

                                CurrentPackage.ReplaceResource(keyEntry, keyResource);
                            }

                            CurrentPackage.SavePackage();

                            OnListElements(CurrentPackage);
                            break;
                        case DialogResult.No:
                            break;
                        case DialogResult.Cancel:
                            return false;
                    }
                }
            }
            finally
            {
                Application.UseWaitCursor = false;
            }

            return true;
        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            if (Filename == null) return;

            if (!sChanged) return;

            SavePackage(false);
        }

        private void MenuDetails_Click(object sender, EventArgs e)
        {
            ListItem item = ContentList.SelectedItems[0] as ListItem;

            Details dialog = new Details(item, CurrentPackage);

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            sChanged = true;
        }

        public class MenuDetailsUpdater : MenuUpdater
        {
            public MenuDetailsUpdater()
                : base(sMainForm.MenuDetails)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = (selection.Count == 1);
            }
        }

        private void ContentList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ImportSelected(true);
        }

        private bool ImportFile(ListItem listItem, bool autoSet)
        {
            if (listItem.Import(autoSet))
            {
                sChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ImportFile(ListItem listItem, Exporters.Exporter exporter)
        {           
            if (listItem.GetImporter().Import(exporter, CurrentPackage))
            {
                sChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ImportSelected(bool autoSet)
        {
            Application.UseWaitCursor = true;

            try
            {
                foreach (ListItem selected in ContentList.SelectedItems)
                {
                    if (!ImportFile(selected, autoSet)) return;
                }

                ExportLog();
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        private void MenuImport_Click(object sender, EventArgs e)
        {
            ImportSelected(ContentList.SelectedItems.Count > 1);
        }

        public class MenuImportUpdater : MenuUpdater
        {
            public MenuImportUpdater()
                : base(sMainForm.MenuImport)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (selection.Count == 0) return;

                if (selection.Count > 1)
                {
                    foreach (ListViewItem selected in selection)
                    {
                        ListItem listItem = selected as ListItem;
                        if (listItem == null) continue;

                        if (!(listItem is STBLListItem)) return;
                    }
                }

                mItem.Enabled = true;
            }
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SavePackage(true))
            {
                e.Cancel = true;
                return;
            }

            Properties.Settings.Default.Save();

            sMainForm = null;
        }

        private void MenuRenameSTBL_Click(object sender, EventArgs e)
        {
            string scriptName = null;

            foreach (ListItem item in ContentList.Items)
            {
                if (item is S3SAListItem)
                {
                    scriptName = item.Text;
                    break;
                }
            }

            TextInput dialog = new TextInput("HashName and Prefix", scriptName);

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string[] results = dialog.Result.Split(' ');

            ulong result = FNV64.GetHash(results[0]);

            string orig = result.ToString("X16");
            
            int origValue = int.Parse(orig.Substring(0,2), System.Globalization.NumberStyles.HexNumber, null);
            if (origValue < STBL.Count)
            {
                result++;

                orig = result.ToString("X16");
            }

            string part = orig.Substring(2);

            string prefix = null;
            if (results.Length > 1)
            {
                prefix = results[1] + " ";
            }

            foreach(string aPrefix in STBL.Keys)
            {
                ulong instance = Convert.ToUInt64("0x" + aPrefix + part, 16);

                bool found = false;
                foreach (IResourceIndexEntry entry in CurrentPackage.FindAll(key => ((key.ResourceType == (uint)ResourceType.STBL))))
                {
                    string instanceStr = entry["Instance"];

                    if (instanceStr.Substring(2, 2) == aPrefix)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    STBLListItem stblItem = new STBLListItem();

                    if (stblItem.AddNew(instance, CurrentPackage, true))
                    {
                        ContentList.Items.Add(stblItem);
                    }
                }
            }

            foreach (ListItem item in ContentList.Items)
            {
                if (!(item is STBLListItem)) continue;

                item.Instance = "0x" + STBL.GetPrefix(item.Instance) + part;

                item.Name = "Strings " + prefix + STBL.GetProperName(item.Instance);

                sChanged = true;
            }
        }

        public class MenuRenameSTBLUpdater : MenuUpdater
        {
            public MenuRenameSTBLUpdater()
                : base(sMainForm.MenuRenameSTBL)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuImportSTBL_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;

            try
            {
                foreach (ListItem item in ContentList.Items)
                {
                    STBLListItem stblItem = item as STBLListItem;
                    if (stblItem == null) continue;

                    if (!ImportFile(stblItem, true)) return;
                }

                foreach (ListItem item in ContentList.Items)
                {
                    STBLListItem stblItem = item as STBLListItem;
                    if (stblItem == null) continue;

                    stblItem.Test(sMainForm.CurrentPackage);
                }

                ExportLog();
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        public class MenuImportSTBLUpdater : MenuUpdater
        {
            public MenuImportSTBLUpdater()
                : base(sMainForm.MenuImportSTBL)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                if (sMainForm.CurrentPackage.Find(value => value.ResourceType == (uint)ResourceType.STBL) == null) return;

                mItem.Enabled = true;
            }
        }

        private void TestAllSTBL_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;

            try
            {
                foreach (ListItem item in ContentList.Items)
                {
                    STBLListItem stblItem = item as STBLListItem;
                    if (stblItem == null) continue;

                    stblItem.Test(sMainForm.CurrentPackage);
                }

                ExportLog();
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        public class MenuTestAllSTBLUpdater : MenuUpdater
        {
            public MenuTestAllSTBLUpdater()
                : base(sMainForm.TestAllSTBL)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                if (sMainForm.CurrentPackage.Find(value => value.ResourceType == (uint)ResourceType.STBL) == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuResetPaths_Click(object sender, EventArgs e)
        {
            PathStore.Reset(PackageName);
        }

        private void MenuUpdateVersion_Click(object sender, EventArgs e)
        {
            string version = null;

            foreach (ListItem listItem in ContentList.SelectedItems)
            {
                S3SAListItem item = listItem as S3SAListItem;
                if (item == null) continue;

                version = item.Version;

                if (!string.IsNullOrEmpty(version))
                {
                    break;
                }
            }

            TextInput dialog = new TextInput("Version", version);
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            if (version == dialog.Result) return;

            version = dialog.Result;

            foreach (ListItem listItem in ContentList.SelectedItems)
            {
                S3SAListItem item = listItem as S3SAListItem;
                if (item == null) continue;

                item.Version = version;
            }

            sChanged = true;
        }

        public class MenuUpdateVersionUpdater : MenuUpdater
        {
            public MenuUpdateVersionUpdater()
                : base(sMainForm.MenuUpdateVersion)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                bool found = false;

                foreach (ListItem listItem in selection)
                {
                    S3SAListItem s3saItem = listItem as S3SAListItem;
                    if (s3saItem == null) continue;

                    if (!string.IsNullOrEmpty(s3saItem.Version))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return;

                mItem.Enabled = true;
            }
        }

        private void MenuExport_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;

            try
            {
                foreach (ListItem selected in ContentList.SelectedItems)
                {
                    if (!selected.Export(false)) return;
                }
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        public class MenuExportUpdater : MenuUpdater
        {
            public MenuExportUpdater()
                : base(sMainForm.MenuExport)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (selection.Count == 0) return;

                mItem.Enabled = true;
            }
        }

        private void MenuNewXML_Click(object sender, EventArgs e)
        {
            TextListItem item = new TextListItem(ResourceType._XML);

            if (item.AddNew(0, CurrentPackage, false))
            {
                ContentList.Items.Add(item);

                sChanged = true;
            }
        }

        public class MenuNewXMLUpdater : MenuUpdater
        {
            public MenuNewXMLUpdater()
                : base(sMainForm.MenuNewXML)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuNewLAYO_Click(object sender, EventArgs e)
        {
            TextListItem item = new TextListItem(ResourceType.LAYO);

            if (item.AddNew(0, CurrentPackage, false))
            {
                ContentList.Items.Add(item);

                sChanged = true;
            }
        }

        public class MenuNewLAYOUpdater : MenuUpdater
        {
            public MenuNewLAYOUpdater()
                : base(sMainForm.MenuNewLAYO)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuNewS3SA_Click(object sender, EventArgs e)
        {
            S3SAListItem item = new S3SAListItem();

            if (item.AddNew(0, CurrentPackage, false))
            {
                ContentList.Items.Add(item);

                sChanged = true;
            }
        }

        public class MenuNewS3SAUpdater : MenuUpdater
        {
            public MenuNewS3SAUpdater()
                : base(sMainForm.MenuNewS3SA)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuNewITUN_Click(object sender, EventArgs e)
        {
            TextListItem item = new TextListItem(ResourceType.ITUN);

            if (item.AddNew(0, CurrentPackage, false))
            {
                ContentList.Items.Add(item);

                sChanged = true;
            }
        }

        public class MenuNewITUNUpdater : MenuUpdater
        {
            public MenuNewITUNUpdater()
                : base(sMainForm.MenuNewITUN)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete the selected resources?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;

            List<ListViewItem> entries = new List<ListViewItem>();

            foreach (ListItem listItem in ContentList.SelectedItems)
            {
                entries.Add(listItem);
            }

            foreach(ListItem listItem in entries)
            {
                if (listItem.Delete(CurrentPackage))
                {
                    sChanged = true;
                }
            }
        }

        public class MenuDeleteUpdater : MenuUpdater
        {
            public MenuDeleteUpdater()
                : base(sMainForm.MenuDelete)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (selection.Count == 0) return;

                mItem.Enabled = true;
            }
        }


        private void SavePreview()
        {
            if ((mActivePreview != null) && (sPreviewChanged))
            {
                mActivePreview.SetContents (EditPreview.Text, CurrentPackage);
                sChanged = true;
                sPreviewChanged = false;
            }
        }

        private void ContentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<MenuUpdaters.MenuUpdater> updaters = DerivativeSearch.Find<MenuUpdaters.MenuUpdater>(true);

            foreach (MenuUpdaters.MenuUpdater updater in updaters)
            {
                updater.Update(ContentList.SelectedItems);
            }

            SavePreview();

            string result = "";

            if (!MasterSplit.Panel2Collapsed)
            {
                if (ContentList.SelectedItems.Count == 1)
                {
                    mActivePreview = ContentList.SelectedItems[0] as IPreviewListItem;
                    if (mActivePreview != null)
                    {
                        result = mActivePreview.GetContents (CurrentPackage);
                    }
                }
                else
                {
                    mActivePreview = null;
                }
            }

            sPreviewChanged = true;
            EditPreview.Text = result;
            sPreviewChanged = false;
        }

        public static bool PreviewChanged
        {
            set
            {
                if (!sPreviewChanged)
                {
                    sChanged = true;
                }
                sPreviewChanged = true;
            }
        }

        private void MenuEdit_Click(object sender, EventArgs e)
        {
            MasterSplit.Panel2Collapsed = !MasterSplit.Panel2Collapsed;

            ContentList_SelectedIndexChanged(null, null);
        }

        public class MenuEditUpdater : MenuUpdater
        {
            public MenuEditUpdater()
                : base(sMainForm.MenuEdit)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                mItem.Checked = !sMainForm.MasterSplit.Panel2Collapsed;

                if (selection.Count != 1) return;

                IPreviewListItem textItem = selection[0] as IPreviewListItem;
                if (textItem == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuHelp_Click(object sender, EventArgs e)
        {
            AboutBox dialog = new AboutBox();
            dialog.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UpgradeSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeSettings = false;
            }
        }

        private void ButtonCompare_Click(object sender, EventArgs e)
        {
            if (!SavePackage(true)) return;

            Compare dialog = new Compare();
            dialog.ShowDialog();
        }

        private void MenuNewIMAG_Click(object sender, EventArgs e)
        {
            IMAGListItem item = new IMAGListItem();

            if (item.AddNew(0, CurrentPackage, false))
            {
                ContentList.Items.Add(item);

                sChanged = true;
            }
        }

        public class MenuNewIMAGUpdater : MenuUpdater
        {
            public MenuNewIMAGUpdater()
                : base(sMainForm.MenuNewIMAG)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                mItem.Enabled = true;
            }
        }

        private void MenuSearch_Click(object sender, EventArgs e)
        {
            PackageSearch dialog = new PackageSearch();
            dialog.ShowDialog();
        }

        private void XMLColoring_Click(object sender, EventArgs e)
        {
            EditPreview.AllowXmlFormatting = !EditPreview.AllowXmlFormatting;

            NRaasPacker.Properties.Settings.Default.XMLColoring = EditPreview.AllowXmlFormatting;
        }

        private void MenuImportEnglish_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Import the English translation into all languages?", "Import English", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            Application.UseWaitCursor = true;

            try
            {
                Exporters.Exporter englishExporter = null;

                foreach (ListItem item in ContentList.Items)
                {
                    STBLListItem stblItem = item as STBLListItem;
                    if (stblItem == null) continue;

                    if (STBL.GetPrefix(stblItem.Instance) == "00")
                    {
                        englishExporter = stblItem.GetExporter ();
                        break;
                    }
                }

                foreach (ListItem item in ContentList.Items)
                {
                    STBLListItem stblItem = item as STBLListItem;
                    if (stblItem == null) continue;

                    if (!ImportFile(stblItem, englishExporter)) return;
                }

                foreach (ListItem item in ContentList.Items)
                {
                    STBLListItem stblItem = item as STBLListItem;
                    if (stblItem == null) continue;

                    stblItem.Test(sMainForm.CurrentPackage);
                }

                ExportLog();
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        public class MenuImportEnglishUpdater : MenuUpdater
        {
            public MenuImportEnglishUpdater()
                : base(sMainForm.MenuImportEnglish)
            { }

            public override void Update(ListView.SelectedListViewItemCollection selection)
            {
                mItem.Enabled = false;

                if (sMainForm.CurrentPackage == null) return;

                if (sMainForm.CurrentPackage.Find(value => value.ResourceType == (uint)ResourceType.STBL) == null) return;

                mItem.Enabled = true;
            }
        }

        private void UpdateAll_Click(object sender, EventArgs e)
        {
            foreach (ListItem selected in ContentList.Items)
            {
                S3SAListItem s3saItem = selected as S3SAListItem;
                if (s3saItem == null) continue;

                if (!ImportFile(selected, true)) return;
            }

            MenuImportSTBL_Click(sender, e);

            SavePackage(false);
        }

        private void ExportAllSTBL_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Export all STBL ?  Doing so will overwrite any existing files.", "Export", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            foreach (ListItem item in ContentList.Items)
            {
                STBLListItem stblItem = item as STBLListItem;
                if (stblItem == null) continue;

                switch (STBL.GetPrefix(stblItem.Instance))
                {
                    case "00":
                    case "17":
                        continue;
                }

                stblItem.Export(true, false);
            }

            // Do the English separately to ensure that it was not overwritten by another export
            foreach (ListItem item in ContentList.Items)
            {
                STBLListItem stblItem = item as STBLListItem;
                if (stblItem == null) continue;

                if (STBL.GetPrefix(stblItem.Instance) != "00") continue;

                stblItem.Export(true, false);
            }

            MessageBox.Show("Exported");
        }

        /*
        private void ButtonRemoveSnaps_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog(); ;

            dialog.CheckFileExists = true;
            dialog.FileName = "";
            dialog.Filter = "Neighborhood Packages|*.nhd|All Files|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() != DialogResult.OK) return;

            LeftText.Text = dialog.FileName;

        }
        */
    }
}
