using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Conversion
{
    public partial class MainForm : Form
    {
        static MainForm()
        { }

        public MainForm()
        {
            InitializeComponent();
        }

        private void GetStage1(List<Tuple<string, string>> regEx)
        {
            regEx.Add(new Tuple<string, string>("(\\s)assembly(\\s)", "$1public$2"));
            regEx.Add(new Tuple<string, string>("(\\s)family(\\s)", "$1public$2"));
            regEx.Add(new Tuple<string, string>("(\\s)private(\\s)", "$1public$2"));
            regEx.Add(new Tuple<string, string>("(\\s)sealed(\\s)", "$1"));
            regEx.Add(new Tuple<string, string>("(\\s)initonly(\\s)", "$1"));
            regEx.Add(new Tuple<string, string>("\\.field (|static )assembly", ".field $1 public"));
        }

        private void GetStage2(List<Tuple<string, string>> regEx)
        {
            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname instance void(\\s*\\n\\s*add_)", ".method private hidebysig specialname instance void$1"));
        }

        private void GetStage3(List<Tuple<string, string>> regEx)
        {
            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname instance void(\\s*\\n\\s*remove_)", ".method private hidebysig specialname instance void$1"));
        }

        private void GetStage4(List<Tuple<string, string>> regEx)
        {
            regEx.Add(new Tuple<string, string>("\\.method public hidebysig newslot specialname virtual(|\\s*final)(\\s*\\n\\s*instance void  add_)", ".method private hidebysig newslot specialname virtual $1$2"));
            regEx.Add(new Tuple<string, string>("\\.method public hidebysig newslot specialname virtual(|\\s*final)(\\s*\\n\\s*instance void  remove_)", ".method private hidebysig newslot specialname virtual $1$2"));
        }

        private void GetStage5(List<Tuple<string, string>> regEx)
        {
            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname static(\\s*\\n\\s*void  add_)", ".method private hidebysig specialname static$1"));
        }

        private void GetStage6(List<Tuple<string, string>> regEx)
        {
            regEx.Add(new Tuple<string, string>("\\.method public hidebysig specialname static(\\s*\\n\\s*void  remove_)", ".method private hidebysig specialname static$1"));
        }

        private void Perform(string filename, List<Tuple<string, string>> regEx)
        {
            if (string.IsNullOrEmpty(filename)) return;

            string contents1 = null;
            string contents2 = null;

            try
            {
                Application.UseWaitCursor = true;

                using (StreamReader file = new StreamReader(filename))
                {
                    contents1 = file.ReadToEnd();
                }

                contents2 = contents1.Substring(contents1.Length / 2);
                contents1 = contents1.Substring(0, contents1.Length / 2);

                foreach (Tuple<string, string> value in regEx)
                {
                    Regex regex = new Regex(value.Item1);

                    contents1 = regex.Replace(contents1, value.Item2);
                    contents2 = regex.Replace(contents2, value.Item2);
                }

                using (StreamWriter file = new StreamWriter(filename))
                {
                    file.Write(contents1.ToCharArray());
                    file.Write(contents2.ToCharArray());
                }
            }
            finally
            {
                Application.UseWaitCursor = false;
            }

            MessageBox.Show("Complete");
        }

        private string GetFilename()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.FilterIndex = 1;
            dialog.Filter = "IL Files|*.il";
            dialog.DefaultExt = "il";
            dialog.InitialDirectory = "D:\\Downloads\\For Games\\Sims3 Downloads\\Hacks\\Coding\\Sims3\\Compiler";
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return null;

            return dialog.FileName;
        }

        private void ButtonConvert_Click(object sender, EventArgs e)
        {           
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage1(regEx);
            GetStage2(regEx);
            GetStage3(regEx);
            GetStage4(regEx);
            GetStage5(regEx);
            GetStage6(regEx);

            Perform(GetFilename(), regEx);
        }

        private void Stage1_Click(object sender, EventArgs e)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage1(regEx);

            Perform(GetFilename(), regEx);
        }

        private void Stage2_Click(object sender, EventArgs e)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage2(regEx);

            Perform(GetFilename(), regEx);
        }

        private void Stage3_Click(object sender, EventArgs e)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage3(regEx);

            Perform(GetFilename(), regEx);
        }

        private void Stage4_Click(object sender, EventArgs e)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage4(regEx);

            Perform(GetFilename(), regEx);
        }

        private void Stage5_Click(object sender, EventArgs e)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage5(regEx);

            Perform(GetFilename(), regEx);
        }

        private void Stage6_Click(object sender, EventArgs e)
        {
            List<Tuple<string, string>> regEx = new List<Tuple<string, string>>();
            GetStage6(regEx);

            Perform(GetFilename(), regEx);
        }
    }
}
