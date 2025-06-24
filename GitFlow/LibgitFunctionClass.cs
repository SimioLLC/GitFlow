using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LibGit2Sharp;
using GitFlow;
using GitFlow.Properties;

namespace GitFlow

{
    internal class LibgitFunctionClass
    {


        /*
         * Functions Specific for GitFlow AddIn
         * members: commit_and_push_maybe_branch, Merge_to_main_delete_old_branch, Select_Branch
         * members but implimented elsewhere: git_init, git_clone
         */

        public static bool commit_and_push_maybe_branch_handler(string localRepoPath, string commitMessage, Signature signature)
        {
            // Function to commit changes and push to the remote repository, optionally creating a new branch
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // commitMessage: Message for the commit
            // signature: Signature object containing author and committer information
            // branchName: Optional name of the new branch to create before committing


            using (var repo = new Repository(localRepoPath))
            {
                if (repo.Head.FriendlyName == "main" || repo.Head.FriendlyName == "master")
                {
                    //newBranchOption = newBranchPopUP();
                    DialogResult dialogResult = MessageBox.Show(Resources.Resource1.BranchInsteadOfMain, "Commit/Push", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        //create Branch window
                        return true;
                    }
                    else // if (dialogResult == DialogResult.No)
                    {
                        //commit and push to main
                        try { 
                            if (git_dirty(localRepoPath))
                            {
                                if (!git_commit(localRepoPath, commitMessage, signature))
                                {
                                    throw new Exception("Failed to commit changes.");
                                    
                                }
                            }
                            if (!git_push(localRepoPath))
                            {
                                throw new Exception("Failed to push changes.");
                                
                            }
                            return true; // Return true if all operations are successful
                            }
                        catch (Exception ex)
                        {
                            throw new Exception($"An error occurred while committing and pushing changes: {ex.Message}");
                            
                        }
                    }
                }
                else
                {//on a dev branch already
                    if (git_dirty(localRepoPath))
                    {
                        if (!git_commit(localRepoPath, commitMessage, signature))
                        {
                            throw new Exception("Failed to commit changes.");
                            
                        }
                    }
                    if (!git_push(localRepoPath))
                    {
                        throw new Exception("Failed to push changes.");
                        
                    }
                    return true;
                }

            }


        }



        /*
         * Functions for getting started with git
         * members: clone, init
         */




