using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace GitFlow
{
    public partial class CommitForm : DevExpress.XtraEditors.XtraForm
    {
        public CommitForm()
        {
            InitializeComponent();
        }

        private string _commitMessage = string.Empty;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _commitMessage = textBox1.Text;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try 
            { 
                if (string.IsNullOrWhiteSpace(_commitMessage))
                {
                    throw new Exception("Commit message cannot be empty.");
                }
                LibgitFunctionClass.git_commit(GitContext.Instance.RepositoryPath, _commitMessage, GitContext.Instance.GetSignature());
                LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                MessageBox.Show("Commit and Push Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}