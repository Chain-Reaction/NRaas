using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NRaasPacker
{
    public partial class TextInput : Form
    {
        string mResult;

        public TextInput(string title, string result)
        {
            InitializeComponent();

            Text = title;

            mResult = result;

            TextValue.Text = mResult;
        }

        public string Result
        {
            get { return mResult; }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            mResult = TextValue.Text;

            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
