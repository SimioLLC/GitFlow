namespace GitFlow
{
    partial class CommitForm
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
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Commit Message";
            textBox1.Size = new Size(424, 172);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new Point(388, 178);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new Size(48, 27);
            simpleButton1.TabIndex = 1;
            simpleButton1.Text = "Ok";
            simpleButton1.Click += simpleButton1_Click;
            // 
            // simpleButton2
            // 
            simpleButton2.Location = new Point(334, 178);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new Size(48, 27);
            simpleButton2.TabIndex = 2;
            simpleButton2.Text = "Cancel";
            simpleButton2.Click += simpleButton2_Click;
            // 
            // CommitForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(448, 217);
            Controls.Add(simpleButton2);
            Controls.Add(simpleButton1);
            Controls.Add(textBox1);
            IconOptions.Image = Properties.Resources.Icon1;
            Name = "CommitForm";
            Text = "Commit Message";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
    }
}