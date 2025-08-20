using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Windows.Forms;

namespace GitFlow
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Security.Policy;
    using DevExpress.DataProcessing.InMemoryDataProcessor;
    using LibGit2Sharp;
    using SimioAPI;
    using SimioAPI.Extensions;

    internal class SystemDirectoryHandler
    {

        public static string GetTargetFrameworkForExecutable(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                return $"Please provide the path to the assembly. {assemblyPath} doesn't work.";
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                var targetFrameworkAttribute = assembly
                    .GetCustomAttribute<TargetFrameworkAttribute>();

                if (targetFrameworkAttribute != null)
                {
                    return $"{targetFrameworkAttribute.FrameworkName}";
                }
                else
                {
                    return $"?Target Framework attribute not found: {assemblyPath}";
                }
            }
            catch (Exception ex)
            {
                return $"?Exception:{ex.Message}";
            }
        }




        /// <summary>
        /// Prompt the user to select a parent folder for the repository.
        /// </summary>
        /// <param name="defaultPath"></param>
        /// <returns></returns>
        public static string PromptForFolder(string defaultPath)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the parent folder under which you want to clone the repository";
                folderDialog.SelectedPath = defaultPath;
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    return folderDialog.SelectedPath;
                }
            }

            return null;
        }



        /// <summary>
        /// Get the last folder, which is the project folder.
        /// Returns an empty string if the project folder is not found.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetLastFolderName(string path)
        {
            // Ensure the path is not null or empty
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            // Normalize the path to avoid issues with different directory separators
            string normalizedPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Get the name of the last directory
            string lastFolderName = new DirectoryInfo(normalizedPath).Name;

            return lastFolderName;
        }

        /// <summary>
        /// Get the second to last folder, which is the project parent folder.
        /// Returns an empty string if the parent folder is not found.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetSecondToLastFolderName(string path)
        {
            // Ensure the path is not null or empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Normalize the path to avoid issues with different directory separators
            string normalizedPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Get the directory information
            DirectoryInfo directoryInfo = new DirectoryInfo(normalizedPath);

            // If the path points to a file, get the parent directory
            if (File.Exists(normalizedPath))
            {
                directoryInfo = directoryInfo.Parent;
            }

            // Get the parent directory (second-to-last folder)
            DirectoryInfo parentDirectory = directoryInfo?.Parent;

            // If there is no parent directory, return null or handle the situation as needed
            if (parentDirectory == null)
            {
                return string.Empty; // "The path does not have a second-to-last folder.");
            }

            return parentDirectory.Name;
        }
        /// <summary>
        /// Get the parent folder, which is the path holding the project folder.
        /// The fullProjectPath could be a file or a folder.
        /// Examples: c:\a\b\c\
        /// Returns an empty string if the parent folder is not found.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetProjectFolderParentPath(string fullProjectPath)
        {
            // Ensure the path is not null or empty
            if (string.IsNullOrEmpty(fullProjectPath))
                return string.Empty;

            // Normalize the path to avoid issues with different directory separators
            string normalizedPath = Path.GetFullPath(fullProjectPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Get the directory information
            DirectoryInfo directoryInfo = new DirectoryInfo(normalizedPath);

            // If the path points to a file, get the parent directory
            if (File.Exists(normalizedPath))
            {
                directoryInfo = directoryInfo.Parent;
            }

            // Get the parent directory (second-to-last folder)
            DirectoryInfo parentDirectory = directoryInfo?.Parent;

            // If there is no parent directory, return null or handle the situation as needed
            if (parentDirectory == null)
            {
                return string.Empty; // "The path does not have a second-to-last folder.");
            }

            return parentDirectory.Name;
        }

        /// <summary>
        /// Returns the full path to the parent of the project path, which might be a folder or a file. 
        /// E.g. c:\x\y\parent\foo\foo.simproj or c:\x\y\parent\foo should both return c:\x\y\parent
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        public static string GetParentFolderFullPath(string projectPath)
        {
            // Ensure the path is not null or empty
            if (string.IsNullOrEmpty(projectPath))
            {
                return string.Empty; // "Path cannot be null or empty", nameof(projectPath));
            }

            // Normalize the path to avoid issues with different directory separators
            string normalizedPath = Path.GetFullPath(projectPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Get the directory information
            DirectoryInfo directoryInfo;

            if (Directory.Exists(normalizedPath))
            {
                // If the path is a directory
                directoryInfo = new DirectoryInfo(normalizedPath);
            }
            else if (File.Exists(normalizedPath))
            {
                // If the path is a file, get the parent directory
                directoryInfo = new FileInfo(normalizedPath).Directory;
            }
            else
            {
                return string.Empty; // "The specified path does not exist.");
            }

            // Get the parent directory
            DirectoryInfo parentDirectory = directoryInfo?.Parent;

            // If there is no parent directory, return null or handle the situation as needed
            if (parentDirectory == null)
            {
                return string.Empty; // "The path does not have a parent directory.");
            }

            return parentDirectory.FullName;
        }



        /// <summary>
        /// Use reflection to get a string value for a property named "propertyName".
        /// The name must be an exact match, and known to be string.
        /// If not found, a blank string is returned.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static string GetStringProperty(Object obj, string propertyName)
        {
            try
            {
                foreach (PropertyInfo pi in obj.GetType().GetProperties())
                {
                    var getter = pi.GetGetMethod();

                        if (pi.Name == propertyName)
                        {
                            var vv = pi.GetValue(obj, null);
                            return (vv ?? "").ToString();
                        }
                    

                }
                return string.Empty;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool Refresh()
        {
            try
            {
                string projectFileNameWithExtSimproj = GitContext.Instance.simioContext.ActiveProject.Name + ".simproj";
                string fullPath = Path.Combine(GitContext.Instance.RepositoryPath, projectFileNameWithExtSimproj);

                if(!File.Exists(fullPath))
                {
                    return false;
                }

                GitContext.Instance.simioContext.ExecuteUICommand("CloseProject", fullPath);
                return true;

            }
            catch (Exception)
            {
                MessageBox.Show(Resources.Resource1.RefreshFail, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false; // Return 1 to indicate failure
        }

        public static void DeleteFileInGitRepository(string repositoryPath, string fileNameToDelete)
        {
            // Construct the path to the .git directory
            string gitDirectory = Path.Combine(repositoryPath, ".git");

            // Check if the .git directory exists before proceeding
            if (Directory.Exists(gitDirectory))
            {
                // Call the recursive function to delete the specified file
                DeleteFileRecursively(gitDirectory, fileNameToDelete);
            }
            else
            {
                // Optionally handle the case where the .git directory doesn't exist
                // For example, you could log a message or throw an exception.
                MessageBox.Show($"The .git directory was not found in: {repositoryPath}");
            }
        }


        public static void DeleteFileRecursively(string directory, string fileNameToDelete)
        {
            // Recursively call this function for all subdirectories
            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteFileRecursively(subdirectory, fileNameToDelete);
            }

            // Search for and delete the specified file in the current directory
            foreach (var filePath in Directory.EnumerateFiles(directory))
            {
                // Check if the current file's name matches the one to delete
                if (Path.GetFileName(filePath).Equals(fileNameToDelete, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var fileInfo = new FileInfo(filePath);
                        // Remove the read-only attribute if it exists
                        if (fileInfo.IsReadOnly)
                        {
                            fileInfo.IsReadOnly = false;
                        }
                        fileInfo.Delete();
                        System.Console.WriteLine($"Deleted: {filePath}");
                    }
                    catch (IOException ex)
                    {
                        // Handle potential errors, such as the file being in use
                        System.Console.WriteLine($"Could not delete file: {filePath}. Reason: {ex.Message}");
                    }
                }
            }
            // Note: The directory itself is not deleted in this version, only the specified files.
        }







        /*
         * 
         * FUNCTIONS TO TRY AND FIC THE GIT CHECKOUT ERROR OF NOT BEING ABLE TO DELETE FOLDERS
         * 
         */



        public static void PreemptiveDeleteRemovedDirs(Repository repo, string targetBranchName)
        {
            /// <summary>
            /// Compares the current HEAD to a target branch, finds all directories that will be removed,
            /// and forcefully deletes them from the working directory.
            /// </summary>
            /// <param name="repo">An open Repository object.</param>
            /// <param name="targetBranchName">The name of the branch you are about to check out.</param>
            // 1. Get the commit for the "from" (current) and "to" (target) branches.
            try
            {
                var fromTree = repo.Head.Tip.Tree;
                var toBranch = repo.Branches[targetBranchName];
                if (toBranch == null)
                {
                    // If the local branch doesn't exist, it might be a remote one.
                    // For redundancy check again but for the branch in the origin
                    toBranch = repo.Branches[$"origin/{targetBranchName}"];
                    if (toBranch == null)
                    {
                        // Cannot find the target branch to compare against.
                        return;
                    }
                }
                var toTree = toBranch.Tip.Tree;

                // 2. Get a complete list of all directory paths for each tree.
                var fromDirs = GetAllDirectoryPaths(fromTree);
                var toDirs = GetAllDirectoryPaths(toTree);

                // 3. Find which directories are in the "from" set but NOT in the "to" set.
                var dirsToDelete = fromDirs.Except(toDirs);

                // 4. Order by longest path first to delete subdirectories before parent directories.
                // e.g., delete 'a/b/c' before attempting to delete 'a/b'.
                foreach (var dirPath in dirsToDelete.OrderByDescending(p => p.Length))
                {
                    string fullPath = Path.Combine(repo.Info.WorkingDirectory, dirPath);

                    if (Directory.Exists(fullPath))
                    {
                        ForceDeleteDirectory(fullPath); // Use the helper to handle read-only files.
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed.
                throw new Exception($"Error during preemptive delete: {ex.Message}");
            }
        }
        /// <summary>
        /// Recursively finds all directory paths within a given Git Tree.
        /// </summary>
        private static HashSet<string> GetAllDirectoryPaths(Tree tree, string currentPath = "")
        {
            var paths = new HashSet<string>();
            foreach (var entry in tree)
            {
                if (entry.TargetType == TreeEntryTargetType.Tree)
                {
                    // This entry is a subdirectory.
                    string path = Path.Combine(currentPath, entry.Name);
                    paths.Add(path);
                    // Recurse into the subdirectory to find its children.
                    paths.UnionWith(GetAllDirectoryPaths((Tree)entry.Target, path));
                }
            }
            return paths;
        }

        /// <summary>
        /// Forcefully deletes a directory, even if it contains read-only files.
        /// </summary>
        public static void ForceDeleteDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            var directoryInfo = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }
            directoryInfo.Delete(true);
        }


        private static bool DeleteDirectoryRecursive(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            try
            {
                // First, remove read-only attributes from all files and subdirectories
                // This is crucial for successful deletion if files are marked read-only.
                foreach (string file in Directory.GetFiles(path))
                {
                    File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
                }

                foreach (string subDirectory in Directory.GetDirectories(path))
                {
                    DeleteDirectoryRecursive(subDirectory); // Recursive call for subdirectories
                }

                // Now that all contents are processed, delete the directory itself.
                // The 'true' argument ensures recursive deletion, though we've handled attributes manually.
                // It's good practice to keep it for robustness.
                Directory.Delete(path, true);
                if (Directory.Exists(path))
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
