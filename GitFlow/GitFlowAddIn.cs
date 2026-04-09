using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.CodeParser;

using System.Windows.Forms;
using SimioAPI;
using SimioAPI.Extensions;
using System.Reflection;
using LibGit2Sharp;
using System.IO;



namespace GitFlow
{
    /// <summary>
    /// Shared helper for all add-in actions. Ensures the user is connected
    /// to a Git repository before performing any operation.
    /// </summary>
    internal static class AddInHelper
    {
        /// <summary>
        /// Ensures GitContext is initialized and the user has at least the
        /// required permission level. Attempts auto-connect if possible.
        /// Returns true if ready to proceed, false if the user should abort.
        /// </summary>
        /// <param name="context">The Simio design context.</param>
        /// <param name="requiredPermission">1 = read, 2 = read/write</param>
        public static bool EnsureConnected(IDesignContext context, int requiredPermission)
        {
            GitContext.Instance.simioContext = context;

            // Already initialized -- just check permissions
            if (GitContext.Instance.IsInitialized)
            {
                return CheckPermission(requiredPermission);
            }

            // Try auto-connect: detect repo from the active project
            if (TryAutoConnect(context))
            {
                return CheckPermission(requiredPermission);
            }

            // Auto-connect failed -- open ConnectForm
            ConnectForm connectForm = new ConnectForm();
            connectForm.ShowDialog();

            // Check if ConnectForm succeeded
            if (GitContext.Instance.IsInitialized)
            {
                return CheckPermission(requiredPermission);
            }

            return false;
        }

