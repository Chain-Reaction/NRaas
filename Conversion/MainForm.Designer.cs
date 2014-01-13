namespace Conversion
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
            this.ButtonConvert = new System.Windows.Forms.Button();
            this.Stage1 = new System.Windows.Forms.Button();
            this.Stage2 = new System.Windows.Forms.Button();
            this.Stage3 = new System.Windows.Forms.Button();
            this.Stage4 = new System.Windows.Forms.Button();
            this.Stage5 = new System.Windows.Forms.Button();
            this.Stage6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonConvert
            // 
            this.ButtonConvert.Location = new System.Drawing.Point(89, 12);
            this.ButtonConvert.Name = "ButtonConvert";
            this.ButtonConvert.Size = new System.Drawing.Size(75, 23);
            this.ButtonConvert.TabIndex = 0;
            this.ButtonConvert.Text = "Convert";
            this.ButtonConvert.UseVisualStyleBackColor = true;
            this.ButtonConvert.Click += new System.EventHandler(this.ButtonConvert_Click);
            // 
            // Stage1
            // 
            this.Stage1.Location = new System.Drawing.Point(95, 68);
            this.Stage1.Name = "Stage1";
            this.Stage1.Size = new System.Drawing.Size(75, 23);
            this.Stage1.TabIndex = 1;
            this.Stage1.Text = "Stage 1";
            this.Stage1.UseVisualStyleBackColor = true;
            this.Stage1.Click += new System.EventHandler(this.Stage1_Click);
            // 
            // Stage2
            // 
            this.Stage2.Location = new System.Drawing.Point(95, 97);
            this.Stage2.Name = "Stage2";
            this.Stage2.Size = new System.Drawing.Size(75, 23);
            this.Stage2.TabIndex = 2;
            this.Stage2.Text = "Stage 2";
            this.Stage2.UseVisualStyleBackColor = true;
            this.Stage2.Click += new System.EventHandler(this.Stage2_Click);
            // 
            // Stage3
            // 
            this.Stage3.Location = new System.Drawing.Point(95, 124);
            this.Stage3.Name = "Stage3";
            this.Stage3.Size = new System.Drawing.Size(75, 23);
            this.Stage3.TabIndex = 3;
            this.Stage3.Text = "Stage 3";
            this.Stage3.UseVisualStyleBackColor = true;
            this.Stage3.Click += new System.EventHandler(this.Stage3_Click);
            // 
            // Stage4
            // 
            this.Stage4.Location = new System.Drawing.Point(95, 152);
            this.Stage4.Name = "Stage4";
            this.Stage4.Size = new System.Drawing.Size(75, 23);
            this.Stage4.TabIndex = 4;
            this.Stage4.Text = "Stage 4";
            this.Stage4.UseVisualStyleBackColor = true;
            this.Stage4.Click += new System.EventHandler(this.Stage4_Click);
            // 
            // Stage5
            // 
            this.Stage5.Location = new System.Drawing.Point(95, 181);
            this.Stage5.Name = "Stage5";
            this.Stage5.Size = new System.Drawing.Size(75, 23);
            this.Stage5.TabIndex = 5;
            this.Stage5.Text = "Stage 5";
            this.Stage5.UseVisualStyleBackColor = true;
            this.Stage5.Click += new System.EventHandler(this.Stage5_Click);
            // 
            // Stage6
            // 
            this.Stage6.Location = new System.Drawing.Point(95, 210);
            this.Stage6.Name = "Stage6";
            this.Stage6.Size = new System.Drawing.Size(75, 23);
            this.Stage6.TabIndex = 6;
            this.Stage6.Text = "Stage 6";
            this.Stage6.UseVisualStyleBackColor = true;
            this.Stage6.Click += new System.EventHandler(this.Stage6_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 246);
            this.Controls.Add(this.Stage6);
            this.Controls.Add(this.Stage5);
            this.Controls.Add(this.Stage4);
            this.Controls.Add(this.Stage3);
            this.Controls.Add(this.Stage2);
            this.Controls.Add(this.Stage1);
            this.Controls.Add(this.ButtonConvert);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonConvert;
        private System.Windows.Forms.Button Stage1;
        private System.Windows.Forms.Button Stage2;
        private System.Windows.Forms.Button Stage3;
        private System.Windows.Forms.Button Stage4;
        private System.Windows.Forms.Button Stage5;
        private System.Windows.Forms.Button Stage6;
    }
}

