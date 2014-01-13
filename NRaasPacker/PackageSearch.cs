using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.Package;

namespace NRaasPacker
{
    public partial class PackageSearch : Form
    {
        public PackageSearch()
        {
            InitializeComponent();

            Folder.Text = Path.GetDirectoryName(MainForm.PublicFilename);
        }

        private void Search_Click(object sender, EventArgs e)
        {
            string instanceStr = Instance.Text;
            string groupStr = Group.Text;

            ulong instance = 0;

            try
            {
                instance = Convert.ToUInt64(instanceStr, 16);
            }
            catch
            {
                MessageBox.Show("Format of Instance is invalid.");
                return;
            }

            uint group = 0;

            if (!string.IsNullOrEmpty(groupStr))
            {
                try
                {
                    group = Convert.ToUInt32(groupStr, 16);
                }
                catch
                {
                    MessageBox.Show("Format of Group is invalid.");
                    return;
                }
            }

            string[] files = Directory.GetFiles(Folder.Text, "*.package", SearchOption.AllDirectories);
            if (files == null) return;

            Application.UseWaitCursor = true;

            try
            {
                MainForm.Log("Searching: " + Folder.Text, false);
                MainForm.Log("Instance: " + instanceStr, false);

                if (!string.IsNullOrEmpty(groupStr))
                {
                    MainForm.Log("Group: " + groupStr, false);
                }

                bool success = false;

                foreach (string file in files)
                {
                    IPackage package = null;

                    try
                    {
                        package = Package.OpenPackage(0, file, false);
                    }
                    catch
                    { }

                    if (package == null)
                    {
                        MainForm.Log("Unable to open " + file, true);
                    }
                    else if (string.IsNullOrEmpty(groupStr))
                    {
                        if (package.Find(value => (value.Instance == instance)) != null)
                        {
                            MainForm.Log("Match Found: " + file, true);
                            success = true;
                        }
                    }
                    else
                    {
                        if (package.Find(value => ((value.Instance == instance) && (value.ResourceGroup == group))) != null)
                        {
                            MainForm.Log("Match Found: " + file, true);
                            success = true;
                        }
                    }
                }

                if (!success)
                {
                    MainForm.Log("No Matches Found.", true);
                }

                MainForm.ExportLog("PackageSearch.log");
            }
            finally
            {
                Application.UseWaitCursor = false;
            }
        }

        private void ChooseFolder_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.SelectedPath = Path.GetDirectoryName(MainForm.PublicFilename);

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            Folder.Text = dialog.SelectedPath;
        }
    }
}
