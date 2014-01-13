namespace NRaasPacker
{
    partial class Compare
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
            this.LeftButton = new System.Windows.Forms.Button();
            this.LeftText = new System.Windows.Forms.TextBox();
            this.RightButton = new System.Windows.Forms.Button();
            this.RightText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ResultsText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultButton = new System.Windows.Forms.Button();
            this.CompareFiles = new System.Windows.Forms.Button();
            this.CheckCopyMissing = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LeftButton
            // 
            this.LeftButton.Location = new System.Drawing.Point(784, 9);
            this.LeftButton.Name = "LeftButton";
            this.LeftButton.Size = new System.Drawing.Size(75, 23);
            this.LeftButton.TabIndex = 0;
            this.LeftButton.Text = "Browse";
            this.LeftButton.UseVisualStyleBackColor = true;
            this.LeftButton.Click += new System.EventHandler(this.LeftButton_Click);
            // 
            // LeftText
            // 
            this.LeftText.Location = new System.Drawing.Point(85, 12);
            this.LeftText.Name = "LeftText";
            this.LeftText.ReadOnly = true;
            this.LeftText.Size = new System.Drawing.Size(693, 22);
            this.LeftText.TabIndex = 1;
            // 
            // RightButton
            // 
            this.RightButton.Location = new System.Drawing.Point(784, 38);
            this.RightButton.Name = "RightButton";
            this.RightButton.Size = new System.Drawing.Size(75, 23);
            this.RightButton.TabIndex = 2;
            this.RightButton.Text = "Browse";
            this.RightButton.UseVisualStyleBackColor = true;
            this.RightButton.Click += new System.EventHandler(this.RightButton_Click);
            // 
            // RightText
            // 
            this.RightText.Location = new System.Drawing.Point(85, 41);
            this.RightText.Name = "RightText";
            this.RightText.ReadOnly = true;
            this.RightText.Size = new System.Drawing.Size(693, 22);
            this.RightText.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Left File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Right File";
            // 
            // ResultsText
            // 
            this.ResultsText.Location = new System.Drawing.Point(85, 89);
            this.ResultsText.Name = "ResultsText";
            this.ResultsText.ReadOnly = true;
            this.ResultsText.Size = new System.Drawing.Size(693, 22);
            this.ResultsText.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Results";
            // 
            // ResultButton
            // 
            this.ResultButton.Location = new System.Drawing.Point(784, 89);
            this.ResultButton.Name = "ResultButton";
            this.ResultButton.Size = new System.Drawing.Size(75, 23);
            this.ResultButton.TabIndex = 8;
            this.ResultButton.Text = "Browse";
            this.ResultButton.UseVisualStyleBackColor = true;
            this.ResultButton.Click += new System.EventHandler(this.ResultButton_Click);
            // 
            // CompareFiles
            // 
            this.CompareFiles.Location = new System.Drawing.Point(684, 134);
            this.CompareFiles.Name = "CompareFiles";
            this.CompareFiles.Size = new System.Drawing.Size(94, 36);
            this.CompareFiles.TabIndex = 9;
            this.CompareFiles.Text = "Compare";
            this.CompareFiles.UseVisualStyleBackColor = true;
            this.CompareFiles.Click += new System.EventHandler(this.CompareFiles_Click);
            // 
            // CheckCopyMissing
            // 
            this.CheckCopyMissing.AutoSize = true;
            this.CheckCopyMissing.Checked = true;
            this.CheckCopyMissing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckCopyMissing.Location = new System.Drawing.Point(85, 117);
            this.CheckCopyMissing.Name = "CheckCopyMissing";
            this.CheckCopyMissing.Size = new System.Drawing.Size(346, 21);
            this.CheckCopyMissing.TabIndex = 10;
            this.CheckCopyMissing.Text = "Copy items that are missing in one or the other file";
            this.CheckCopyMissing.UseVisualStyleBackColor = true;
            // 
            // Compare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 182);
            this.Controls.Add(this.CheckCopyMissing);
            this.Controls.Add(this.CompareFiles);
            this.Controls.Add(this.ResultButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ResultsText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RightText);
            this.Controls.Add(this.RightButton);
            this.Controls.Add(this.LeftText);
            this.Controls.Add(this.LeftButton);
            this.Name = "Compare";
            this.Text = "Compare";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LeftButton;
        private System.Windows.Forms.TextBox LeftText;
        private System.Windows.Forms.Button RightButton;
        private System.Windows.Forms.TextBox RightText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ResultsText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ResultButton;
        private System.Windows.Forms.Button CompareFiles;
        private System.Windows.Forms.CheckBox CheckCopyMissing;
    }
}