        public static bool git_init(string localRepoPath, string gitRepoUrl, Signature sig, string gitIgnFile = "C:/source/repos/Libgit2sharpTester/ConsoleApp1/Assests/.gitignore")
        {
            // Function to initialize a new Git repository and set it up with a remote
            // Parameters:
            // localRepoPath: Path where the repository will be initialized
            // gitRepoUrl: URL of the remote Git repository to link with
            // sig: Signature object containing author and committer information
            // gitIgnFile: Path to the .gitignore file to be copied into the new repository

            // Initialize a new Git repository at the specified path
            // Check for existing .git directory
            string gitDir = Path.Combine(localRepoPath, ".git");
            if (Directory.Exists(gitDir))
            {
                throw new Exception(Resources.Resource1.Initrepodetected);
            }

            try
            {
                //init the local repository
                Repository.Init(@localRepoPath);

                //check for existing gitignore file
                string gitFile = Path.Combine(localRepoPath, ".gitignore");

                //add a folder called Builds
                string dirName = "Builds";

                string dir = Path.Combine(localRepoPath, dirName);
                if (!Directory.Exists(dir)) 
                {
                    Directory.CreateDirectory(dir);
                    //make a text folder called Build descrption that contains the text: this is a folder that is used to hold all simio builds to keep track of progress and share builds with others

                    string descriptionFileName = "Build_folder_description.txt";
                    string descriptionContent = Resources.Resource1.BuildFolderDescription;

                    // 2. Combine the new directory path with the file name.
                    string filePath = Path.Combine(dir, descriptionFileName);

                    // 3. Create the file and write the content to it.
                    // File.WriteAllText handles creating, writing, and closing the file in one step.
                    File.WriteAllText(filePath, descriptionContent);

                }
                if (!File.Exists(gitFile))
                {
                    File.Copy(gitIgnFile, Path.Combine(localRepoPath, Path.GetFileName(gitIgnFile)), true);

                }
                //add the git ignore
                

                // Create a new repository instance
                using (var repo = new Repository(localRepoPath))
                {
                    //TODO: MOVE CURRENT PROJECT FOLDER CONTENTS TO NEW REPO FOLDER

                    // 1. Stage changes
                    Commands.Stage(repo, "*");

                    // 2. Create a commit
                    repo.Commit("Initial commit", sig, sig);

                    // 3. Creating a main branch if it doesn't exist
                    var mainBranch = repo.Branches["main"];
                    if (mainBranch == null)
                    {
                        mainBranch = repo.CreateBranch("main");
                    }
                    Commands.Checkout(repo, mainBranch);

                    // 4. Add remote
                    if (repo.Network.Remotes["origin"] == null)
                        repo.Network.Remotes.Add("origin", gitRepoUrl);

                    // 5. Set upstream (tracking) for main
                    repo.Branches.Update(mainBranch, b => b.TrackedBranch = "refs/remotes/origin/main");

                    // 6. Push to origin main
                    var remote = repo.Network.Remotes["origin"];
                    repo.Network.Push(mainBranch, new PushOptions { CredentialsProvider = PrivateRepoCredentials });

                }

            }
            catch (Exception ex) when (ex.Message.Contains("unsupported URL protocol"))
            {
                throw new Exception(Resources.Resource1.RemoteRetrival);

            }
            catch (Exception ex) when (ex.Message.Contains("authentication replays"))
            {
                throw new Exception(Resources.Resource1.AuthenticationErrorInit);

            }
            catch (Exception ex) when (ex.Message.Contains("403"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

            }
            catch (Exception ex) when (ex.Message.Contains("404"))
            {
                throw new Exception(Resources.Resource1.RemoteRetrival);
            }
            catch (Exception ex) when (ex.Message.Contains("A .git repository already exists"))
            {
                throw new Exception(Resources.Resource1.Initrepodetected);

            }
            catch (Exception ex)
            {
                throw new Exception("An Initialization Error Occured: " + ex.Message);
                
            }
            try
            {
                // ... existing initialization logic ...

                // At the end, delete the 'master' branch file if it exists
                string masterBranchFile = Path.Combine(localRepoPath, ".git", "refs", "heads", "master");
                if (File.Exists(masterBranchFile))
                {
                    File.Delete(masterBranchFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.Resource1.MasterBranchRemovalFail + " " + ex.Message);
                
            }

            return true; // Return true if successful
        }




        public static bool git_clone(string localRepoPath, string gitRepoUrl)
        {
            // Function to clone a Git repository to a local path
            // Parameters:
            // localRepoPath: Path where the repository will be cloned
            // gitRepoUrl: URL of the remote Git repository to clone
            // Check for existing .git directory
            string gitDir = Path.Combine(localRepoPath, ".git");
            if (Directory.Exists(gitDir))
            {
                throw new Exception(Resources.Resource1.Clonerepodetected);

            }

            if (!(Directory.GetFileSystemEntries(localRepoPath).Length == 0))
            {
                throw new Exception(Resources.Resource1.Clonerepodetected);
                
            }
            else
            {
                try
                {
                    string clonedRepoPath = Repository.Clone(gitRepoUrl, localRepoPath, new CloneOptions() { FetchOptions = { CredentialsProvider = PrivateRepoCredentials } });

                    //also should pull all branches in remote repo


                    //EVENTUALLY SHOULD BE A REFRESH NOT A NEW OPEN
                    //var simprojFile = Directory.EnumerateFiles(localRepoPath, "*.simproj", SearchOption.TopDirectoryOnly).FirstOrDefault(); // Gets the first match or null
                    //Process.Start(new ProcessStartInfo(simprojFile) { UseShellExecute = true });
                    
                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

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
                    throw new Exception($"An error occurred while cloning the repository: {ex.Message}");
                    

                }
                return true; // Return true if successful
            }
        }




        /*
         * Functions for core git functionality
         * members: pull, push, commit
         */




        public static bool git_pull(string localRepoPath, Signature signature)
        {
            // Function to pull changes from the remote repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // signature: Signature object containing author and committer information


            using (var repo = new Repository(localRepoPath))
            {
                LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                options.FetchOptions = new FetchOptions();
                options.FetchOptions.CredentialsProvider = PrivateRepoCredentials;
                try
                {
                    Commands.Pull(repo, signature, options);
                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("403"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("404"))
                {
                    throw new Exception(Resources.Resource1.RemoteRetrival);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error pulling from remote: " + e.Message);
                    return false; // Return false if an error occurs
                }
                //new PullOptions() { FetchOptions = { CredentialsProvider = PrivateRepoCredentials } }
                //finds file and opens it
                //var simprojFile = Directory.EnumerateFiles(localRepoPath, "*.simproj", SearchOption.TopDirectoryOnly).FirstOrDefault(); // Gets the first match or null
                //Process.Start(new ProcessStartInfo(simprojFile) { UseShellExecute = true });
                //Console.WriteLine($"Attempting to open '{simprojFile}' with the default application.");
                return true; // Return true if successful

            }
        }




        public static bool git_commit(string localRepoPath, string commitMessage, Signature signature)
        {
            // Function to commit changes in a local Git repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // commitMessage: Message for the commit
            // signature: Signature object containing author and committer information

            using (var repo = new Repository(localRepoPath))
            {
                // Stage all changes
                Commands.Stage(repo, "*");
                // Create the commit
                try
                {
                    var commit = repo.Commit(commitMessage, signature, signature);
                    // Optionally, you can log the commit SHA or other details
                    //Console.WriteLine($"Commit created: {commit.Sha}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while committing changes: {ex.Message}");
                    
                }
            }
            return true; // Return true if successful
        }




        public static bool git_push(string localRepoPath)
        {
            // Function to push changes to the remote repository
            // Parameters:
            // localRepoPath: Path to the local Git repository

            using (var repo = new Repository(localRepoPath))
            {
                // Push changes to the remote repository
                try
                {
                    Remote remote = repo.Network.Remotes["origin"];

                    // Push to the remote repository
                    //FriendlyName gets the name of the current branch
                    var branchName = repo.Head.FriendlyName;
                    repo.Network.Push(remote, $"refs/heads/{branchName}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });

                }
                catch (Exception e)
                {
                    throw new Exception("Error pushing to remote: " + e.Message);

                }
            }
            return true; // Return true if successful
        }




        /*
         * Functions for modified core git functionality
         * members: force_push, safe_push, force_pull, safe_pull
         */




        public static bool git_safe_push(string localRepoPath, Signature signature)
        {
            // Function to push changes to the remote repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            if (git_dirty(localRepoPath))
            {
                throw new Exception("There are uncommitted changes in the repository.");
                
            }
            if (!git_safe_pull(localRepoPath, signature))
            {
                //This will essentially make sure you are ahead of the remote repo and tehre are no conflicts before safe push allows a push
                throw new Exception(Resources.Resource1.GitSafePullErrorDuringSafePush);
                
                //future should offer to overwrite local, remote or abort the operation
            }
            using (var repo = new Repository(localRepoPath))
            {
                // Push changes to the remote repository
                try
                {
                    Remote remote = repo.Network.Remotes["origin"];

                    // Push to the remote repository
                    //FriendlyName gets the name of the current branch
                    var branchName = repo.Head.FriendlyName;
                    repo.Network.Push(remote, $"refs/heads/{branchName}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });

                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("403"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("404"))
                {
                    throw new Exception(Resources.Resource1.RemoteRetrival);

                }
                catch (Exception e)
                {
                    throw new Exception("Error pushing to remote: " + e.Message);

                }
            }
            return true; // Return true if successful
        }




        public static bool git_safe_pull(string localRepoPath, Signature signature)
        {
            // Function to safely pull changes from the remote repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // signature: Signature object containing author and committer information

            /*
             * The point of this function is to safely pull changes from the remote repository
             * It does this by only trying a fast-forward merge, which means it will not merge if there are conflicts between the remote and local branches.
             * If a merge conflict is detected, it will reset the local branch to the last commit before the merge attempt.
             * I believe it autocommits local changes before merging or something because I am not loosing uncommited changes after running this with conflicts but I need to test this further
             */
            using (var repo = new Repository(localRepoPath))
            {
                LibGit2Sharp.FetchOptions options = new LibGit2Sharp.FetchOptions();
                options.CredentialsProvider = PrivateRepoCredentials;
                try
                {
                    var remote = repo.Network.Remotes["origin"];
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repo, remote.Name, refSpecs, options, "");


                    LibGit2Sharp.MergeOptions mergeOptions = new LibGit2Sharp.MergeOptions
                    {
                        FastForwardStrategy = FastForwardStrategy.FastForwardOnly // Use the default fast-forward strategy
                    };

                    var localBranch = repo.Head;
                    var remoteBranch = repo.Branches[$"origin/{localBranch.FriendlyName}"];
                    if (remoteBranch == null)
                    {

                        throw new Exception($"Remote branch origin/{localBranch.FriendlyName} not found.");

                    }

                    var mergeResult = repo.Merge(remoteBranch, signature, mergeOptions);

                    if (mergeResult.Status == MergeStatus.Conflicts)
                    {

                        repo.Reset(ResetMode.Hard, repo.Head.Tip);
                        throw new Exception(Resources.Resource1.MergeConflicts);

                        // this will reset to the last commit before the merge attempt

                    }
                    if (mergeResult.Status == MergeStatus.UpToDate)
                    {

                        //MessageBox.Show(Resources.Resource1.AlreadyUpToDate, "Git Pull Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true; // Return true if already up to date
                    }


                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

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
                    throw new Exception("Error fetching/merging from remote: " + ex.Message);

                }
                //new PullOptions() { FetchOptions = { CredentialsProvider = PrivateRepoCredentials } }
                //finds file and opens it
                //var simprojFile = Directory.EnumerateFiles(localRepoPath, "*.simproj", SearchOption.TopDirectoryOnly).FirstOrDefault(); // Gets the first match or null
                //Process.Start(new ProcessStartInfo(simprojFile) { UseShellExecute = true });
                //Console.WriteLine($"Attempting to open '{simprojFile}' with the default application.");
                return true; // Return true if successful

            }
        }




        public static bool git_push_force(string localRepoPath)
        {
            // Function to force push changes to the remote repository
            // Parameters:
            // localRepoPath: Path to the local Git repository

            //NEEDS TESTINNG

            using (var repo = new Repository(localRepoPath))
            {
                var remote = repo.Network.Remotes["origin"];


                // Use +refs/heads/branchName to force push
                string branchName = repo.Head.FriendlyName;
                string refSpec = $"+refs/heads/{branchName}";
                try
                {
                    repo.Network.Push(remote, refSpec, new PushOptions { CredentialsProvider = PrivateRepoCredentials });
                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("403"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("404"))
                {
                    throw new Exception(Resources.Resource1.RemoteRetrival);
                }
                catch (Exception e)
                {
                    throw new Exception("Error pushing to remote: " + e.Message);

                }
                return true; // Return true if successful
            }

        }




        public static bool git_force_pull(string localRepoPath, Signature signature)
        {
            // Function to force pull changes from the remote repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // signature: Signature object containing author and committer information

            //needs some testing
            using (var repo = new Repository(localRepoPath))
            {
                try
                {
                    // Fetch the latest changes from the remote repository
                    var remote = repo.Network.Remotes["origin"];
                    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                    Commands.Fetch(repo, remote.Name, refSpecs, new FetchOptions { CredentialsProvider = PrivateRepoCredentials }, "");
                    // Reset the local branch to match the remote branch
                    var branchName = repo.Head.FriendlyName;
                    var remoteBranch = repo.Branches[$"origin/{branchName}"];
                    if (remoteBranch == null)
                    {
                        throw new Exception($"Remote branch origin/{branchName} not found.");

                    }
                    // Reset the local branch to the state of the remote branch
                    repo.Reset(ResetMode.Hard, remoteBranch.Tip);
                    
                    MessageBox.Show($"Local branch '{branchName}' has been forcefully updated to match the remote branch.", "Git Force Pull Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("403"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("404"))
                {
                    throw new Exception(Resources.Resource1.RemoteRetrival);
                }
                catch (Exception e)
                {
                    throw new Exception("Error during force pull: " + e.Message);

                }
            }
            return true; // Return true if successful
        }




        /*
         * Functions for git branching
         * members: git_delete_branch, git_create_branch, git_checkout_branch, git_merge_branch
         */


        public static bool git_create_branch(string localRepoPath, string branchName, Signature signature)
        {
            // Function to create a new branch in the local repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // branchName: Name of the new branch to create
            // signature: Signature object containing author and committer information
            try
            {
                using (var repo = new Repository(localRepoPath))
                {
                    // Check if the branch already exists
                    if (repo.Branches[branchName] != null)
                    {
                        throw new Exception($"Branch '{branchName}' already exists.");

                    }
                    // Create a new branch
                    var newBranch = repo.CreateBranch(branchName);
                    Commands.Checkout(repo, newBranch);
                    // Push the new branch to the remote repository
                    repo.Network.Push(repo.Network.Remotes["origin"], $"refs/heads/{branchName}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });
                }
            }
            catch (Exception ex) when (ex.Message.Contains("authentication replays"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

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
                throw new Exception($"An error occurred while creating the branch: {ex.Message}");

            }
            return true; // Return true if successful
        }




        public static bool git_delete_branch(string localRepoPath, String branchName)
        {
            // Function to delete a branch both locally and remotely
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // branchName: Name of the branch to delete
            try
            {
                using (var repo = new Repository(localRepoPath))
                {
                    //test if local repo has this branch checkedout already because they need to be in a different branch to do this
                    if (repo.Head.FriendlyName == branchName)
                    {
                        throw new Exception($"Cannot delete branch '{branchName}' because it is currently checked out. Please checkout a different branch first.");

                    }


                    //check if branch exists remotely
                    // Check if the branch exists in remote repository
                    Branch remoteBranch = repo.Branches[$"origin/{branchName}"];
                    if (remoteBranch == null)
                    {
                        throw new Exception($"Remote branch '{branchName}' does not exist.");

                    }

                    //Delete branch locally
                    repo.Branches.Remove(branchName);

                    //Delete branch remotely
                    repo.Network.Push(repo.Network.Remotes["origin"], $":refs/heads/{branchName}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });
                }
            }
            catch (Exception ex) when (ex.Message.Contains("authentication replays"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

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
                throw new Exception($"An error occurred while deleting the branch: {ex.Message}");


            }
            return true; // Return true if successful
        }




        public static bool git_checkout_branch(string localRepoPath, string branchName)
        {
            // Function to checkout a branch in the local repository
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // branchName: Name of the branch to checkout
            try
            {
                if (branchName.StartsWith("origin/"))
                {
                    branchName = branchName.Substring("origin/".Length);
                }
                using (var repo = new Repository(localRepoPath))
                {
                    // Check if the branch exists in remote repository
                    Branch remoteBranch = repo.Branches[$"origin/{branchName}"];
                    if (remoteBranch == null)
                    {
                        //throw new Exception($"Remote branch '{branchName}' does not exist.");
                        return false;
                    }

                    //initializing local branch so it is not local to the if statement
                    Branch localBranch;

                    if (repo.Branches[branchName] == null)
                    {
                        //local branch does not exist so create it
                        localBranch = repo.CreateBranch(branchName, remoteBranch.Tip);
                    }
                    else
                    {
                        localBranch = repo.Branches[branchName];
                    }

                    //set up tracking
                    repo.Branches.Update(localBranch, b => b.UpstreamBranch = $"refs/heads/{branchName}");



                    //HANDLE CHECKOUT ERROR
                    //SystemDirectoryHandler.PreemptiveDeleteRemovedDirs(repo, branchName);
                    /*
                     * This function manually deletes directories that are present in current branch but not in the one you are going to.
                     * Only use if totally necessary as it is a bit dangerous as it can delete read only files
                     */





                    // Define the checkout options
                    var checkoutOptions = new CheckoutOptions
                    {
                        // It forces the checkout to overwrite and delete files as needed.
                        CheckoutModifiers = CheckoutModifiers.Force
                    };

                    // Checkout the branch
                    Commands.Checkout(repo, branchName, checkoutOptions);
                }
            }
            catch (Exception ex) when (ex.Message.Contains("authentication replays"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

            }
            catch (Exception ex) when (ex.Message.Contains("failed rmdir"))
            {
                throw new Exception(ex.Message);
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
                //throw error
                throw new Exception($"An error occurred while checking out the branch: {ex.Message}");

            }
            return true; // Return true if successful
        }




        //overwrite branch with another branch ie git_merge_branch

        public static bool git_branch_merge_force(string repoPath, string branchToOverwrite, string sourceBranch = "")
        {
            // Function to overwrite a branch with another branch
            // Parameters:
            // repoPath: Path to the local Git repository
            // branchToOverwrite: Name of the branch to be overwritten say the main branch in the case of a dev branch being merged onto the main branch
            // sourceBranch: Name of the branch to overwrite with, say the dev branch in the case of a dev branch being merged onto the main branch

            /*FLOW FOR USE OF THIS FUNCTION
                                               *
                                               |\  <-------git_branch_merge_force(repoPath, branchToOverwrite, sourceBranch)
                                               | \
                         branchToOverwrite >-> 0  * <-< SourceBranch
                                           |   |  |   |
                                           |-> 0  * <-|
                                           |   | /
                                           |   |/  <------- git_create_branch(repoPath, branchName)
                                           |-> 0
             */

            try
            {
                using (var repo = new Repository(repoPath))
                {

                    if (string.IsNullOrWhiteSpace(sourceBranch))
                    {
                        // If no source branch is specified, use the current HEAD branch
                        sourceBranch = repo.Head.FriendlyName;
                    }


                    // 1. Get the branches
                    var branch1 = repo.Branches[branchToOverwrite];
                    var branch2 = repo.Branches[sourceBranch];

                    //check if the source branch actually exist remotly
                    Branch remoteSourceBranch = repo.Branches[$"origin/{sourceBranch}"];
                    if (remoteSourceBranch == null)
                    {
                        throw new Exception($"Remote branch '{sourceBranch}' does not exist.");

                    }
                    Branch remoteBranchToOverwrite = repo.Branches[$"origin/{branchToOverwrite}"];
                    if (remoteBranchToOverwrite == null)
                    {
                        throw new Exception($"Remote branch '{branchToOverwrite}' does not exist.");

                    }

                    //makes sure we have a local version of the existing branches of the overwrite branch
                    Branch localBranchToOverwrite;
                    if (repo.Branches[branchToOverwrite] == null)
                    {
                        //local branch does not exist so create it
                        localBranchToOverwrite = repo.CreateBranch(branchToOverwrite, remoteBranchToOverwrite.Tip);
                    }
                    else
                    {
                        localBranchToOverwrite = repo.Branches[branchToOverwrite];
                    }


                    //makes sure we have a local version of the existing branches of the source branch
                    Branch localSourceBranch;
                    if (repo.Branches[sourceBranch] == null)
                    {
                        //local branch does not exist so create it
                        localSourceBranch = repo.CreateBranch(sourceBranch, remoteSourceBranch.Tip);
                    }
                    else
                    {
                        localSourceBranch = repo.Branches[sourceBranch];
                    }

                    // 2. Move branch1 to branch2's tip
                    Reference branch1Ref = repo.Refs[branch1.CanonicalName];
                    repo.Refs.UpdateTarget(branch1Ref, branch2.Tip.Id);
                        
                    // 3. Push branch1 to remote (force)
                    var remote = repo.Network.Remotes["origin"];
                    string refSpec = $"+refs/heads/{branchToOverwrite}";
                    repo.Network.Push(remote, refSpec, new PushOptions { CredentialsProvider = PrivateRepoCredentials });

                    git_checkout_branch(repoPath, branchToOverwrite); // Checkout the overwritten branch to ensure it is the current branch

                    // TODO : Unsure if they should be deleted or not
                    // 4. Delete branch2 locally
                    repo.Branches.Remove(sourceBranch);

                    try
                    {
                        //attempt to delete the local version of the branch
                        SystemDirectoryHandler.DeleteFileInGitRepository(GitContext.Instance.RepositoryPath, sourceBranch);
                    } catch (Exception e) {
                        //if it fails to delete the local branch then just ignore it
                        throw new Exception($"Failed to delete local branch '{sourceBranch}': {e.Message}");
                    }

                    // 5. Delete branch2 remotely
                    repo.Network.Push(remote, $":refs/heads/{sourceBranch}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });
                    
                    MessageBox.Show($"Branch '{branchToOverwrite}' has been forcefully updated to match '{sourceBranch}'.", "Git Branch Merge Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) when (ex.Message.Contains("authentication replays"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

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
                throw new Exception($"An error occurred: {ex.Message}");
            }
            return true;
        }




        /*
         * Functions to get git status likley not in UI but rather for internal checks and troubleshooting
         * members: git_dirty, git_status
         */




        public static bool git_dirty(string localRepoPath)
        {
            // Function to check if there are uncommitted changes in the local repository
            // Parameters:
            // localRepoPath: Path to the local Git repository

            //NEEDS TO ME TESTED MORE

            using (var repo = new Repository(localRepoPath))
            {
                // Check if there are any changes in the repository
                var status = repo.RetrieveStatus();
                if (status.IsDirty)
                {

                    return true; // Return true if there are uncommitted changes
                }
                else
                {

                    return false; // Return false if there are no uncommitted changes
                }
            }
        }


        public static string git_current_branch(String RepositoryPath)
        {
            // Function to get the current branch name of the local Git repository
            // Parameters:
            // RepositoryPath: Path to the local Git repository
            using (var repo = new Repository(RepositoryPath))
            {
                // Return the friendly name of the current branch
                return repo.Head.FriendlyName;
            }
        }


        public static void git_status(string localRepoPath)
        {
            // Function to retrieve and print the status of the local Git repository
            // Parameters:
            // localRepoPath: Path to the local Git repository

            //NEEDS SOME TESTING
            //ALSO LIKLEY WONT BE IN FILE TOOL MORE FOR VISUALLY DEBUGGING
            using (var repo = new Repository(localRepoPath))
            {
                // Retrieve the status of the repository
                var status = repo.RetrieveStatus();
                // Print the status to the console
                Console.WriteLine("Repository Status:");
                foreach (var entry in status)
                {
                    Console.WriteLine($"{entry.FilePath}: {entry.State}");
                }
            }
        }


        public static bool git_main_branch_check(string localRepoPath)
        {
            using (var repo = new Repository(localRepoPath))
            {
                return repo.Head.FriendlyName == "main" || repo.Head.FriendlyName == "master" || repo.Head.FriendlyName == "Main" || repo.Head.FriendlyName == "Master";
            }
        }

        public static List<string> git_get_branches(string localRepoPath)
        {
            using(var repo = new Repository(localRepoPath))
            {
                // Retrieve the list of branches in the repository
                var branches = repo.Branches.Select(b => b.FriendlyName).ToList();
                //remove all strings that contain "origin/" if they have one without origin/ in front otherwise dont get rid

                var branchSet = new HashSet<string>(branches);


                // This modifies the 'branches' list directly.
                branches.RemoveAll(branchName => branchName.StartsWith("origin/") && branchSet.Contains(branchName.Substring("origin/".Length)));
                //removes all fetched branches
                branches.RemoveAll(branchName => branchName.Contains("origin/HEAD"));



                return branches; // Return the list of branch names
            }
        }



        /*
         * Functions for reverting changes
         * members: git_checkout_SHA, git_reset_local
         */




        public static bool git_checkout_SHA(string repoPath, string historicalCommitSha, String newBranchName)
        {
            // Function to checkout a historical commit and create a new branch from it
            // Parameters:
            // repoPath: Path to the local Git repository
            // historicalCommitSha: SHA of the historical commit to checkout
            // newBranchName: Name of the new branch to create from the historical commit

            try
            {
                using (var repo = new Repository(repoPath))
                {
                    var currBranch = repo.Branches[repo.Head.FriendlyName];//gets the current branch via friendly name returning the name of the branch which is used by repo.Branches to return a branch
                    var branchTip = currBranch.Tip; // Gets the tip commit of the current branch
                    Console.WriteLine($"Current branch: {currBranch.FriendlyName}");
                    // 1. Get the historical commit whose state (tree) we want to use 
                    LibGit2Sharp.Commit historicalCommit = repo.Lookup<LibGit2Sharp.Commit>(historicalCommitSha);
                    if (historicalCommit == null)
                    {
                        throw new Exception($"Historical commit with SHA '{historicalCommitSha}' not found.");

                    }
                    // 2. Checkout the historical commit
                    //Commands.Checkout(repo, historicalCommit);
                    var newBranch = repo.CreateBranch(newBranchName, historicalCommit);
                    var remote = repo.Network.Remotes["origin"];
                    repo.Network.Push(remote, $"refs/heads/{newBranchName}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });


                }
            }
            catch (Exception ex) when (ex.Message.Contains("authentication replays"))
            {
                throw new Exception(Resources.Resource1.AuthenticationError);

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
                throw new Exception($"An error occurred while checking out the historical commit: {ex.Message}");

            }
            return true; // Return true if successful
        }




        public static bool git_reset_local(string localRepoPath)
        {
            // Function to reset the local repository to the last commit
            // Parameters:
            // localRepoPath: Path to the local Git repository
            using (var repo = new Repository(localRepoPath))
            {
                // Reset the repository to the last commit
                try
                {
                    repo.Reset(ResetMode.Hard, repo.Head.Tip);
                    MessageBox.Show("Local repository has been reset to the last commit.", "Git Reset Status", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex) when (ex.Message.Contains("authentication replays"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("403"))
                {
                    throw new Exception(Resources.Resource1.AuthenticationError);

                }
                catch (Exception ex) when (ex.Message.Contains("404"))
                {
                    throw new Exception(Resources.Resource1.RemoteRetrival);
                }
                catch (Exception e)
                {
                    throw new Exception("Error resetting local repository: " + e.Message);

                }
                return true; // Return true if successful
            }
        }


        //sort of just a force_pull not sure if still needs to be implimetned
        /*
         * TODO : Priority : 2 NEED TO CREATE A FORCE PULL FIRST FOR THIS AS THEY ARE ESSENTIALLY THE SAME FUNCTION
        public static void git_reset_remote(string localRepoPath, string branchName)
        {
            // Function to reset the remote branch to the last commit
            // Parameters:
            // localRepoPath: Path to the local Git repository
            // branchName: Name of the branch to reset on the remote
            using (var repo = new Repository(localRepoPath))
            {
                try
                {
                    var remote = repo.Network.Remotes["origin"];
                    // Reset the remote branch to the last commit
                    repo.Network.Push(remote, $":refs/heads/{branchName}", new PushOptions { CredentialsProvider = PrivateRepoCredentials });
                    Console.WriteLine($"Remote branch '{branchName}' has been reset to the last commit.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error resetting remote repository: " + e.Message);
                }
            }
        }
        */

        //Create branch
        /* TODO : Priority : 3 
         * CREATE BRANCH
         * check if name is in use
         * if yes than false
         * else:
         * create local branch
         * push to remote
         * checkout branch
         * not fully implimented yet
         * not sent to remote yet
        public static bool git_create_branch(string localRepoPath, string branchName)
        {
            using (var repo = new Repository(localRepoPath))
            {
                // Check if the branch already exists
                if (repo.Branches[branchName] != null)
                {
                    Console.WriteLine($"Branch '{branchName}' already exists.");
                    return false; // Return false if the branch already exists
                }
                // Create a new branch
                var newBranch = repo.CreateBranch(branchName);
                // Checkout the new branch
                Commands.Checkout(repo, newBranch);
                return true; // Return true if successful
            }
        }
        */


        //IM OKAY WITH THIS FUNCTION MINUS THE PAT HANDLING WHICH SHOULD HAS A SEPERATE FUNCTION TO STORE SECURLY
        //SHOULD ALSO BE WRITABLE




        /*
         * Functions to handle credential handshaking eventually needs some encryption and secure storage
         * members: PrivateRepoCredentials, getPAT, setPAT, getUser, setUser
         */




        public static Credentials PrivateRepoCredentials(string url, string usernameFromUrl,
                                                         SupportedCredentialTypes types)
        {
            // Function to provide credentials for accessing a private repository
            // Parameters: NOT BEING USED AT THE MOMENT
            // url: URL of the repository
            // usernameFromUrl: Username extracted from the URL (if any)
            // types: Supported credential types

            return new UsernamePasswordCredentials { Username = getUser(), Password = getPAT() };
        }




        public static string getPAT()
        {
            // Function to retrieve the Personal Access Token (PAT)
            // Returns: The PAT as a string
            return GitContext.Instance.PAT;
        }






        public static string getUser()
        {
            // Function to retrieve the username
            // Returns: The username as a string

            return GitContext.Instance.UserName;
        }








    }
}

