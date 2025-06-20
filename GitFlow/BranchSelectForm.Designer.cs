using DevExpress.XtraGauges.Presets.PresetManager;




namespace GitFlow
{
    partial class BranchSelectForm
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
            mruEdit1 = new DevExpress.XtraEditors.MRUEdit();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)mruEdit1.Properties).BeginInit();
            SuspendLayout();
            // 
            // mruEdit1
            // 
            mruEdit1.Location = new Point(28, 12);
            mruEdit1.Name = "mruEdit1";
            mruEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            mruEdit1.Size = new Size(336, 22);
            mruEdit1.TabIndex = 0;
            mruEdit1.SelectedIndexChanged += mruEdit1_SelectedIndexChanged;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new Point(88, 50);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new Size(99, 32);
            simpleButton1.TabIndex = 1;
            simpleButton1.Text = "Select Branch";
            simpleButton1.Click += simpleButton1_Click;
            // 
            // simpleButton2
            // 
            simpleButton2.Location = new Point(193, 50);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new Size(99, 32);
            simpleButton2.TabIndex = 2;
            simpleButton2.Text = "Cancel";
            simpleButton2.Click += simpleButton2_Click;
            // 
            // BranchSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(382, 94);
            Controls.Add(simpleButton2);
            Controls.Add(simpleButton1);
            Controls.Add(mruEdit1);
            IconOptions.Image = Properties.Resources.Icon1;
            Name = "BranchSelectForm";
            Text = "Select Branch";
            Load += BranchSelectForm_Load_1;
            ((System.ComponentModel.ISupportInitialize)mruEdit1.Properties).EndInit();
            ResumeLayout(false);


            List<string> branches = LibgitFunctionClass.git_get_branches(GitContext.Instance.RepositoryPath);
            mruEdit1.Properties.Items.AddRange(branches.ToArray<string>());

        }

        private void BranchSelectForm_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        private DevExpress.XtraEditors.MRUEdit mruEdit1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
    }
}