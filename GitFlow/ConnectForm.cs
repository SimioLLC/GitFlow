using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private Label lblHeader;
        private Label lblStatus;
        private Label lblStep1;
        private Label lblLocalPath;
        private TextBox txtLocalPath;
        private Button btnBrowse;
        private Label lblDetectedInfo;
        private Panel pnlModeSelection;
        private RadioButton rbInit;
        private RadioButton rbClone;
        private Label lblStep2;
        private Label lblRemoteUrl;
        private TextBox txtRemoteUrl;
        private Label lblStep3;
        private Label lblAuthHelp;
        private Button btnSignInGitHub;
        private Label lblOrPat;
        private TextBox txtPat;
        private LinkLabel lnkCreateToken;
        private Label lblUserInfo;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private Button btnOk;
        private Button btnCancel;

        public ConnectForm()
        {
            InitializeFormLayout();
            // Run auto-detect after form is fully loaded, not during construction
            this.Load += (s, e) => AutoDetect();
        }

        private void InitializeFormLayout()
        {
            this.Text = "Connect to Repository";
            this.Size = new Size(540, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 12;
            int leftMargin = 20;
            int fieldWidth = 480;

            // Header
            lblHeader = new Label
            {
                Text = "Connect Your Simio Project to Git",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 24),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
            y += 28;

            // Status / detection message
            lblStatus = new Label
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 36),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(0, 120, 0)
            };
            y += 40;

            // ── STEP 1: Local folder ──
            lblStep1 = new Label
            {
                Text = "Step 1: Select your project folder",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 18),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            y += 20;

            lblLocalPath = new Label
            {
                Text = "Browse to the folder where your Simio project is (or will be):",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 16),
                Font = new Font("Segoe UI", 8.25F),
                ForeColor = Color.Gray
            };
            y += 18;

            txtLocalPath = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth - 40, 23)
            };
            txtLocalPath.TextChanged += TxtLocalPath_TextChanged;

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
            y += 28;

            // Detection info (shown after folder is selected and repo found)
            lblDetectedInfo = new Label
            {
                Location = new Point(leftMargin + 10, y),
                Size = new Size(fieldWidth - 10, 18),
                Font = new Font("Segoe UI", 8.25F, FontStyle.Italic),
                ForeColor = Color.FromArgb(0, 100, 180),
                Visible = false
            };
            y += 22;

            // Mode selection (only shown when folder has no repo)
            pnlModeSelection = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 28),
                Visible = false
            };
            rbInit = new RadioButton
            {
                Text = "Create a new repository here",
                Location = new Point(0, 4),
                Size = new Size(220, 20),
                Checked = true
            };
            rbInit.CheckedChanged += (s, e) => { if (rbInit.Checked) SetMode(ConnectMode.InitNew); };
            rbClone = new RadioButton
            {
                Text = "Clone a repository into this folder",
                Location = new Point(230, 4),
                Size = new Size(240, 20)
            };
            rbClone.CheckedChanged += (s, e) => { if (rbClone.Checked) SetMode(ConnectMode.Clone); };
            pnlModeSelection.Controls.AddRange(new Control[] { rbInit, rbClone });
            y += 32;

            // ── STEP 2: Remote URL ──
            lblStep2 = new Label
            {
                Text = "Step 2: Remote repository URL",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 18),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            y += 20;

            lblRemoteUrl = new Label
            {
                Text = "The web address of your Git repository (e.g. from GitHub, Azure DevOps, or Bitbucket):",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 16),
                Font = new Font("Segoe UI", 8.25F),
                ForeColor = Color.Gray
            };
            y += 18;

            txtRemoteUrl = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 23),
                PlaceholderText = "https://github.com/your-org/your-repo.git"
            };
            txtRemoteUrl.TextChanged += (s, e) => UpdateAuthUI();
            y += 32;

            // ── STEP 3: Authentication ──
            lblStep3 = new Label
            {
                Text = "Step 3: Sign in",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 18),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            y += 20;

            lblAuthHelp = new Label
            {
                Text = "Choose how to authenticate. GitHub users can sign in directly.\n" +
                       "For other hosts (Azure DevOps, Bitbucket), use a Personal Access Token (PAT).",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 32),
                Font = new Font("Segoe UI", 8.25F),
                ForeColor = Color.Gray
            };
            y += 35;

            btnSignInGitHub = new Button
            {
                Text = "Sign in with GitHub",
                Location = new Point(leftMargin, y),
                Size = new Size(200, 32),
                Font = new Font("Segoe UI", 9.5F),
                FlatStyle = FlatStyle.System
            };
            btnSignInGitHub.Click += BtnSignInGitHub_Click;
            y += 40;

            lblOrPat = new Label
            {
                Text = "Or enter a Personal Access Token:",
                Location = new Point(leftMargin, y),
                Size = new Size(250, 18),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            lnkCreateToken = new LinkLabel
            {
                Text = "How do I get a token?",
                Location = new Point(leftMargin + 255, y),
                Size = new Size(150, 18)
            };
            lnkCreateToken.LinkClicked += (s, e) =>
            {
                var host = GitHostDetector.DetectHost(txtRemoteUrl.Text);
                var url = GitHostDetector.GetTokenCreationUrl(host);
                if (string.IsNullOrEmpty(url))
                    url = "https://github.com/settings/tokens";
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
                catch { }
            };
            y += 20;

            txtPat = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 23),
                PlaceholderText = "Paste your token here (starts with ghp_ for GitHub)",
                UseSystemPasswordChar = true
            };
            y += 32;

            // ── Username / Email ──
            lblUserInfo = new Label
            {
                Text = "Your name and email (shown in commit history, optional):",
                Location = new Point(leftMargin, y),
                Size = new Size(fieldWidth, 16),
                Font = new Font("Segoe UI", 8.25F),
                ForeColor = Color.Gray
            };
            y += 18;

            txtUsername = new TextBox
            {
                Location = new Point(leftMargin, y),
                Size = new Size(230, 23),
                PlaceholderText = "Your name"
            };
            txtEmail = new TextBox
            {
                Location = new Point(leftMargin + 240, y),
                Size = new Size(240, 23),
                PlaceholderText = "Your email"
            };
            y += 35;

            // ── Buttons ──
            btnOk = new Button
            {
                Text = "Connect",
                Location = new Point(leftMargin + fieldWidth - 185, y),
                Size = new Size(90, 32),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + fieldWidth - 85, y),
                Size = new Size(85, 32)
            };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblHeader, lblStatus,
                lblStep1, lblLocalPath, txtLocalPath, btnBrowse,
                lblDetectedInfo, pnlModeSelection,
                lblStep2, lblRemoteUrl, txtRemoteUrl,
                lblStep3, lblAuthHelp, btnSignInGitHub,
                lblOrPat, lnkCreateToken, txtPat,
                lblUserInfo, txtUsername, txtEmail,
                btnOk, btnCancel
            });

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        /// <summary>
        /// Called when the local path text changes. Checks if the folder
        /// is an existing git repo and auto-fills remote URL + credentials.
        /// </summary>
        private void TxtLocalPath_TextChanged(object sender, EventArgs e)
        {
            string path = txtLocalPath.Text.Trim();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                lblDetectedInfo.Visible = false;
                return;
            }

            // Check if this folder (or a parent) is already a git repo
            string repoRoot = LibgitFunctionClass.FindRepoRoot(path);
            if (repoRoot != null)
            {
                _detectedRepoPath = repoRoot;
                _mode = ConnectMode.OpenExisting;

                // Auto-populate remote URL from git config
                string remoteUrl = LibgitFunctionClass.ReadRemoteUrl(repoRoot);
                if (!string.IsNullOrEmpty(remoteUrl))
                {
                    txtRemoteUrl.Text = remoteUrl;
                }

                // Try to load stored credentials
                try
                {
                    var cred = CredentialHandler.ReadCredential(repoRoot);
                    if (cred != null)
                    {
                        if (!string.IsNullOrEmpty(cred.Password) && string.IsNullOrEmpty(txtPat.Text))
                            txtPat.Text = cred.Password;
                        if (!string.IsNullOrEmpty(cred.UserName) && string.IsNullOrEmpty(txtUsername.Text))
                            txtUsername.Text = cred.UserName;
                        if (!string.IsNullOrEmpty(cred.Comment) && string.IsNullOrEmpty(txtEmail.Text))
                            txtEmail.Text = cred.Comment;
                    }
                }
                catch { }

                // Get current branch info
                string branchInfo = "";
                try
                {
                    string branch = LibgitFunctionClass.git_current_branch(repoRoot);
                    branchInfo = $" (branch: {branch})";
                }
                catch { }

                lblDetectedInfo.Text = $"Git repository detected at: {repoRoot}{branchInfo}";
                lblDetectedInfo.Visible = true;
                pnlModeSelection.Visible = false;

                lblStatus.Text = "Existing repository found! Fill in authentication and click Connect.";
                lblStatus.ForeColor = Color.FromArgb(0, 120, 0);
            }
            else
            {
                _detectedRepoPath = null;
                lblDetectedInfo.Text = "No Git repository found in this folder.";
                lblDetectedInfo.ForeColor = Color.FromArgb(180, 100, 0);
                lblDetectedInfo.Visible = true;
                pnlModeSelection.Visible = true;
                _mode = rbClone.Checked ? ConnectMode.Clone : ConnectMode.InitNew;

                lblStatus.Text = "Choose to create a new repository or clone an existing one:";
                lblStatus.ForeColor = Color.FromArgb(60, 60, 60);
            }

            UpdateAuthUI();
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
                    txtLocalPath.Text = projectDir; // This triggers TxtLocalPath_TextChanged

                    if (_detectedRepoPath != null)
                    {
                        lblStatus.Text = "Your Simio project is already in a Git repository.\nFill in authentication below and click Connect.";
                    }
                    else
                    {
                        lblStatus.Text = "Your Simio project is not yet in a Git repository.\nChoose how to set one up:";
                        pnlModeSelection.Visible = true;
                    }
                }
                else
                {
                    // No active project
                    _mode = ConnectMode.Clone;
                    lblStatus.Text = "No Simio project is open. Browse to your project folder,\nor clone a repository to get started.";
                    lblStatus.ForeColor = Color.FromArgb(60, 60, 60);
                }
            }
            catch
            {
                _mode = ConnectMode.Clone;
                lblStatus.Text = "Browse to your project folder to get started:";
                lblStatus.ForeColor = Color.FromArgb(60, 60, 60);
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
            }
            else if (mode == ConnectMode.InitNew)
            {
                txtRemoteUrl.ReadOnly = false;
                txtLocalPath.ReadOnly = false;
            }
        }

        private void UpdateAuthUI()
        {
            var host = GitHostDetector.DetectHost(txtRemoteUrl.Text);

            // Always show the GitHub button -- it's the most common case
            // and users may not have typed the URL yet
            if (host == GitHost.GitHub || host == GitHost.Unknown)
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

            // Always show the token link
            lnkCreateToken.Visible = true;
        }

        private async void BtnSignInGitHub_Click(object sender, EventArgs e)
        {
            if (!OAuthDeviceFlowHandler.IsConfigured)
            {
                MessageBox.Show(
                    "GitHub sign-in is not yet configured for this installation.\n\n" +
                    "To use this feature, a GitHub OAuth App must be registered\n" +
                    "and its client_id set in OAuthDeviceFlowHandler.cs.\n\n" +
                    "For now, please use a Personal Access Token (PAT) instead:\n" +
                    "1. Go to github.com > Settings > Developer settings > Personal access tokens\n" +
                    "2. Click 'Generate new token (classic)'\n" +
                    "3. Select the 'repo' scope and generate\n" +
                    "4. Copy the token and paste it in the field below",
                    "GitHub Sign-in Not Configured", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the PAT creation page for convenience
                try { Process.Start(new ProcessStartInfo("https://github.com/settings/tokens") { UseShellExecute = true }); }
                catch { }
                return;
            }

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
                MessageBox.Show(
                    "Could not sign in with GitHub.\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    "Please use a Personal Access Token (PAT) instead.\n" +
                    "Click 'How do I get a token?' for instructions.",
                    "Sign-in Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("Please select a local folder in Step 1.", "Required Field",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // For OpenExisting, use the detected repo root (may differ from browsed path)
                if (_mode == ConnectMode.OpenExisting && _detectedRepoPath != null)
                {
                    repoPath = _detectedRepoPath;
                }

                // Save/update credentials
                if (!string.IsNullOrEmpty(pat))
                {
                    var existingCred = CredentialHandler.ReadCredential(repoPath);
                    if (existingCred == null)
                        CredentialHandler.SaveCredential(repoPath, username, pat, email,
                            Meziantou.Framework.Win32.CredentialPersistence.LocalMachine);
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
                    // Gather diagnostic info
                    string remoteName = "none";
                    string remoteUrlActual = "none";
                    try
                    {
                        using (var diagRepo = new Repository(repoPath))
                        {
                            var r = diagRepo.Network.Remotes["origin"]
                                ?? diagRepo.Network.Remotes.FirstOrDefault();
                            if (r != null) { remoteName = r.Name; remoteUrlActual = r.Url; }
                        }
                    }
                    catch { }

                    bool hasPat = !string.IsNullOrEmpty(pat);

                    MessageBox.Show(
                        "Could not verify permissions for this repository.\n\n" +
                        "Please check the following:\n" +
                        $"  Repository: {repoPath}\n" +
                        $"  Remote: {remoteName} ({remoteUrlActual})\n" +
                        $"  Token provided: {(hasPat ? "Yes" : "No")}\n\n" +
                        "Common fixes:\n" +
                        "- Make sure your Personal Access Token has 'repo' scope\n" +
                        "- Check that the remote URL is correct\n" +
                        "- Try generating a new token at github.com/settings/tokens",
                        "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string branchName = "";
                try { branchName = LibgitFunctionClass.git_current_branch(repoPath); }
                catch { }

                // Check if a project is already open
                bool projectAlreadyOpen = false;
                try { projectAlreadyOpen = GitContext.Instance.simioContext?.ActiveProject != null; }
                catch { }

                if (projectAlreadyOpen)
                {
                    MessageBox.Show(
                        $"Connected successfully!\n\n" +
                        $"Repository: {repoPath}\n" +
                        $"Branch: {branchName}\n" +
                        $"Permission: {permStr}\n\n" +
                        "You can now use Commit & Push, Pull, and other\n" +
                        "version control actions from the ribbon.",
                        "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // No project open -- find and offer to open .simproj
                    TryOpenProject(repoPath, branchName, permStr);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TryOpenProject(string repoPath, string branchName, string permStr)
        {
            try
            {
                // Search for .simproj files in the repo
                var simprojFiles = Directory.EnumerateFiles(repoPath, "*.simproj", SearchOption.AllDirectories).ToList();

                if (simprojFiles.Count == 0)
                {
                    MessageBox.Show(
                        $"Connected successfully!\n\n" +
                        $"Repository: {repoPath}\n" +
                        $"Branch: {branchName}\n" +
                        $"Permission: {permStr}\n\n" +
                        "No Simio project files (.simproj) were found in this repository.\n" +
                        "You can create a new project and save it to this folder.",
                        "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string selectedFile;

                if (simprojFiles.Count == 1)
                {
                    selectedFile = simprojFiles[0];
                }
                else
                {
                    // Multiple .simproj files -- let user pick
                    // Show relative paths for readability
                    var relPaths = simprojFiles.Select(f =>
                        f.StartsWith(repoPath) ? f.Substring(repoPath.Length).TrimStart('\\', '/') : f
                    ).ToList();

                    string fileList = string.Join("\n", relPaths.Select((p, i) => $"  {i + 1}. {p}"));

                    // For simplicity, use the first one but tell the user
                    selectedFile = simprojFiles[0];
                    string selectedRel = relPaths[0];

                    DialogResult pickResult = MessageBox.Show(
                        $"Connected successfully! (Branch: {branchName}, Permission: {permStr})\n\n" +
                        $"Found {simprojFiles.Count} Simio project files:\n{fileList}\n\n" +
                        $"Open '{selectedRel}'?",
                        "Open Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (pickResult != DialogResult.Yes) return;
                }

                // Open the project file in Simio
                try
                {
                    GitContext.Instance.simioContext.ExecuteUICommand("LoadProject", selectedFile);

                    MessageBox.Show(
                        $"Connected and project loaded!\n\n" +
                        $"Branch: {branchName}\n" +
                        $"Permission: {permStr}\n\n" +
                        "You can now use Commit & Push, Pull, and other\n" +
                        "version control actions from the ribbon.",
                        "Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    // LoadProject may not be available -- try opening via shell
                    try
                    {
                        Process.Start(new ProcessStartInfo(selectedFile) { UseShellExecute = true });
                        MessageBox.Show(
                            $"Connected! Opening project...\n\n" +
                            $"Branch: {branchName}\n" +
                            $"Permission: {permStr}",
                            "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show(
                            $"Connected successfully!\n\n" +
                            $"Branch: {branchName}\n" +
                            $"Permission: {permStr}\n\n" +
                            $"Please open your project manually:\n{selectedFile}",
                            "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show(
                    $"Connected successfully!\n\n" +
                    $"Repository: {repoPath}\n" +
                    $"Branch: {branchName}\n" +
                    $"Permission: {permStr}",
                    "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                throw new Exception(
                    "Remote URL is required to create a repository.\n\n" +
                    "This is the web address of your repository on GitHub,\n" +
                    "Azure DevOps, or Bitbucket. Create a new empty repository\n" +
                    "on your hosting service first, then paste the URL here.");

            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string dllDirectory = Path.GetDirectoryName(assemblyLocation);
            string gitIgnorePath = Path.Combine(dllDirectory, ".gitignore");

            LibgitFunctionClass.git_init(repoPath, remoteUrl,
                GitContext.Instance.GetSignature(), gitIgnorePath);
        }

        private void ExecuteClone(string repoPath, string remoteUrl)
        {
            if (string.IsNullOrEmpty(remoteUrl))
                throw new Exception(
                    "Remote URL is required to clone a repository.\n\n" +
                    "This is the web address of the repository you want to download.\n" +
                    "You can find it on GitHub by clicking the green 'Code' button.");

            LibgitFunctionClass.git_clone(repoPath, remoteUrl);
        }
    }
}
