using System;
using System.Collections.Generic;
using LibGit2Sharp; // Ensure you have a 'using' for this library

namespace GitFlow
{
    /// <summary>
    /// A thread-safe singleton class to hold the context for Git operations.
    /// Access the single instance via the static 'Instance' property.
    /// </summary>
    public sealed class GitContext
    {
        #region Singleton Implementation

        // Use Lazy<T> for a simple, thread-safe, and performant singleton implementation.
        private static readonly Lazy<GitContext> lazyInstance =
            new Lazy<GitContext>(() => new GitContext());

        /// <summary>
        /// Gets the single, shared instance of the GitContext.
        /// </summary>
        public static GitContext Instance => lazyInstance.Value;

        // A private constructor is essential to prevent creating new instances from outside.
        private GitContext() { }

        #endregion

        #region Properties

        // Properties have private setters to ensure they are only modified from within this class,
        // primarily through the Initialize method. This prevents accidental changes.
        public bool IsInitialized { get; private set; } = false;
        public string RepositoryPath { get; private set; }
        public string RemoteUrl { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string PAT { get; private set; }

        public int PermissionLevel { get; set; } = 0; // Default to no access

        public SimioAPI.Extensions.IDesignContext simioContext { get; set; }

        /// <summary>
        /// The current working branch. Can be updated using 'SetCurrentBranch'.
        /// </summary>
        public string CurrentBranch { get; private set; }

        /// <summary>
        /// A read-only view of the available branches. Updated via 'SetAvailableBranches'.
        /// </summary>
        public IReadOnlyList<string> Branches { get; private set; } = new List<string>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the core configuration for the GitContext. This should be called once
        /// at application startup.
        /// </summary>
        /// <param name="path">The local file path to the repository.</param>
        /// <param name="remoteUrl">The URL of the remote repository.</param>
        /// <param name="pat">The Personal Access Token for authentication.</param>
        /// <param name="userName">The username for commits.</param>
        /// <param name="email">The email address for commits.</param>
        public void Initialize(string path, string remoteUrl, string pat, string userName, string email)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "DefaultUser"; // Default username if not provided
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                email = "DefaultSimioEmail@simioemail.com"; // Default username if not provided
            }

            RepositoryPath = path;
            RemoteUrl = remoteUrl;
            PAT = pat;
            UserName = userName;
            Email = email;
            IsInitialized = true;
        }

        /// <summary>
        /// Creates a Signature for committing, using the configured UserName and Email.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the context has not been initialized.</exception>
        public Signature GetSignature()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new InvalidOperationException("GitContext is not initialized. Initialize before getting a signature.");
            }
            return new Signature(UserName, Email, DateTimeOffset.Now);
        }

        /// <summary>
        /// Sets the current working branch.
        /// </summary>
        public void SetCurrentBranch(string branchName)
        {
            CurrentBranch = branchName;
        }

        /// <summary>
        /// Updates the list of known branches.
        /// </summary>
        public void SetAvailableBranches(List<string> branches)
        {
            Branches = branches ?? new List<string>();
            //Branches = branches or null list
        }


        #endregion
    }
}