        private static bool TryAutoConnect(IDesignContext context)
        {
            try
            {
                // Get project file path
                string projectPath = null;
                if (context?.ActiveProject != null)
                {
                    projectPath = SystemDirectoryHandler.GetStringProperty(
                        context.ActiveProject, "FileName");
                }

                if (string.IsNullOrEmpty(projectPath))
                    return false;

                string projectDir = Path.GetDirectoryName(projectPath);
                string repoRoot = LibgitFunctionClass.FindRepoRoot(projectDir);
                if (repoRoot == null)
                    return false;

                // Found a repo -- read remote URL
                string remoteUrl = LibgitFunctionClass.ReadRemoteUrl(repoRoot);

                // Try to load stored credentials
                string pat = "";
                string username = "DefaultUser";
                string email = "DefaultUser@email.com";

                try
                {
                    var cred = CredentialHandler.ReadCredential(repoRoot);
                    if (cred != null)
                    {
                        pat = cred.Password ?? "";
                        username = cred.UserName ?? "DefaultUser";
                        email = cred.Comment ?? "DefaultUser@email.com";
                    }
                }
                catch { }

                // Need credentials to verify permissions
                if (string.IsNullOrEmpty(pat))
                    return false;

                // Initialize context
                GitContext.Instance.Initialize(repoRoot, remoteUrl, pat, username, email);

                // Check permissions
                int permLevel = LibgitFunctionClass.GetPermission(repoRoot);
                GitContext.Instance.PermissionLevel = permLevel;

                return permLevel > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool CheckPermission(int requiredPermission)
        {
            if (GitContext.Instance.PermissionLevel >= requiredPermission)
                return true;

            if (requiredPermission >= 2)
                MessageBox.Show(Resources.Resource1.PermissionErrorBlockedAction,
                    "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show(Resources.Resource1.PermissionErrorReadAction,
                    "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return false;
        }
    }


    public class ConnectRepo : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => "Connect";
        public string Description => "Connect to a Git repository - create new, clone existing, or open a local repo";

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageConnect))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            GitContext.Instance.simioContext = context;
            ConnectForm form = new ConnectForm();
            form.ShowDialog();
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameRepoActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class Commit_Push_Maybe_Branch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => Resources.Resource1.ButtonLabelCommitPush;
        public string Description => Resources.Resource1.ButtonDescriptionCommitPush;

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageCommitPush))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 2)) return;

                if (LibgitFunctionClass.git_main_branch_check(GitContext.Instance.RepositoryPath))
                {
                    DialogResult result = MessageBox.Show(
                        Resources.Resource1.BranchUponCommitPushPrompt,
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        CreateBranchForm createBranchForm = new CreateBranchForm();
                        createBranchForm.Show();
                        return;
                    }
                }

                // On a dev branch, or user chose not to create a branch
                if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                {
                    CommitForm commitForm = new CommitForm();
                    commitForm.Show();
                }
                else
                {
                    LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                    MessageBox.Show(Resources.Resource1.PushSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class Pull : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => Resources.Resource1.ButtonLabelPull;
        public string Description => Resources.Resource1.ButtonDescriptionPull;

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImagePull))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 1)) return;

                if (LibgitFunctionClass.git_safe_pull(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature()))
                {
                    MessageBox.Show(Resources.Resource1.NormalPullSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SystemDirectoryHandler.Refresh();
                }
            }
            catch (Exception ex) when (ex.Message.Contains("conflicts prevent checkout") || ex.Message.Contains("Cannot perform fast-forward merge"))
            {
                if (DialogResult.Yes == MessageBox.Show(Resources.Resource1.PullForcePromptAfterFailSafePull, "Merge Conflict", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    try
                    {
                        LibgitFunctionClass.git_force_pull(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                        MessageBox.Show(Resources.Resource1.PullForceSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SystemDirectoryHandler.Refresh();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex) when (ex.Message.Contains("403"))
            {
                MessageBox.Show(Resources.Resource1.AuthenticationError, "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) when (ex.Message.Contains("404"))
            {
                MessageBox.Show(Resources.Resource1.RemoteRetrival, "Remote Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class LocalReset : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => Resources.Resource1.ButtonLabelGitReset;
        public string Description => Resources.Resource1.ButtonDescriptionGitReset;

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageReset))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 1)) return;

                if (LibgitFunctionClass.git_reset_local(GitContext.Instance.RepositoryPath))
                {
                    MessageBox.Show(Resources.Resource1.LocalResetSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SystemDirectoryHandler.Refresh();
                }
            }
            catch (Exception ex) when (ex.Message.Contains("403"))
            {
                MessageBox.Show(Resources.Resource1.AuthenticationError, "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) when (ex.Message.Contains("404"))
            {
                MessageBox.Show(Resources.Resource1.RemoteRetrival, "Remote Retrieval Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class CreateBranch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => "Create Branch";
        public string Description => "Create a new branch for your changes (keeps main safe)";

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageSelectBranch))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 2)) return;

                CreateBranchForm form = new CreateBranchForm();
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameBranchingActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class SelectBranch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => Resources.Resource1.ButtonLabelSelectBranch;
        public string Description => Resources.Resource1.ButtonDescriptionSelectBranch;

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageSelectBranch))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 1)) return;

                BranchSelectForm FormViewer = new BranchSelectForm();
                FormViewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameBranchingActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class MergeOverMain : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => Resources.Resource1.ButtonLabelMergeMain;
        public string Description => Resources.Resource1.ButtonDescriptionMergeMain;

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImagePromote))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 2)) return;

                if (LibgitFunctionClass.git_main_branch_check(GitContext.Instance.RepositoryPath))
                {
                    MessageBox.Show(Resources.Resource1.MainToMainMergeError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    if (!LibgitFunctionClass.CanMergeWithoutConflicts(GitContext.Instance.RepositoryPath, "main"))
                    {
                        string warningMessage = "Merge conflicts detected. Forcing this merge will overwrite the 'main' branch with your current branch's content. This is a destructive action and cannot be undone easily.\n\nDo you want to continue?";
                        DialogResult result = MessageBox.Show(warningMessage, "Conflict Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result != DialogResult.Yes) return;
                    }

                    LibgitFunctionClass.git_branch_merge_force(GitContext.Instance.RepositoryPath, "main");
                }
                catch (Exception ex) when (ex.Message.Contains("An error occurred: failed rmdir - "))
                {
                    MessageBox.Show(Resources.Resource1.MergeToMainSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameBranchingActions;
        public string TabName => Resources.Resource1.TabName;
    }


    public class RemoveBranch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name => Resources.Resource1.ButtonLabelRemoveBranch;
        public string Description => Resources.Resource1.ButtonDescriptionRemoveBranch;

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageRemoveBranch))
                    return Image.FromStream(ms);
            }
        }

        public void Execute(IDesignContext context)
        {
            try
            {
                if (!AddInHelper.EnsureConnected(context, 2)) return;

                BranchRemoveForm FormViewer = new BranchRemoveForm();
                FormViewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName => Resources.Resource1.CatagoryNameVC;
        public string GroupName => Resources.Resource1.GroupNameBranchingActions;
        public string TabName => Resources.Resource1.TabName;
    }

}
