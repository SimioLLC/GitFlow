using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.CodeParser;
using DevExpress.DirectX.Common.Direct2D;
using DevExpress.Pdf.Xmp;
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
                // defaultize other strings to defaults instead of empty strings
                if (string.IsNullOrEmpty(_username))
                {
                    _username = "defaultuser";
                }
                if (string.IsNullOrEmpty(_email))
                {
                    _email = "defaultuser@email.com";
                }

                // Attempt to set the Git context

                if (string.IsNullOrEmpty(_pat))
                {
                    var credential = CredentialHandler.ReadCredential(_RepoPath);
                    if (credential == null)
                    {
                        throw new Exception(Resources.Resource1.ErrorNoPermissions);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(credential.Password) || string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Comment))
                        {
                            throw new Exception(Resources.Resource1.ErrorNoPermissions);
                        }
                        else
                        {
                            _pat = credential.Password; // If no PAT is provided, try to read it from the stored credentials
                            _username = credential.UserName; // If no username is provided, try to read it from the stored credentials
                            _email = credential.Comment; // If no email is provided, try to read it from the stored credential

                        }

                    }


                }
                else//either save or update creds depending if there already exists creds
                {
                    var credential = CredentialHandler.ReadCredential(_RepoPath);
                    if (credential == null)
                    {
                        CredentialHandler.SaveCredential(_RepoPath, _username, _pat, _email, Meziantou.Framework.Win32.CredentialPersistence.LocalMachine);
                    }
                    else
                    {
                        CredentialHandler.UpdateCredential(_RepoPath, _username, _pat, _email);
                    }
                        
                }


                    GitContext.Instance.Initialize(_RepoPath, _remoteURL, _pat, _username, _email);

                int permissionLevel = LibgitFunctionClass.GetPermission(_RepoPath);
                // Show the permission level in a message box
                string permissionLevelString = "Unknown";
                if (permissionLevel == 0)
                {
                    permissionLevelString = "No Access";
                    throw new Exception(Resources.Resource1.ErrorNoPermissions);
                }
                else if (permissionLevel == 1)
                {
                    permissionLevelString = "Read Only";
                }
                else if (permissionLevel == 2)
                {
                    permissionLevelString = "Read/Write";
                }

                
                GitContext.Instance.PermissionLevel = permissionLevel; // Set the permission level in the GitContext

                // Show the permission level in a message box
                MessageBox.Show(this, "Permission: "+ permissionLevelString, "Permission Level", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MessageBox.Show(Resources.Resource1.RepoOpenedSuccess);
                // If successful, close the form
                this.Close();
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            _username = textBox4.Text;
        }
    }
}