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
                    MessageBox.Show(this,
                        "Please enter a commit message.\n\n" +
                        "This is a short description of what you changed\n" +
                        "(e.g. 'Updated server processing time' or 'Added new source').",
                        "Message Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Check if there are actually changes to commit
                if (!LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                {
                    MessageBox.Show(this,
                        "There are no changes to save.\n\n" +
                        "Your model matches the last saved version.\n" +
                        "Make some changes in Simio first, then come back here.",
                        "Nothing to Commit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Confirm before committing
                string currentBranch = "unknown";
                try { currentBranch = LibgitFunctionClass.git_current_branch(GitContext.Instance.RepositoryPath); }
                catch { }

                DialogResult confirm = MessageBox.Show(this,
                    $"Save and push your changes?\n\n" +
                    $"Branch: {currentBranch}\n" +
                    $"Message: \"{_commitMessage}\"\n\n" +
                    "This will save your changes and share them with your team.",
                    "Confirm Commit & Push", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (confirm != DialogResult.OK) return;

                LibgitFunctionClass.git_commit(GitContext.Instance.RepositoryPath, _commitMessage, GitContext.Instance.GetSignature());

                try
                {
                    LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                    MessageBox.Show(this,
                        $"Changes saved and pushed successfully!\n\n" +
                        $"Branch: {currentBranch}\n" +
                        $"Message: \"{_commitMessage}\"\n\n" +
                        "Your team can now pull these changes.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception pushEx)
                {
                    // Commit succeeded but push failed -- let the user know their work is saved locally
                    MessageBox.Show(this,
                        "Your changes were saved locally, but could not be pushed to the remote.\n\n" +
                        $"Reason: {pushEx.Message}\n\n" +
                        "Your work is safe. Try 'Commit & Push' again later,\n" +
                        "or use 'Pull' first to get the latest changes from your team.",
                        "Saved Locally (Push Failed)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
