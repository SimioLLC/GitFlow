namespace GitFlow
{
    partial class GitNewForm
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
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            textBox3 = new TextBox();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(27, 41);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "URL (Remote Repository)";
            textBox1.Size = new Size(436, 23);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(27, 131);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Path (Local Repository)";
            textBox2.Size = new Size(436, 23);
            textBox2.TabIndex = 1;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new Point(466, 131);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new Size(27, 25);
            simpleButton1.TabIndex = 2;
            simpleButton1.Text = "...";
            simpleButton1.Click += simpleButton1_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(27, 89);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "PAT (Personal Access Token)";
            textBox3.Size = new Size(436, 23);
            textBox3.TabIndex = 3;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // simpleButton2
            // 
            simpleButton2.Location = new Point(433, 253);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new Size(60, 26);
            simpleButton2.TabIndex = 4;
            simpleButton2.Text = "Cancel";
            simpleButton2.Click += simpleButton2_Click;
            // 
            // simpleButton3
            // 
            simpleButton3.Location = new Point(367, 253);
            simpleButton3.Name = "simpleButton3";
            simpleButton3.Size = new Size(60, 26);
            simpleButton3.TabIndex = 5;
            simpleButton3.Text = "Ok";
            simpleButton3.Click += simpleButton3_Click;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(27, 169);
            textBox4.Name = "textBox4";
            textBox4.PlaceholderText = "Username (optional)";
            textBox4.Size = new Size(436, 23);
            textBox4.TabIndex = 6;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(27, 210);
            textBox5.Name = "textBox5";
            textBox5.PlaceholderText = "Email (optional)";
            textBox5.Size = new Size(436, 23);
            textBox5.TabIndex = 7;
            textBox5.TextChanged += textBox5_TextChanged;
            // 
            // GitNewForm
            // 
            Appearance.BackColor = Color.GhostWhite;
            Appearance.Options.UseBackColor = true;
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 291);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(simpleButton3);
            Controls.Add(simpleButton2);
            Controls.Add(textBox3);
            Controls.Add(simpleButton1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            IconOptions.Image = Properties.Resources.Icon1;
            Name = "GitNewForm";
            Text = "Initialize Repository";
            Load += GitNewForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox textBox1;
        private TextBox textBox2;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private TextBox textBox3;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private TextBox textBox4;
        private TextBox textBox5;
    }
}