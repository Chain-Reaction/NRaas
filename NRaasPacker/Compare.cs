using s3pi.Interfaces;
using s3pi.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker
{
    public partial class Compare : Form
    {
        public Compare()
        {
            InitializeComponent();
        }

        private void LeftButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog(); ;

            dialog.CheckFileExists = true;
            dialog.FileName = "";
            dialog.Filter = "DBPF Packages|*.package|All Files|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() != DialogResult.OK) return;

            LeftText.Text = dialog.FileName;
        }

        private void RightButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog(); ;

            dialog.CheckFileExists = true;
            dialog.FileName = "";
            dialog.Filter = "DBPF Packages|*.package|All Files|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() != DialogResult.OK) return;

            RightText.Text = dialog.FileName;
        }

        private void ResultButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog(); ;

            dialog.CheckPathExists = true;
            dialog.FileName = "";
            dialog.Filter = "DBPF Packages|*.package|All Files|*.*";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() != DialogResult.OK) return;

            ResultsText.Text = dialog.FileName;
        }

        private void CompareFiles_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(LeftText.Text))
            {
                System.Windows.Forms.MessageBox.Show("The Left File has not been selected.", "Compare", MessageBoxButtons.OK);
                return;
            }
            else if (string.IsNullOrEmpty(RightText.Text))
            {
                System.Windows.Forms.MessageBox.Show("The Right File has not been selected.", "Compare", MessageBoxButtons.OK);
                return;
            }
            else if (string.IsNullOrEmpty(ResultsText.Text))
            {
                System.Windows.Forms.MessageBox.Show("The Results File has not been selected.", "Compare", MessageBoxButtons.OK);
                return;
            }

            IPackage leftPackage = Package.OpenPackage(0, LeftText.Text);
            if (leftPackage == null)
            {
                System.Windows.Forms.MessageBox.Show("The Left File could not be opened.", "Compare", MessageBoxButtons.OK);
                return;
            }

            IPackage rightPackage = Package.OpenPackage(0, RightText.Text);
            if (rightPackage == null)
            {
                System.Windows.Forms.MessageBox.Show("The Right File could not be opened.", "Compare", MessageBoxButtons.OK);

                Package.ClosePackage(0, leftPackage);
                return;
            }

            IPackage resultsPackage = Package.NewPackage(0);
            if (resultsPackage == null)
            {
                System.Windows.Forms.MessageBox.Show("The Right File could not be opened.", "Compare", MessageBoxButtons.OK);

                Package.ClosePackage(0, leftPackage);
                Package.ClosePackage(0, rightPackage);
                return;
            }

            try
            {
                UseWaitCursor = true;

                NameMapResource.NameMapResource keyResource = null;

                IResourceIndexEntry leftKey = leftPackage.Find(key => key.ResourceType == (uint)ResourceType._KEY);
                if (leftKey != null)
                {
                    NameMapResource.NameMapResource leftResource = ResourceHandlers.CreateResource(leftKey, leftPackage) as NameMapResource.NameMapResource;

                    IResourceIndexEntry keyEntry = resultsPackage.AddResource(new AResource.TGIBlock(0, null, leftKey.ResourceType, leftKey.ResourceGroup, leftKey.Instance), leftResource.Stream, false);
                    if (keyEntry != null)
                    {
                        keyResource = ResourceHandlers.CreateResource(keyEntry, resultsPackage) as NameMapResource.NameMapResource;
                    }
                }

                IResourceIndexEntry rightKey = rightPackage.Find(key => key.ResourceType == (uint)ResourceType._KEY);
                if (rightKey != null)
                {
                    NameMapResource.NameMapResource rightResource = ResourceHandlers.CreateResource(rightKey, rightPackage) as NameMapResource.NameMapResource;

                    if (keyResource == null)
                    {
                        IResourceIndexEntry keyEntry = resultsPackage.AddResource(new AResource.TGIBlock(0, null, rightKey.ResourceType, rightKey.ResourceGroup, rightKey.Instance), rightResource.Stream, false);
                        if (keyEntry != null)
                        {
                            keyResource = ResourceHandlers.CreateResource(keyEntry, resultsPackage) as NameMapResource.NameMapResource;
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<ulong, string> element in rightResource)
                        {
                            if (keyResource.ContainsKey(element.Key)) continue;

                            keyResource.Add(element.Key, element.Value);
                        }
                    }
                }

                IList<IResourceIndexEntry> leftList = leftPackage.FindAll(value => { return true; });
                foreach (IResourceIndexEntry left in leftList)
                {
                    ResourceType type = (ResourceType)left.ResourceType;

                    bool test = false;
                    switch (type)
                    {
                        case ResourceType._XML:
                        case ResourceType.ITUN:
                            test = true;
                            break;
                    }

                    if (!test) continue;

                    IResourceIndexEntry right = rightPackage.Find(value => { return ((value.Instance == left.Instance) && (value.ResourceGroup == left.ResourceGroup) && (value.ResourceType == left.ResourceType)); });
                    if (right == null)
                    {
                        if (CheckCopyMissing.Checked)
                        {
                            TextResource.TextResource leftResource = ResourceHandlers.CreateResource(left, leftPackage) as TextResource.TextResource;

                            resultsPackage.AddResource(new AResource.TGIBlock(0, null, left.ResourceType, left.ResourceGroup, left.Instance), leftResource.Stream, false);
                        }
                    }
                    else
                    {
                        TextResource.TextResource leftResource = ResourceHandlers.CreateResource(left, leftPackage) as TextResource.TextResource;

                        StreamReader leftReader = leftResource.TextFileReader as StreamReader;
                        leftReader.BaseStream.Position = 0;
                        string leftText = leftReader.ReadToEnd();

                        TextResource.TextResource rightResource = ResourceHandlers.CreateResource(right, rightPackage) as TextResource.TextResource;

                        StreamReader rightReader = rightResource.TextFileReader as StreamReader;
                        rightReader.BaseStream.Position = 0;
                        string rightText = rightReader.ReadToEnd();

                        if (!leftText.Equals(rightText))
                        {
                            resultsPackage.AddResource(new AResource.TGIBlock(0, null, left.ResourceType, left.ResourceGroup, left.Instance), leftResource.Stream, false);
                        }
                    }
                }

                IList<IResourceIndexEntry> rightList = rightPackage.FindAll(value => { return true; });
                foreach (IResourceIndexEntry right in rightList)
                {
                    ResourceType type = (ResourceType)right.ResourceType;

                    bool test = false;
                    switch (type)
                    {
                        case ResourceType._XML:
                        case ResourceType.ITUN:
                            test = true;
                            break;
                    }

                    if (!test) continue;

                    IResourceIndexEntry left = rightPackage.Find(value => { return ((value.Instance == right.Instance) && (value.ResourceGroup == right.ResourceGroup) && (value.ResourceType == right.ResourceType)); });
                    if (left == null)
                    {
                        if (CheckCopyMissing.Checked)
                        {
                            TextResource.TextResource rightResource = ResourceHandlers.CreateResource(right, rightPackage) as TextResource.TextResource;

                            resultsPackage.AddResource(new AResource.TGIBlock(0, null, right.ResourceType, right.ResourceGroup, right.Instance), rightResource.Stream, false);
                        }
                    }
                }

                resultsPackage.SaveAs(ResultsText.Text);
            }
            finally
            {
                Package.ClosePackage(0, leftPackage);
                Package.ClosePackage(0, rightPackage);
                Package.ClosePackage(0, resultsPackage);

                UseWaitCursor = false;
            }

            System.Windows.Forms.MessageBox.Show("Complete.", "Compare", MessageBoxButtons.OK);
        }
    }
}
