using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.CodeParser;


//using System.Windows.Forms;
using SimioAPI;
using SimioAPI.Extensions;
using System.Reflection;
using LibGit2Sharp;



namespace GitFlow
{
    public class InitRepo : IDesignAddIn, IDesignAddInGuiDetails
    {
        




        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Initialize Repository"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "Turns a folder on your computer into a Git repository to start tracking file changes.\r\nMust be connected to a remote repository \r\n"; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.IconIco; ; }
        }

        /// <summary>
        /// Method called when the add-in is run.
        /// </summary>\

        #endregion


        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            GitNewForm FormViewer = new GitNewForm();

            FormViewer.Show();
        }
            


        #region IDesignAddInGuiDetails Members

        // Here is a sample implementation of the optional IDesignAddInGuiDetails interface.
        // To use this implementation, un-comment the interface name on the "class" line at
        // the top of this file.
        //
        // If a design-time add-in implements this optional interface, it can specify where
        // in Simio's ribbon area it should appear.  Merely implementing the interface, and
        // returning null for CategoryName, TabName, and GroupName will cause the add-in to
        // appear at a default location defined by Simio.  However, the add-in can return a
        // specific name for any or all of these properties, to indicate where it should be
        // located in Simio's ribbon area.

        /// <summary>
        /// Property returning the category name for this Add-In.  Return null to use Simio's default add-in category name.
        /// </summary>
        public string CategoryName
        {
            get { return "Version Control"; }
        }

        /// <summary>
        /// Property returning the group name for this Add-In.  Return null to use Simio's default add-in group name.
        /// </summary>
        public string GroupName
        {
            get { return "Repository"; }
        }

        /// <summary>
        /// Property returning the tab name for this Add-In.  Return null to use Simio's default add-in tab name.
        /// </summary>
        public string TabName
        {
            get { return "GitFlow"; }
        }

        #endregion

    }

    public class CloneRepo : IDesignAddIn, IDesignAddInGuiDetails
    {
        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Clone Repository"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "Downloads a complete copy of an existing remote repository, including all files and history, to your machine. Syncs your new local repository to the remote cloned repository \r\n"; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.IconIco; ; }
        }



        #endregion

        /// <summary>
        /// Method called when the add-in is run.
        /// </summary>\

        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            CloneRepoForm FormViewer = new CloneRepoForm();

            FormViewer.Show();
        }


        #region IDesignAddInGuiDetails Members

        // Here is a sample implementation of the optional IDesignAddInGuiDetails interface.
        // To use this implementation, un-comment the interface name on the "class" line at
        // the top of this file.
        //
        // If a design-time add-in implements this optional interface, it can specify where
        // in Simio's ribbon area it should appear.  Merely implementing the interface, and
        // returning null for CategoryName, TabName, and GroupName will cause the add-in to
        // appear at a default location defined by Simio.  However, the add-in can return a
        // specific name for any or all of these properties, to indicate where it should be
        // located in Simio's ribbon area.

        /// <summary>
        /// Property returning the category name for this Add-In.  Return null to use Simio's default add-in category name.
        /// </summary>
        public string CategoryName
        {
            get { return "Version Control"; }
        }

        /// <summary>
        /// Property returning the group name for this Add-In.  Return null to use Simio's default add-in group name.
        /// </summary>
        public string GroupName
        {
            get { return "Repository"; }
        }

        /// <summary>
        /// Property returning the tab name for this Add-In.  Return null to use Simio's default add-in tab name.
        /// </summary>
        public string TabName
        {
            get { return "GitFlow"; }
        }

        #endregion

    }

    public class OpenRepo : IDesignAddIn, IDesignAddInGuiDetails
    {
        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Connect Repository"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "Links your local repository to a remote one on a server, enabling you to sync changes."; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.IconIco; ; }
        }

        /// <summary>
        /// Method called when the add-in is run.
        /// </summary>\
        /*
        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            if (context.ActiveModel != null)
            {
                var intelligentObjects = context.ActiveModel.Facility.IntelligentObjects;

                // Create the Source, Server, and Sink. Space them out along a diagonal line. The X and Z coordinate of the location specify the left to right and top to bottom 
                //  coordinates from a top down view. The Y coordinate specifies the elevation. We cast them to IFixedObject here so that we can get to their Nodes collection
                //  later
                var source = intelligentObjects.CreateObject("Source", new FacilityLocation(-5, 0, -5)) as IFixedObject;
                var server = intelligentObjects.CreateObject("Server", new FacilityLocation(0, 0, 0)) as IFixedObject;
                var sink = intelligentObjects.CreateObject("Sink", new FacilityLocation(5, 0, 5)) as IFixedObject;

                if (source == null || server == null || sink == null)
                {
                    MessageBox.Show("Could not create Standard Library objects. You need to load the Standard Library in the Facility view.");
                    return;
                }

                // Nodes is an IEnumerable, so we will create a temporary List from it to quickly get to the first node in the set
                var sourceoutput = new List<INodeObject>(source.Nodes)[0];

                var servernodes = new List<INodeObject>(server.Nodes);
                var serverinput = servernodes[0];
                var serveroutput = servernodes[1];

                var sinkinput = new List<INodeObject>(sink.Nodes)[0];

                // This path goes directly from the output of source to the input of server
                var path1 = intelligentObjects.CreateLink("Path", sourceoutput, serverinput, null);
                // This path goes from the output of server to the input of sink, with one vertex in between
                var path2 = intelligentObjects.CreateLink("Path", serveroutput, sinkinput, new List<FacilityLocation> { new FacilityLocation(3, 0, 0) });
            }
            else
            {
                MessageBox.Show("You must have an active model to run this add-in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */

        #endregion


        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            OpenRepoForm FormViewer = new OpenRepoForm();

            FormViewer.Show();
        }


        #region IDesignAddInGuiDetails Members

        // Here is a sample implementation of the optional IDesignAddInGuiDetails interface.
        // To use this implementation, un-comment the interface name on the "class" line at
        // the top of this file.
        //
        // If a design-time add-in implements this optional interface, it can specify where
        // in Simio's ribbon area it should appear.  Merely implementing the interface, and
        // returning null for CategoryName, TabName, and GroupName will cause the add-in to
        // appear at a default location defined by Simio.  However, the add-in can return a
        // specific name for any or all of these properties, to indicate where it should be
        // located in Simio's ribbon area.

        /// <summary>
        /// Property returning the category name for this Add-In.  Return null to use Simio's default add-in category name.
        /// </summary>
        public string CategoryName
        {
            get { return "Version Control"; }
        }

        /// <summary>
        /// Property returning the group name for this Add-In.  Return null to use Simio's default add-in group name.
        /// </summary>
        public string GroupName
        {
            get { return "Repository"; }
        }

        /// <summary>
        /// Property returning the tab name for this Add-In.  Return null to use Simio's default add-in tab name.
        /// </summary>
        public string TabName
        {
            get { return "GitFlow"; }
        }

        #endregion

    }

    public class Commit_Push_Maybe_Branch : IDesignAddIn, IDesignAddInGuiDetails
    {
        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Commit/Push"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "A commit saves a snapshot of your changes locally, and a push uploads those saved changes to a remote server.\r\nMUST save before this to ensure all changes are pushed \r\n"; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.IconIco; }
        }



        #endregion


        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Open Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Open the OpenRepoForm to allow the user to connect to a repository
                        OpenRepoForm openRepoForm = new OpenRepoForm();
                        openRepoForm.ShowDialog();
                    }
                    else
                    {
                        // If the user chooses not to connect, exit the method
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (LibgitFunctionClass.git_main_branch_check(GitContext.Instance.RepositoryPath))
                {
                    // Display a Yes/No message box
                    DialogResult result = MessageBox.Show(Resources.Resource1.BranchUponCommitPushPrompt, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // Handle the user's choice
                    if (result == DialogResult.Yes)
                    {
                        //open dialog to create branch
                        CreateBranchForm createBranchForm = new CreateBranchForm();
                        createBranchForm.Show();

                    }
                    else if (result == DialogResult.No)
                    {
                        // Perform the action for No
                        if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                        {
                            //Only commit if there are uncommited changes
                            //open git commit form
                            CommitForm commitForm = new CommitForm();
                            commitForm.Show();


                        }
                        else
                        {
                            try
                            {
                                LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                            }
                            catch (Exception ex)
                            {
                                // Show the error message and do not close the form
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }


                    }
                }
                else
                {
                    if (LibgitFunctionClass.git_dirty(GitContext.Instance.RepositoryPath))
                    {
                        //Only commit if there are uncommited changes
                        //open git commit form
                        CommitForm commitForm = new CommitForm();
                        commitForm.Show();


                    }
                    else
                    {
                        try
                        {
                            LibgitFunctionClass.git_safe_push(GitContext.Instance.RepositoryPath, GitContext.Instance.GetSignature());
                        }
                        catch (Exception ex)
                        {
                            // Show the error message and do not close the form
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region IDesignAddInGuiDetails Members

        // Here is a sample implementation of the optional IDesignAddInGuiDetails interface.
        // To use this implementation, un-comment the interface name on the "class" line at
        // the top of this file.
        //
        // If a design-time add-in implements this optional interface, it can specify where
        // in Simio's ribbon area it should appear.  Merely implementing the interface, and
        // returning null for CategoryName, TabName, and GroupName will cause the add-in to
        // appear at a default location defined by Simio.  However, the add-in can return a
        // specific name for any or all of these properties, to indicate where it should be
        // located in Simio's ribbon area.

        /// <summary>
        /// Property returning the category name for this Add-In.  Return null to use Simio's default add-in category name.
        /// </summary>
        public string CategoryName
        {
            get { return "Version Control"; }
        }

        /// <summary>
        /// Property returning the group name for this Add-In.  Return null to use Simio's default add-in group name.
        /// </summary>
        public string GroupName
        {
            get { return "Git Actions"; }
        }

        /// <summary>
        /// Property returning the tab name for this Add-In.  Return null to use Simio's default add-in tab name.
        /// </summary>
        public string TabName
        {
            get { return "GitFlow"; }
        }

        #endregion

    }

    public class SelectBranch : IDesignAddIn, IDesignAddInGuiDetails
    {
        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Select Branch"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "Switches your current working context to a different line of development within the repository.\r\nOpen and close project to see changes \r\n"; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.IconIco; ; }
        }



        #endregion


        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Open Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Open the OpenRepoForm to allow the user to connect to a repository
                        OpenRepoForm openRepoForm = new OpenRepoForm();
                        openRepoForm.ShowDialog();
                    }
                    else
                    {
                        // If the user chooses not to connect, exit the method
                        MessageBox.Show(Resources.Resource1.ConnectRepoBeforeActionError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                //bring up branch select form
                BranchSelectForm FormViewer = new BranchSelectForm();
                FormViewer.Show();
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region IDesignAddInGuiDetails Members

        // Here is a sample implementation of the optional IDesignAddInGuiDetails interface.
        // To use this implementation, un-comment the interface name on the "class" line at
        // the top of this file.
        //
        // If a design-time add-in implements this optional interface, it can specify where
        // in Simio's ribbon area it should appear.  Merely implementing the interface, and
        // returning null for CategoryName, TabName, and GroupName will cause the add-in to
        // appear at a default location defined by Simio.  However, the add-in can return a
        // specific name for any or all of these properties, to indicate where it should be
        // located in Simio's ribbon area.

        /// <summary>
        /// Property returning the category name for this Add-In.  Return null to use Simio's default add-in category name.
        /// </summary>
        public string CategoryName
        {
            get { return "Version Control"; }
        }

        /// <summary>
        /// Property returning the group name for this Add-In.  Return null to use Simio's default add-in group name.
        /// </summary>
        public string GroupName
        {
            get { return "Git Actions"; }
        }

        /// <summary>
        /// Property returning the tab name for this Add-In.  Return null to use Simio's default add-in tab name.
        /// </summary>
        public string TabName
        {
            get { return "GitFlow"; }
        }

        #endregion

    }

    public class MergeOverMain : IDesignAddIn, IDesignAddInGuiDetails
    {
        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "Merge to Main"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.  
        /// </summary>
        public string Description
        {
            get { return "Integrates the completed work from current branch into the primary main branch of the project.\r\n\r\n"; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.IconIco; }
        }



        #endregion


        public void Execute(SimioAPI.Extensions.IDesignContext context)
        {

            //var path = context.ActiveProject.
            //tetsting with context provided by Simio
            try
            {
                if (GitContext.Instance.IsInitialized == false)
                {
                    DialogResult result = MessageBox.Show(Resources.Resource1.ConnectRepoPrompt, "Open Repository", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Open the OpenRepoForm to allow the user to connect to a repository
                        OpenRepoForm openRepoForm = new OpenRepoForm();
                        openRepoForm.ShowDialog();
                    }
                    else
                    {
                        // If the user chooses not to connect, exit the method
                        MessageBox.Show(Resources.Resource1.MustHaveRepoInstanceMerge, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (LibgitFunctionClass.git_main_branch_check(GitContext.Instance.RepositoryPath))
                {
                    // Error message if the user is already on the main branch
                    MessageBox.Show(Resources.Resource1.MainToMainMergeError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    try
                    {
                        // Attempt to merge the current branch into main
                        LibgitFunctionClass.git_branch_merge_force(GitContext.Instance.RepositoryPath, "main");
                        MessageBox.Show(Resources.Resource1.MergeToMainSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) when (ex.Message.Contains("An error occurred: failed rmdir - "))
                    {
                        //do nothing since the user propbably wont really  care about what is in .git/logs/refs/remotes/origin/
                        //This just causes local branches to not be deleted and take up a bit more space
                        MessageBox.Show(Resources.Resource1.MergeToMainSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //had to put this here because it stops for this error and wont execute otherwise even though error is not crucial
                    }
                    catch (Exception ex)
                    {
                        // Show the error message and do not close the form
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Show the error message and do not close the form
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /*
        internal static string GetStringProperty(Object obj, string propertyName)
        {
            try
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    var getter = pi.GetGetMethod();
                    if(!(getter.ReturnType.IsArray))
                    {
                        if (pi.Name == propertyName)
                        {
                            var vv = pi.GetValue(obj, null);
                            return (vv ?? "").ToString();
                        }
                    }

                }
                return string.Empty;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        */


        #region IDesignAddInGuiDetails Members

        // Here is a sample implementation of the optional IDesignAddInGuiDetails interface.
        // To use this implementation, un-comment the interface name on the "class" line at
        // the top of this file.
        //
        // If a design-time add-in implements this optional interface, it can specify where
        // in Simio's ribbon area it should appear.  Merely implementing the interface, and
        // returning null for CategoryName, TabName, and GroupName will cause the add-in to
        // appear at a default location defined by Simio.  However, the add-in can return a
        // specific name for any or all of these properties, to indicate where it should be
        // located in Simio's ribbon area.

        /// <summary>
        /// Property returning the category name for this Add-In.  Return null to use Simio's default add-in category name.
        /// </summary>
        public string CategoryName
        {
            get { return "Version Control"; }
        }

        /// <summary>
        /// Property returning the group name for this Add-In.  Return null to use Simio's default add-in group name.
        /// </summary>
        public string GroupName
        {
            get { return "Git Actions"; }
        }

        /// <summary>
        /// Property returning the tab name for this Add-In.  Return null to use Simio's default add-in tab name.
        /// </summary>
        public string TabName
        {
            get { return "GitFlow"; }
        }

        #endregion

    }

}









//sample code from different add in
/*
try
{


    // Check to make sure a model has been opened in Simio
    string projectFileName = SystemDirectoryHandler.GetStringProperty(context.ActiveProject, "FileName");
    if (string.IsNullOrEmpty(projectFileName))
    {
        string message = "There is no active project file. Enter the Parent folder under which the Simio Project folder will reside.";
        GitContext.ParentFolderPath = GetFolderPath(message);
        if (!Directory.Exists(GitContext.ParentFolderPath))
        {
            AlertLog($"The folder {GitContext.ParentFolderPath} does not exist.  Please create it and try again.");
            return;
        }
        // See if it has children
        if (Directory.GetDirectories(GitContext.ParentFolderPath).Length == 1)
        {
            string childPath = Directory.GetDirectories(GitContext.ParentFolderPath)[0];
            GitContext.SimioProjectName = SystemDirectoryHandler.GetLastFolderName(childPath);
            AlertLog($"The parent folder has one Child, which is assumed to be the Simio Project={GitContext.SimioProjectName}");
        }
        else
        {
            GitContext.SimioProjectName = string.Empty;
        }
    }
    else // We found the name in the file.
    {
        string fullProjectFilePath = SystemDirectoryHandler.GetStringProperty(context.ActiveProject, "FileName");
        string folderPath = Path.GetDirectoryName(fullProjectFilePath);
        GitContext.ParentFolderPath = SystemDirectoryHandler.GetParentFolderFullPath(folderPath);
        GitContext.SimioProjectName = SystemDirectoryHandler.GetLastFolderName(folderPath);
    }

    StringBuilder sb = new StringBuilder();

    // Check for folder expectations

    if (Directory.Exists(GitContext.ProjectFolderPath))
    {
        string gitFolder = Path.Combine(GitContext.ProjectFolderPath, ".git");
        if (!Directory.Exists(gitFolder))
        {
            AlertLog($"No .git folder ({gitFolder}). Please set up Git on the Simio project folder.");
            goto ShowForm;
        }
    }

}

catch (Exception ex)
{
    if (ex.Message != "Canceled")
    {
        MessageBox.Show(ex.Message, "Execute Error");
    }
}
*/
/*
ShowForm:

    // Launch the form and give it access to the Simio Design and Git Contexts
    GitNewForm FormViewer = new GitNewForm
    {
        GitContext = GitContext
    };

    FormViewer.Show();
*/