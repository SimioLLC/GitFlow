using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DirectX.Common.Direct2D;
using DevExpress.XtraEditors;

namespace GitFlow
{
    public partial class OpenRepoForm : DevExpress.XtraEditors.XtraForm
    {
        public OpenRepoForm()
        {
            InitializeComponent();
        }

        //vars to hold objects
        

        private string _pat = "";
        private string _remoteURL = "";
        private string _RepoPath = "";
        private string _username = "";
        private string _email = "";




        private void OpenRepoForm_Load(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _email = textBox1.Text;
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
                if (string.IsNullOrEmpty(_pat))
                {
                    throw new Exception(Resources.Resource1.FieldFillPAT);
                }

                //check if _RepoPath ends with .git, if so remove it
                if (_RepoPath.EndsWith(".git"))
                {
                    _RepoPath = _RepoPath.Substring(0, _RepoPath.Length - "/.git".Length);// really it is a reverse slash but I am only using it for length so it doesn't really matter
                }

                string gitDir = Path.Combine(_RepoPath, ".git");
                if (!Directory.Exists(gitDir))
                {
                    throw new Exception(Resources.Resource1.GitOpenNoGitDetected + " " + _RepoPath);

                }
                // Attempt to set the Git context

                GitContext.Instance.Initialize(_RepoPath, _remoteURL, _pat, _username, _email);
                
                //have to talk to higherups but unsure how to open since there is no folder opening in simio and one repo may have multiple projects or even multiple folders of projects
                MessageBox.Show(Resources.Resource1.RepoOpenedSuccess);
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
    }
}