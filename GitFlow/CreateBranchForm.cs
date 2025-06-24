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
            //create branch
            try
            {
                if (string.IsNullOrWhiteSpace(_brachName))
                {
                    throw new Exception("Branch name cannot be empty.");
                }
                LibgitFunctionClass.git_create_branch(GitContext.Instance.RepositoryPath, _brachName, GitContext.Instance.GetSignature());
                MessageBox.Show("Branch Created Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                CommitForm commitForm = new CommitForm();
                commitForm.Show();


            }
            catch (Exception ex) when (ex.Message.Contains("the given reference name"))
            {
                // Handle the case where the branch already exists
                MessageBox.Show(this, Resources.Resource1.CreateNewBranchWithSpaceError, "Branch Naming Issue", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
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