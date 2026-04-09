using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitFlow
{
    public partial class OAuthProgressForm : Form
    {
        private readonly string _userCode;
        private readonly string _verificationUri;
        private readonly CancellationTokenSource _cts;
        private Task<string> _pollTask;

        public string AccessToken { get; private set; }

        public OAuthProgressForm(string userCode, string verificationUri, CancellationTokenSource cts)
        {
            _userCode = userCode;
            _verificationUri = verificationUri;
            _cts = cts;

            InitializeFormLayout();
        }

        private void InitializeFormLayout()
        {
            this.Text = "Sign in with GitHub";
            this.Size = new System.Drawing.Size(420, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblInstruction = new Label
            {
                Text = "To sign in, enter this code on GitHub:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(360, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            var lblCode = new Label
            {
                Text = _userCode,
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(360, 45),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Consolas", 22, System.Drawing.FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245)
            };

            var btnCopyAndOpen = new Button
            {
                Text = "Copy Code && Open Browser",
                Location = new System.Drawing.Point(80, 115),
                Size = new System.Drawing.Size(240, 35),
                Font = new System.Drawing.Font("Segoe UI", 10)
            };
            btnCopyAndOpen.Click += (s, e) =>
            {
                Clipboard.SetText(_userCode);
                try
                {
                    Process.Start(new ProcessStartInfo(_verificationUri) { UseShellExecute = true });
                }
                catch
                {
                    MessageBox.Show($"Please open this URL in your browser:\n{_verificationUri}",
                        "Open Browser", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            var lblStatus = new Label
            {
                Text = "Waiting for authorization...",
                Location = new System.Drawing.Point(20, 165),
                Size = new System.Drawing.Size(360, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.Gray
            };

            var progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(80, 190),
                Size = new System.Drawing.Size(240, 10),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(155, 210),
                Size = new System.Drawing.Size(90, 30)
            };
            btnCancel.Click += (s, e) =>
            {
                _cts.Cancel();
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.AddRange(new Control[]
            {
                lblInstruction, lblCode, btnCopyAndOpen, lblStatus, progressBar, btnCancel
            });

            this.FormClosing += (s, e) =>
            {
                if (AccessToken == null)
                    _cts.Cancel();
            };
        }

        public void SetPollTask(Task<string> pollTask)
        {
            _pollTask = pollTask;

            // Monitor the poll task and close the form when done
            _pollTask.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    AccessToken = t.Result;
                    this.BeginInvoke(new Action(() =>
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }));
                }
                else if (t.IsFaulted)
                {
                    var errorMsg = t.Exception?.InnerException?.Message ?? "Authorization failed.";
                    this.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(errorMsg, "Authorization Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.Abort;
                        this.Close();
                    }));
                }
            }, TaskScheduler.Default);
        }
    }
}
