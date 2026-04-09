using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LibGit2Sharp;

namespace GitFlow
{
    public enum ConnectMode { OpenExisting, InitNew, Clone }

    public partial class ConnectForm : Form
    {
        private ConnectMode _mode = ConnectMode.Clone;
        private string _detectedRepoPath;

        // Form controls
        private Label lblStatus;
        private Panel pnlModeSelection;
        private RadioButton rbInit;
        private RadioButton rbClone;
        private Label lblRemoteUrl;
        private TextBox txtRemoteUrl;
        private Label lblLocalPath;
        private TextBox txtLocalPath;
        private Button btnBrowse;
        private Panel pnlAuth;
        private Label lblAuth;
        private Button btnSignInGitHub;
        private Label lblOrPat;
        private TextBox txtPat;
        private LinkLabel lnkCreateToken;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblEmail;
        private TextBox txtEmail;
        private Button btnOk;
        private Button btnCancel;

        public ConnectForm()
        {
            InitializeFormLayout();
            AutoDetect();
        }

        private void InitializeFormLayout()
        {
            this.Text = "Connect to Repository";
            this.Size = new Size(520, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 15;
            int leftMargin = 20;
            int fieldWidth = 460;

            // Status label (shows auto-detection result)
            lblStatus = new Label
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(0, 120, 0)
            };
            y += 45;

            // Mode selection panel (only shown when no repo detected)
            pnlModeSelection = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 30),
                Visible = false
            };
            rbInit = new RadioButton
            {
                Text = "Create new repository for this project",
                Location = new Point(0, 5),
                Size = new Size(230, 20),
                Checked = true
            };
            rbInit.CheckedChanged += (s, e) => { if (rbInit.Checked) SetMode(ConnectMode.InitNew); };
            rbClone = new RadioButton
            {
                Text = "Clone an existing repository",
                Location = new Point(235, 5),
                Size = new Size(220, 20)
            };
            rbClone.CheckedChanged += (s, e) => { if (rbClone.Checked) SetMode(ConnectMode.Clone); };
            pnlModeSelection.Controls.AddRange(new Control[] { rbInit, rbClone });
            y += 35;

