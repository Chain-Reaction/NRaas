using NRaasPacker.ListItems;
using s3pi.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker
{
    public partial class Details : Form
    {
        ListItem mItem;

        IPackage mPackage;

        public Details(ListItem item, IPackage package)
        {
            InitializeComponent();

            mItem = item;
            mPackage = package;
            
            NameText.Text = mItem.Text;

            InstanceText.Text = mItem.Instance;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            mItem.Instance = InstanceText.Text;

            mItem.Name = NameText.Text;

            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NameText_TextChanged(object sender, EventArgs e)
        {
            InstanceText.Text = "0x" + FNV64.GetHash(NameText.Text).ToString("X16");
        }
    }
}
