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
using SimioAPI.Extensions;

namespace GitFlow
{
    public partial class BranchSelectForm : DevExpress.XtraEditors.XtraForm
    {
        public BranchSelectForm()
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
                string currentBranch = LibgitFunctionClass.git_current_branch(GitContext.Instance.RepositoryPath);

                if (_branchName == currentBranch)
                {
                    MessageBox.Show(this,
                        $"You are already on the '{_branchName}' branch.",
                        "Already on Branch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Warn about uncommitted changes
                if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                {
                    DialogResult saveFirst = MessageBox.Show(this,
                        "You have unsaved changes in your current model.\n\n" +
                        "Switching branches will discard these changes.\n" +
                        "Would you like to switch anyway?\n\n" +
                        "Tip: Use 'Commit & Push' first to save your work.",
                        "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (saveFirst != DialogResult.Yes) return;
                }

                // Confirm the switch
                DialogResult confirm = MessageBox.Show(this,
                    $"Switch from '{currentBranch}' to '{_branchName}'?\n\n" +
                    "Your project will reload with the contents of that branch.",
                    "Switch Branch", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (confirm != DialogResult.OK) return;

                if (!LibgitFunctionClass.git_checkout_branch(GitContext.Instance.RepositoryPath, _branchName))
                {
                    MessageBox.Show(this,
                        $"Could not switch to branch '{_branchName}'.\n\n" +
                        "The branch may have been deleted from the remote.\n" +
                        "Try using 'Pull' first to update your branch list.",
                        "Branch Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(this,
                    $"Switched to branch '{_branchName}'.\n\n" +
                    "Your project will now reload to show the branch contents.",
                    "Branch Switched", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SystemDirectoryHandler.Refresh();
                this.Close();
            }
            catch (Exception ex) when (ex.Message.Contains("conflicts prevent checkout"))
            {
                MessageBox.Show(this,
                    "Cannot switch branches because you have unsaved changes.\n\n" +
                    "Please save your Simio project and use 'Commit & Push' first,\n" +
                    "then try switching branches again.",
                    "Unsaved Changes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BranchSelectForm_Load_1(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
