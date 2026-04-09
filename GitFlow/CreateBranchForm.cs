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
    public partial class CreateBranchForm : DevExpress.XtraEditors.XtraForm
    {
        public CreateBranchForm()
        {
            InitializeComponent();
        }

        private string _brachName = string.Empty;

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_brachName))
                {
                    MessageBox.Show(this,
                        "Please enter a name for your new branch.\n\n" +
                        "A branch is like a separate workspace where you can make\n" +
                        "changes without affecting the main version of your model.",
                        "Branch Name Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Show current branch info and confirm
                string currentBranch = LibgitFunctionClass.git_current_branch(GitContext.Instance.RepositoryPath);
                DialogResult confirm = MessageBox.Show(this,
                    $"This will create a new branch called '{_brachName}'\n" +
                    $"based on your current branch '{currentBranch}'.\n\n" +
                    "Your work will continue on the new branch.\n\n" +
                    "Continue?",
                    "Create Branch", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (confirm != DialogResult.OK) return;

                LibgitFunctionClass.git_create_branch(GitContext.Instance.RepositoryPath, _brachName, GitContext.Instance.GetSignature());

                MessageBox.Show(this,
                    $"Branch '{_brachName}' created successfully!\n\n" +
                    "You are now working on this branch.\n" +
                    "Any changes you save and commit will go to this branch,\n" +
                    "keeping your main branch safe.\n\n" +
                    "When you're ready, use 'Commit & Push' to save your work.",
                    "Branch Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex) when (ex.Message.Contains("already exists"))
            {
                MessageBox.Show(this,
                    $"A branch named '{_brachName}' already exists.\n\n" +
                    "Please choose a different name, or use 'Select Branch'\n" +
                    "to switch to the existing branch.",
                    "Branch Already Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) when (ex.Message.Contains("the given reference name"))
            {
                MessageBox.Show(this,
                    "That branch name contains invalid characters.\n\n" +
                    "Branch names cannot contain spaces or most special characters.\n" +
                    "Use letters, numbers, hyphens (-) and underscores (_) only.\n\n" +
                    "Example: my-feature or bugfix_v2",
                    "Invalid Branch Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _brachName = textBox1.Text;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateBranchForm_Load(object sender, EventArgs e)
        {

        }
    }
}
