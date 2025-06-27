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
using System.Reflection;
using System.IO;

namespace GitFlow
{
    public partial class GitNewForm : DevExpress.XtraEditors.XtraForm
    {

        //vars to hold objects
        private GitContext context;

        private string _pat = "";
        private string _remoteURL = "";
        private string _RepoPath = "";
        private string _username = "";
        private string _email = "";


        public GitNewForm()
        {
            InitializeComponent();
            var context = GitContext.Instance;
        }

        private void GitNewForm_Load(object sender, EventArgs e)
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

                //add credential
                // Attempt to set the Git context
                if (string.IsNullOrEmpty(_username))
                {
                    _username = "DefaultUser"; // Set a default username if not provided
                }
                if (string.IsNullOrEmpty(_email))
                {
                    _email = "DefaultUser@email.com"; // Set a default email if not provided
                }


                var cred = CredentialHandler.ReadCredential(_RepoPath);
                if (cred == null)
                {
                    CredentialHandler.SaveCredential(_RepoPath, _username, _pat, _email, Meziantou.Framework.Win32.CredentialPersistence.LocalMachine);
                }
                else
                {
                    CredentialHandler.UpdateCredential(_RepoPath, _username, _pat, _email);
                }



                // Attempt to set the Git context

                GitContext.Instance.Initialize(_RepoPath, _remoteURL, _pat, _username, _email);


                string assemblyLocation = Assembly.GetExecutingAssembly().Location;

                // Get the directory containing the DLL.
                string dllDirectory = Path.GetDirectoryName(assemblyLocation);

                // Combine the directory with your file name to get the full path.
                string myGitIgnorePath = Path.Combine(dllDirectory, ".gitignore");

                LibgitFunctionClass.git_init(GitContext.Instance.RepositoryPath, GitContext.Instance.RemoteUrl, GitContext.Instance.GetSignature(), myGitIgnorePath);

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
                MessageBox.Show(this, "Permission: " + permissionLevelString, "Permission Level", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MessageBox.Show("Success Initialized Repository");
                // If successful, close the form
                this.Close();
            }
            catch (Exception ex) when (ex.Message.Contains("contains commits that are not present locally"))
            {
                var resp = MessageBox.Show(this, Resources.Resource1.InitWithDetectedFilesPopUp, "Detected Files on Remote Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (resp == DialogResult.Yes)
                {
                    var resp2 = MessageBox.Show(this, Resources.Resource1.InitWithDetectedFilesPopUp2, "Override Remote Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (resp2 == DialogResult.Yes)
                    {
                        //if dirty
                        if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                        {
                            LibgitFunctionClass.git_commit(GitContext.Instance.RepositoryPath, "Init Override", GitContext.Instance.GetSignature());
                        }
                        else
                        {
                            // Force push to override the remote repository
                            LibgitFunctionClass.git_push_force(GitContext.Instance.RepositoryPath);
                        }
                        LibgitFunctionClass.git_push_force(GitContext.Instance.RepositoryPath);
                        string masterBranchFile = Path.Combine(GitContext.Instance.RepositoryPath, ".git", "refs", "heads", "master");
                        if (File.Exists(masterBranchFile))
                        {
                            File.Delete(masterBranchFile);
                        }

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
                        MessageBox.Show(this, "Permission: " + permissionLevelString, "Permission Level", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        MessageBox.Show(Resources.Resource1.InitAndOverride, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.Close();
                    return;
                }
                else
                {
                    // Do not close the form, allow user to make changes
                    string masterBranchFile = Path.Combine(GitContext.Instance.RepositoryPath, ".git", "refs", "heads", "master");
                    if (File.Exists(masterBranchFile))
                    {
                        File.Delete(masterBranchFile);
                    }
                    return;

                }
            }
            catch (Exception ex) when (ex.Message.Contains("403"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

            }
            catch (Exception ex) when (ex.Message.Contains("404"))
            {
                throw new Exception(Resources.Resource1.RemoteRetrival);
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

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            _email = textBox5.Text;
        }
    }
}