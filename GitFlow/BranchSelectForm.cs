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
                MessageBox.Show("Please select a branch.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                // Attempt to checkout the selected branch
                LibgitFunctionClass.git_checkout_branch(GitContext.Instance.RepositoryPath, _branchName);
                MessageBox.Show("Switched to branch: " + _branchName, "Success" + "\nPlease reopen your project to show contents of the new branch.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex) when (ex.Message.Contains("conflicts prevent checkout"))
            {
                // Handle the case where the branch does not exist
                MessageBox.Show(this, Resources.Resource1.CheckoutWithUncommitedChanges, "Branch Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void BranchSelectForm_Load_1(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}