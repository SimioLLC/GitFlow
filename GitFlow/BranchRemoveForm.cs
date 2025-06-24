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
    public partial class BranchRemoveForm : DevExpress.XtraEditors.XtraForm
    {
        public BranchRemoveForm()
        {
            InitializeComponent();
        }

        String _branchName = "";

        private void mruEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _branchName = mruEdit1.Text;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_branchName))
            {
                MessageBox.Show("Please select a branch to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {

                //if _branchName starts with origin/... get rid of origin/
                if (_branchName.StartsWith("origin/"))
                {
                    _branchName = _branchName.Substring(7);
                }
                //FOR TESTING PURPOSES ONLY
                //send a message box the current branch is being deleted
                //MessageBox.Show(LibgitFunctionClass.git_current_branch(GitContext.Instance.RepositoryPath), "info", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //send message box for what _branch is being deleted
                //MessageBox.Show(_branchName, "Info", MessageBoxButtons.OK, MessageBoxIcon.Question);


                //check if on the branch to be deleted
                if (LibgitFunctionClass.git_current_branch(GitContext.Instance.RepositoryPath) == _branchName)
                {
                    LibgitFunctionClass.git_checkout_branch(GitContext.Instance.RepositoryPath, "main");
                    LibgitFunctionClass.git_delete_branch(GitContext.Instance.RepositoryPath, _branchName);

                    MessageBox.Show("deleted branch: " + _branchName + "\n" + Resources.Resource1.DeleteNowOnMainMessage, "Success" , MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    // Attempt to delete the selected branch
                    LibgitFunctionClass.git_delete_branch(GitContext.Instance.RepositoryPath, _branchName);

                    MessageBox.Show("deleted branch: " + _branchName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }

            }
            catch (Exception ex) when (ex.Message.Contains("conflicts prevent checkout"))
            {
                // Handle the case where the branch does not exist
                MessageBox.Show(this, Resources.Resource1.CheckoutWithUncommitedChanges, "Branch Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BranchRemoveForm_Load_1(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}