            // Remote URL
            lblRemoteUrl = new Label { Text = "Remote URL:", Location = new Point(leftMargin, y), Size = new Size(fieldWidth, 18) };
            y += 20;
            txtRemoteUrl = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 23),
                PlaceholderText = "https://github.com/user/repo.git"
            };
            txtRemoteUrl.TextChanged += (s, e) => UpdateAuthUI();
            y += 30;

            // Local Path
            lblLocalPath = new Label { Text = "Local Folder:", Location = new Point(leftMargin, y), Size = new Size(fieldWidth, 18) };
            y += 20;
            txtLocalPath = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth - 40, 23)
            };
            btnBrowse = new Button
            {
                Text = "...",
                Location = new Point(leftMargin + fieldWidth - 35, y),
                Size = new Size(35, 23)
            };
            btnBrowse.Click += (s, e) =>
            {
                var path = SystemDirectoryHandler.PromptForFolder(txtLocalPath.Text);
                if (!string.IsNullOrEmpty(path))
                    txtLocalPath.Text = path;
            };
            y += 35;

            // Authentication section
            pnlAuth = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 130)
            };

            int ay = 0;
            lblAuth = new Label
            {
                Text = "Authentication:",
                Location = new Point(0, ay),
                Size = new Size(fieldWidth, 18),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            ay += 22;

            btnSignInGitHub = new Button
            {
                Text = "Sign in with GitHub",
                Location = new Point(0, ay),
                Size = new Size(180, 30),
                Font = new Font("Segoe UI", 9)
            };
            btnSignInGitHub.Click += BtnSignInGitHub_Click;
            ay += 38;

            lblOrPat = new Label
            {
                Text = "Or enter a Personal Access Token:",
                Location = new Point(0, ay),
                Size = new Size(250, 18),
                ForeColor = Color.Gray
            };
            lnkCreateToken = new LinkLabel
            {
                Text = "Create token",
                Location = new Point(255, ay),
                Size = new Size(100, 18)
            };
            lnkCreateToken.LinkClicked += (s, e) =>
            {
                var host = GitHostDetector.DetectHost(txtRemoteUrl.Text);
                var url = GitHostDetector.GetTokenCreationUrl(host);
                if (!string.IsNullOrEmpty(url))
                {
                    try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
                    catch { }
                }
            };
            ay += 22;

            txtPat = new TextBox
            {
                Location = new Point(0, ay),
                Size = new Size(fieldWidth, 23),
                PlaceholderText = "Personal Access Token",
                UseSystemPasswordChar = true
            };
            ay += 30;

            pnlAuth.Controls.AddRange(new Control[] { lblAuth, btnSignInGitHub, lblOrPat, lnkCreateToken, txtPat });
            y += 135;

            // Username
            lblUsername = new Label { Text = "Username (optional):", Location = new Point(leftMargin, y), Size = new Size(fieldWidth, 18) };
            y += 20;
            txtUsername = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 23),
                PlaceholderText = "Your name for commit history"
            };
            y += 30;

            // Email
            lblEmail = new Label { Text = "Email (optional):", Location = new Point(leftMargin, y), Size = new Size(fieldWidth, 18) };
            y += 20;
            txtEmail = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 23),
                PlaceholderText = "Your email for commit history"
            };
            y += 35;

            // Buttons
            btnOk = new Button
            {
                Text = "Connect",
                Location = new Point(leftMargin + fieldWidth - 180, y),
                Size = new Size(85, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + fieldWidth - 85, y),
                Size = new Size(85, 30)
            };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblStatus, pnlModeSelection,
                lblRemoteUrl, txtRemoteUrl,
                lblLocalPath, txtLocalPath, btnBrowse,
                pnlAuth,
                lblUsername, txtUsername,
                lblEmail, txtEmail,
                btnOk, btnCancel
            });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private void AutoDetect()
        {
            try
            {
                // Try to get the active project path from Simio context
                string projectPath = null;
                if (GitContext.Instance.simioContext?.ActiveProject != null)
                {
                    projectPath = SystemDirectoryHandler.GetStringProperty(
                        GitContext.Instance.simioContext.ActiveProject, "FileName");
                }

                if (!string.IsNullOrEmpty(projectPath))
                {
                    string projectDir = Path.GetDirectoryName(projectPath);

                    // Walk up looking for an existing .git folder
                    _detectedRepoPath = LibgitFunctionClass.FindRepoRoot(projectDir);

                    if (_detectedRepoPath != null)
                    {
                        // Existing repo found -- Open mode
                        _mode = ConnectMode.OpenExisting;
                        txtLocalPath.Text = _detectedRepoPath;
                        txtLocalPath.ReadOnly = true;

                        string remoteUrl = LibgitFunctionClass.ReadRemoteUrl(_detectedRepoPath);
                        if (!string.IsNullOrEmpty(remoteUrl))
                        {
                            txtRemoteUrl.Text = remoteUrl;
                            txtRemoteUrl.ReadOnly = true;
                        }

                        // Try to load stored credentials
                        var cred = CredentialHandler.ReadCredential(_detectedRepoPath);
                        if (cred != null)
                        {
                            txtPat.Text = cred.Password ?? "";
                            txtUsername.Text = cred.UserName ?? "";
                            txtEmail.Text = cred.Comment ?? "";
                        }

                        lblStatus.Text = $"Found existing repository at:\n{_detectedRepoPath}";
                        pnlModeSelection.Visible = false;
                    }
                    else
                    {
                        // No repo found, but project exists -- show Init/Clone options
                        _mode = ConnectMode.InitNew;
                        txtLocalPath.Text = projectDir;
                        lblStatus.Text = "No Git repository detected for this project.\nChoose how to set one up:";
                        pnlModeSelection.Visible = true;
                    }
                }
                else
                {
                    // No active project -- default to Clone
                    _mode = ConnectMode.Clone;
                    lblStatus.Text = "No active project detected. Clone or connect to a repository:";
                    pnlModeSelection.Visible = false;
                }
            }
            catch
            {
                _mode = ConnectMode.Clone;
                lblStatus.Text = "Enter repository details to connect:";
            }

            UpdateAuthUI();
        }

        private void SetMode(ConnectMode mode)
        {
            _mode = mode;
            if (mode == ConnectMode.Clone)
            {
                txtRemoteUrl.ReadOnly = false;
                txtLocalPath.ReadOnly = false;
                txtRemoteUrl.PlaceholderText = "https://github.com/user/repo.git";
            }
            else if (mode == ConnectMode.InitNew)
            {
                txtRemoteUrl.ReadOnly = false;
                txtLocalPath.ReadOnly = false;
                txtRemoteUrl.PlaceholderText = "https://github.com/user/repo.git (remote to push to)";
            }
        }

        private void UpdateAuthUI()
        {
            var host = GitHostDetector.DetectHost(txtRemoteUrl.Text);

            if (host == GitHost.GitHub)
            {
                btnSignInGitHub.Visible = true;
                btnSignInGitHub.Text = "Sign in with GitHub";
                lblOrPat.Text = "Or enter a Personal Access Token:";
            }
            else
            {
                btnSignInGitHub.Visible = false;
                string hostName = GitHostDetector.GetHostDisplayName(host);
                lblOrPat.Text = $"Enter a Personal Access Token for {hostName}:";
            }

            var tokenUrl = GitHostDetector.GetTokenCreationUrl(host);
            lnkCreateToken.Visible = !string.IsNullOrEmpty(tokenUrl);
        }

        private async void BtnSignInGitHub_Click(object sender, EventArgs e)
        {
            try
            {
                btnSignInGitHub.Enabled = false;
                btnSignInGitHub.Text = "Signing in...";

                string token = await OAuthDeviceFlowHandler.AuthenticateGitHub();
                txtPat.Text = token;

                MessageBox.Show("Successfully signed in with GitHub!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                // User cancelled, do nothing
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sign-in Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSignInGitHub.Enabled = true;
                btnSignInGitHub.Text = "Sign in with GitHub";
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                string repoPath = txtLocalPath.Text.Trim();
                string remoteUrl = txtRemoteUrl.Text.Trim();
                string pat = txtPat.Text;
                string username = string.IsNullOrWhiteSpace(txtUsername.Text) ? "DefaultUser" : txtUsername.Text.Trim();
                string email = string.IsNullOrWhiteSpace(txtEmail.Text) ? "DefaultUser@email.com" : txtEmail.Text.Trim();

                if (string.IsNullOrEmpty(repoPath))
                {
                    MessageBox.Show("Please select a local folder.", "Required Field",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Save/update credentials
                if (!string.IsNullOrEmpty(pat))
                {
                    var existingCred = CredentialHandler.ReadCredential(repoPath);
                    if (existingCred == null)
                        CredentialHandler.SaveCredential(repoPath, username, pat, email,
                            Meziantou.Framework.Win32.CredentialPersistence.Session);
                    else
                        CredentialHandler.UpdateCredential(repoPath, username, pat, email);
                }

                // Initialize GitContext
                GitContext.Instance.Initialize(repoPath, remoteUrl, pat, username, email);

                // Execute the appropriate operation
                switch (_mode)
                {
                    case ConnectMode.OpenExisting:
                        ExecuteOpen(repoPath);
                        break;
                    case ConnectMode.InitNew:
                        ExecuteInit(repoPath, remoteUrl);
                        break;
                    case ConnectMode.Clone:
                        ExecuteClone(repoPath, remoteUrl);
                        break;
                }

                // Check permissions
                int permissionLevel = LibgitFunctionClass.GetPermission(repoPath);
                GitContext.Instance.PermissionLevel = permissionLevel;

                string permStr = permissionLevel == 2 ? "Read/Write" :
                                 permissionLevel == 1 ? "Read Only" : "No Access";

                if (permissionLevel == 0)
                {
                    MessageBox.Show("Could not verify permissions. Please check your credentials.",
                        "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show($"Connected successfully!\nPermission: {permStr}",
                    "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteOpen(string repoPath)
        {
            string gitDir = Path.Combine(repoPath, ".git");
            if (!Directory.Exists(gitDir))
                throw new Exception($"No Git repository found at: {repoPath}");
        }

        private void ExecuteInit(string repoPath, string remoteUrl)
        {
            if (string.IsNullOrEmpty(remoteUrl))
                throw new Exception("Remote URL is required to initialize a repository.");

            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string dllDirectory = Path.GetDirectoryName(assemblyLocation);
            string gitIgnorePath = Path.Combine(dllDirectory, ".gitignore");

            LibgitFunctionClass.git_init(repoPath, remoteUrl,
                GitContext.Instance.GetSignature(), gitIgnorePath);
        }

        private void ExecuteClone(string repoPath, string remoteUrl)
        {
            if (string.IsNullOrEmpty(remoteUrl))
                throw new Exception("Remote URL is required to clone a repository.");

            LibgitFunctionClass.git_clone(repoPath, remoteUrl);
        }
    }
}
