using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DirectX.Common.Direct2D;
using DevExpress.XtraEditors;

namespace GitFlow
{
    public partial class CloneRepoForm : DevExpress.XtraEditors.XtraForm
    {
        CloneRepoForm(GitContext gitContext)
        {
            InitializeComponent();
            GitContext = gitContext;
        }
        //vars to hold objects
        internal GitContext GitContext { get; set; }

        private string _pat = "";
        private string _remoteURL = "";
        private string _RepoPath = "";
        private string _username = "";
        private string _email = "";


        public CloneRepoForm()
        {
            InitializeComponent();
        }

        private void CloneRepoForm_Load(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _remoteURL = textBox1.Text;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            textBox2.Text = SystemDirectoryHandler.PromptForFolder("");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            _pat = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            _RepoPath = textBox2.Text;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                // Attempt to set the Git context
                if (string.IsNullOrEmpty(_RepoPath))
                {
                    throw new Exception(Resources.Resource1.FieldFillPath);
                }
                // Attempt to set the Git context
                if (string.IsNullOrEmpty(_remoteURL))
                {
                    throw new Exception(Resources.Resource1.FieldFillURL);
                }
                // Attempt to set the Git context
                if (string.IsNullOrEmpty(_pat))
                {
                    throw new Exception(Resources.Resource1.FieldFillPAT);
                }

                // Attempt to set the Git context
       
                GitContext.Instance.Initialize(_RepoPath, _remoteURL, _pat, _username, _email);


                LibgitFunctionClass.git_clone(_RepoPath, _remoteURL);
                //LibgitFunctionClass.git_init(GitContext.getRepositoryPath(), GitContext.getRemoteUrl(), GitContext.getSignature());
                MessageBox.Show(Resources.Resource1.CloneSuccess);
                // If successful, close the form
                this.Close();
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //create a new git context
            //GitContext.setGitContext(_RepoPath, _remoteURL, _pat, _username);
            //this.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            _username = textBox4.Text;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            _email = textBox5.Text;
        }
    }
}