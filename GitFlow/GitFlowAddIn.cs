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
    public class ConnectRepo : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return "Connect"; }
        }

        public string Description
        {
            get { return "Connect to a Git repository - create new, clone existing, or open a local repo"; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageConnect))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            GitContext.Instance.simioContext = context;
            ConnectForm form = new ConnectForm();
            form.ShowDialog();
        }

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameRepoActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

    public class Commit_Push_Maybe_Branch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return Resources.Resource1.ButtonLabelCommitPush; }
        }

        public string Description
        {
            get { return Resources.Resource1.ButtonDescriptionCommitPush; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageCommitPush))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            GitContext.Instance.simioContext = context;
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Connect Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ConnectForm connectForm = new ConnectForm();
                        connectForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (GitContext.Instance.PermissionLevel != 2)
                {
                    MessageBox.Show(Resources.Resource1.PermissionErrorBlockedAction, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (LibgitFunctionClass.git_main_branch_check(GitContext.Instance.RepositoryPath))
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.BranchUponCommitPushPrompt, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        CreateBranchForm createBranchForm = new CreateBranchForm();
                        createBranchForm.Show();
                    }
                    else if (result == DialogResult.No)
                    {
                        if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                        {
                            CommitForm commitForm = new CommitForm();
                            commitForm.Show();
                        }
                        else
                        {
                            try
                            {
                                LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                                MessageBox.Show(Resources.Resource1.PushSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                    {
                        CommitForm commitForm = new CommitForm();
                        commitForm.Show();
                    }
                    else
                    {
                        try
                        {
                            LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                            MessageBox.Show(Resources.Resource1.PushSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

    public class Pull : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return Resources.Resource1.ButtonLabelPull; }
        }

        public string Description
        {
            get { return Resources.Resource1.ButtonDescriptionPull; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImagePull))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            GitContext.Instance.simioContext = context;
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Connect Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ConnectForm connectForm = new ConnectForm();
                        connectForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (GitContext.Instance.PermissionLevel < 1)
                {
                    MessageBox.Show(Resources.Resource1.PermissionErrorReadAction, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string projectFilePath = context.ActiveProject.Name;

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

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

    public class LocalReset : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return Resources.Resource1.ButtonLabelGitReset; }
        }

        public string Description
        {
            get { return Resources.Resource1.ButtonDescriptionGitReset; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageReset))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            try
            {
                GitContext.Instance.simioContext = context;
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Connect Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ConnectForm connectForm = new ConnectForm();
                        connectForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (GitContext.Instance.PermissionLevel < 1)
                {
                    MessageBox.Show(Resources.Resource1.PermissionErrorReadAction, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

    public class SelectBranch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return Resources.Resource1.ButtonLabelSelectBranch; }
        }

        public string Description
        {
            get { return Resources.Resource1.ButtonDescriptionSelectBranch; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageSelectBranch))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            GitContext.Instance.simioContext = context;
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Connect Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ConnectForm connectForm = new ConnectForm();
                        connectForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (GitContext.Instance.PermissionLevel < 1)
                {
                    MessageBox.Show(Resources.Resource1.PermissionErrorReadAction, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BranchSelectForm FormViewer = new BranchSelectForm();
                FormViewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameBranchingActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

    public class MergeOverMain : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return Resources.Resource1.ButtonLabelMergeMain; }
        }

        public string Description
        {
            get { return Resources.Resource1.ButtonDescriptionMergeMain; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImagePromote))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            try
            {
                GitContext.Instance.simioContext = context;
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Connect Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ConnectForm connectForm = new ConnectForm();
                        connectForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Resource1.MustHaveRepoInstanceMerge, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (GitContext.Instance.PermissionLevel != 2)
                {
                    MessageBox.Show(Resources.Resource1.PermissionErrorBlockedAction, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (LibgitFunctionClass.git_main_branch_check(GitContext.Instance.RepositoryPath))
                {
                    MessageBox.Show(Resources.Resource1.MainToMainMergeError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    try
                    {
                        if (!LibgitFunctionClass.CanMergeWithoutConflicts(GitContext.Instance.RepositoryPath, "main"))
                        {
                            string warningMessage = "Merge conflicts detected. Forcing this merge will overwrite the 'main' branch with your current branch's content. This is a destructive action and cannot be undone easily.\n\nDo you want to continue?";

                            DialogResult result = MessageBox.Show(warningMessage, "Conflict Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (result == DialogResult.Yes)
                            {
                                LibgitFunctionClass.git_branch_merge_force(GitContext.Instance.RepositoryPath, "main");
                            }
                        }
                        else
                        {
                            LibgitFunctionClass.git_branch_merge_force(GitContext.Instance.RepositoryPath, "main");
                        }

                    }
                    catch (Exception ex) when (ex.Message.Contains("An error occurred: failed rmdir - "))
                    {
                        MessageBox.Show(Resources.Resource1.MergeToMainSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameBranchingActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

    public class RemoveBranch : IDesignAddIn, IDesignAddInGuiDetails
    {
        public string Name
        {
            get { return Resources.Resource1.ButtonLabelRemoveBranch; }
        }

        public string Description
        {
            get { return Resources.Resource1.ButtonDescriptionRemoveBranch; }
        }

        public System.Drawing.Image Icon
        {
            get
            {
                using (var ms = new MemoryStream(Resources.Resource1.ImageRemoveBranch))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            GitContext.Instance.simioContext = context;
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Connect Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ConnectForm connectForm = new ConnectForm();
                        connectForm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else if (GitContext.Instance.PermissionLevel != 2)
                {
                    MessageBox.Show(Resources.Resource1.PermissionErrorBlockedAction, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                BranchRemoveForm FormViewer = new BranchRemoveForm();
                FormViewer.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string CategoryName
        {
            get { return Resources.Resource1.CatagoryNameVC; }
        }

        public string GroupName
        {
            get { return Resources.Resource1.GroupNameBranchingActions; }
        }

        public string TabName
        {
            get { return Resources.Resource1.TabName; }
        }
    }

}
