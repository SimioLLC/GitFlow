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
                MessageBox.Show(this,
                    "Please select a branch from the dropdown list.",
                    "No Branch Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                // Clean up the branch name
                if (_branchName.StartsWith("origin/"))
                    _branchName = _branchName.Substring(7);
                if (_branchName.EndsWith(" (current)"))
                    _branchName = _branchName.Substring(0, _branchName.Length - " (current)".Length);

                // Confirm deletion -- this is destructive
                DialogResult confirm = MessageBox.Show(this,
                    $"Are you sure you want to delete the branch '{_branchName}'?\n\n" +
                    "This will remove the branch both locally and from the remote.\n" +
                    "Any uncommitted changes on that branch will be lost.\n\n" +
                    "This action cannot be undone.",
                    "Confirm Delete Branch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                string currentBranch = LibgitFunctionClass.git_current_branch(GitContext.Instance.RepositoryPath);

                if (currentBranch == _branchName)
                {
                    // Need to switch to main first
                    MessageBox.Show(this,
                        $"You are currently on the '{_branchName}' branch.\n\n" +
                        "You will be switched to 'main' before the branch is deleted.\n" +
                        "Your project will reload.",
                        "Switching to Main", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LibgitFunctionClass.git_checkout_branch(GitContext.Instance.RepositoryPath, "main");
                    LibgitFunctionClass.git_delete_branch(GitContext.Instance.RepositoryPath, _branchName);

                    MessageBox.Show(this,
                        $"Branch '{_branchName}' has been deleted.\n\n" +
                        "You are now on the 'main' branch.\n" +
                        "Your project will reload.",
                        "Branch Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                    SystemDirectoryHandler.Refresh();
                }
                else
                {
                    LibgitFunctionClass.git_delete_branch(GitContext.Instance.RepositoryPath, _branchName);

                    MessageBox.Show(this,
                        $"Branch '{_branchName}' has been deleted.\n\n" +
                        $"You are still on the '{currentBranch}' branch.",
                        "Branch Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
            }
            catch (Exception ex) when (ex.Message.Contains("conflicts prevent checkout"))
            {
                MessageBox.Show(this,
                    "Cannot delete this branch because you have unsaved changes.\n\n" +
                    "Switching to 'main' requires a clean workspace.\n" +
                    "Please save your Simio project and use 'Commit & Push' first,\n" +
                    "then try deleting the branch again.",
                    "Unsaved Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
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
