# Using Git Version Control with Simio

## GitFlow Add-In Guide (V5.0)

---

## 1. Introduction

This guide teaches you how to use Git version control with your Simio simulation models using the GitFlow add-in. Everything happens through buttons in the Simio ribbon — you never need to use a command line or any external Git tools.

### Why Version Control for Simulation Models?

If you have ever named a file `Model_v3_final_FINAL.simproj`, version control is for you. Here are four everyday scenarios where Git makes your life easier:

**Track every model change.** You adjusted the server processing time from 5 to 8 minutes last Tuesday. With Git, that change has a message like "Increased server processing time to test bottleneck." Weeks later, you can look back and see exactly what changed and why — no guessing, no sticky notes.

**Collaborate without overwriting.** Two modelers working on the same facility layout. Without Git, you email `.simproj` files back and forth and hope nobody overwrites the other's work. With Git, each person works on their own branch and merges when ready.

**Undo mistakes safely.** You deleted a source and broke the model. Instead of Ctrl+Z forty-seven times, click one button to reset your model back to the last saved version.

**One project, one file, clean history.** Stop managing `Model_v1.simproj`, `Model_v2_fixed.simproj`, and `Model_FINAL_use_this_one.simproj`. One repo, one project file, and branches for experiments.

---

## 2. Prerequisites and Installation

### What You Need

- Simio installed (any edition that supports add-ins)
- A GitHub account (or Azure DevOps / Bitbucket — this guide uses GitHub as the primary example)
- An internet connection for pushing and pulling changes

### Installing GitFlow

1. Download the GitFlow release (a `.zip` file)
2. Extract the `GitFlow` folder from the zip
3. Copy the `GitFlow` folder to: `Documents\SimioUserExtensions\`
4. Restart Simio

When Simio opens, you should see a **"Version Control"** tab in the ribbon. If you do not see it, double-check that the `GitFlow` folder is directly inside `SimioUserExtensions` (not nested in an extra subfolder).

---

## 3. Key Git Concepts

Each concept below includes a brief definition and how it relates to your Simio workflow.

### Repository (Repo)

A folder that Git tracks. Your Simio project folder becomes a repository. It contains your `.simproj` file plus any data files, scripts, or other supporting files. Git also adds a hidden `.git` folder where it stores the change history.

### Commit

A snapshot of your entire project at a point in time, paired with a message describing what you changed. Think of it as a named save point. Example: *"Added second conveyor line to assembly area."*

### Push

Upload your commits from your computer to the shared server (GitHub, Azure DevOps, etc.) so your teammates can see them. In GitFlow, the **Commit/Push** button does both steps at once.

### Pull

Download your teammates' latest commits from the server and apply them to your local copy. This is how you stay in sync with the team.

### Branch

A parallel line of work where you can experiment without affecting the main version. The **main** branch is the official, stable version of your model. You create branches like `paul-new-layout` or `test-higher-arrival-rate` to try things out. When your experiment works, you merge it back into main.

### Merge (Promote)

Combine a branch's changes back into main. In GitFlow, this is the **Promote to Main** button. It takes everything you did on your branch and applies it to the main branch.

### Remote

The copy of your repository that lives on a server like GitHub. This is the shared hub that everyone on your team pushes to and pulls from. The remote URL looks like `https://github.com/your-org/your-repo.git`.

### .gitignore

A file that tells Git which files to skip. GitFlow creates one automatically when you initialize a repository. It excludes Simio temporary files (logs, backups, model view metadata) and build outputs so that only the important files are tracked.

### Personal Access Token (PAT)

A password-like key that lets GitFlow access your GitHub account on your behalf. You create it once on GitHub and GitFlow stores it securely. You do not need to enter it again for any repository on the same host.

---

## 4. The GitFlow Ribbon

After installing GitFlow, a **Version Control** tab appears in the Simio ribbon. It is organized into three groups:

| Group | Button | What It Does |
|---|---|---|
| **Repository Actions** | **Connect** | Connect your project to a Git repository (create new, clone existing, or reconnect) |
| **Git Actions** | **Commit/Push** | Save a snapshot of your changes and upload them to the server |
| **Git Actions** | **Pull** | Download the latest changes from the server |
| **Git Actions** | **Local Reset** | Revert your model to the last saved version (undo all uncommitted changes) |
| **Branch Actions** | **Create Branch** | Start a new branch for your changes (keeps main safe) |
| **Branch Actions** | **Select Branch** | Switch to a different branch |
| **Branch Actions** | **Promote to Main** | Merge your current branch into the main branch |
| **Branch Actions** | **Remove Branch** | Delete a branch you no longer need |

### Auto-Connect

You do not need to click **Connect** every time you open Simio. If you have previously connected and your credentials are saved, simply click any action button (like **Commit/Push** or **Pull**) and GitFlow connects automatically in the background using your stored credentials. A notification confirms the auto-connection.

---

## 5. Getting Started

There are three common scenarios for getting started. Pick the one that matches your situation.

### Scenario A: Starting Fresh (Your Project Is Not Yet in Git)

Use this path when you have a Simio project on your computer and you want to start tracking it with Git for the first time.

**Step 1 — Create a repository on GitHub**

1. Go to github.com and sign in
2. Click the **+** button in the top-right corner, then **New repository**
3. Give it a name (e.g., `my-simulation-model`)
4. Choose **Private** or **Public**
5. Do **not** add a README, `.gitignore`, or license (GitFlow handles the `.gitignore`)
6. Click **Create repository**
7. Copy the HTTPS URL (e.g., `https://github.com/your-org/my-simulation-model.git`)

**Step 2 — Save your Simio project**

1. Open your project in Simio
2. Use **File > Save As** and save it to a local folder (e.g., `C:\Projects\MyModel\`)
3. Important: Save as a `.simproj` file — this format works best with version control

**Step 3 — Connect in GitFlow**

1. Click the **Connect** button in the Version Control ribbon
2. The Connect dialog opens and auto-detects your open project's folder
3. Since no Git repository exists yet, you will see two options:
   - **Create a new repository here** (selected by default)
   - **Clone a repository into this folder**
4. Select **Create a new repository here**
5. Paste the GitHub remote URL you copied in Step 1
6. Enter your Personal Access Token (see Section 6 for how to create one)
7. Optionally enter your name and email (shown in commit history)
8. Click **Connect**

GitFlow initializes the repository, creates a Simio-specific `.gitignore`, and pushes your project to GitHub. You are now connected and ready to work.

### Scenario B: Joining an Existing Repository (A Teammate Already Set It Up)

Use this path when a teammate has already created the repository and you need to get a copy.

1. Get the repository URL from your teammate or from GitHub
2. Create an empty folder on your computer (e.g., `C:\Projects\TeamModel\`)
   - Important: The folder must be empty and must **not** be inside OneDrive or any cloud-synced folder
3. Open Simio
4. Click **Connect** in the Version Control ribbon
5. Browse to the empty folder you created
6. Select **Clone a repository into this folder**
7. Paste the remote URL
8. Enter your PAT, name, and email
9. Click **Connect**

GitFlow clones the repository into your folder. If a `.simproj` file is found, GitFlow offers to open it for you.

### Scenario C: Reconnecting (Your Project Is Already in a Git Repo)

Use this path when your Simio project is already inside a Git repository folder (for example, you cloned it previously using GitHub Desktop or the command line).

1. Open Simio and open your `.simproj` file
2. Click any action button — for example, **Commit/Push** or **Pull**
3. GitFlow auto-detects the repository and connects using your stored credentials
4. A notification confirms the connection — no dialog needed

If GitFlow does not have stored credentials for this host, it will prompt you for your PAT once.

---

## 6. Authentication

### Creating a Personal Access Token (PAT) on GitHub

A PAT is like a password that gives GitFlow permission to access your repositories. You create it once and GitFlow remembers it.

1. Go to github.com and sign in
2. Click your profile picture (top-right corner) > **Settings**
3. Scroll down the left sidebar and click **Developer settings**
4. Click **Personal access tokens** > **Tokens (classic)** > **Generate new token (classic)**
5. Give it a name (e.g., "GitFlow Simio")
6. Set an expiration (90 days is a good default, or "No expiration" for convenience)
7. Under **Select scopes**, check the **repo** checkbox (this grants full repository access)
8. Click **Generate token**
9. **Copy the token immediately** — you will not be able to see it again after leaving this page
   - GitHub tokens start with `ghp_`

### How Credentials Are Stored

- **Your PAT** is stored securely in **Windows Credential Manager**, keyed by host (e.g., `GitFlow:github.com`). This means one GitHub token works for all your GitHub repositories.
- **Your name and email** are stored in a config file in the GitFlow add-in folder. These are only used for commit messages.
- You can update your credentials at any time by clicking **Connect** and entering new values.

### GitHub OAuth Sign-In

If your organization has configured a GitHub OAuth App, you can click **Sign in with GitHub** in the Connect dialog for browser-based authentication instead of using a PAT. This is optional and requires setup by an administrator.

### Supported Hosts

GitFlow works with:

- **GitHub** (github.com) — PAT or OAuth
- **Azure DevOps** (dev.azure.com) — PAT
- **Bitbucket** (bitbucket.org) — PAT (called "App Passwords" in Bitbucket)

---

## 7. Daily Workflow

This section walks through the actions you will use regularly.

### 7.1 Making and Saving Changes (Commit/Push)

This is the most common action. You have made changes to your model and want to save and share them.

1. Make your changes in Simio (add objects, change properties, update data, etc.)
2. **Save your project in Simio** (Ctrl+S or File > Save) — this is important; GitFlow tracks the saved file
3. Click **Commit/Push** in the ribbon
4. A dialog asks for a commit message — describe what you changed. Good examples:
   - "Added second conveyor line to assembly area"
   - "Updated arrival rate to 15 per hour"
   - "Fixed routing logic for Station 3"
   - "Added overtime schedule to worker resources"
5. Confirm the commit

Your changes are now saved locally and uploaded to the server. Teammates can get your changes by clicking **Pull**.

**What if there are no changes?** If you click **Commit/Push** without making any changes (or forgot to save in Simio first), GitFlow tells you: *"Nothing to Save."* Save your project in Simio and try again.

**What about pushing to main?** If you are on the main branch, GitFlow will ask: *"You're currently on the main branch. Would you like to push to a new development branch instead of directly to main?"* This is a safety feature — it is generally better to work on a branch and merge when ready (see Section 7.3).

### 7.2 Getting Your Team's Changes (Pull)

When teammates have pushed changes, you need to pull them to stay up to date.

1. Click **Pull** in the ribbon
2. GitFlow downloads and applies the changes
3. A message confirms: *"Local repository was synced to remote repository."*
4. **Close and reopen your project** to see the updated model (GitFlow will attempt to refresh automatically, but closing and reopening ensures all changes are visible)

**What if there are conflicts?** This is rare with Simio models (see Section 8 for why), but if it happens, GitFlow will ask: *"Conflicts were encountered. Would you like to overwrite your local project with the remote version?"* If you choose **Yes**, your local copy is replaced with the server version. If you choose **No**, you can commit and push your changes first, then coordinate with your team.

**What if you are already up to date?** GitFlow tells you: *"The local repository was already up to date with remote."*

### 7.3 Working with Branches

Branches let you experiment without affecting the main version. This is especially useful when:

- You want to try a new layout or configuration
- Multiple people are working on the same model
- You want to keep a stable "main" version while developing new features

#### Creating a Branch

1. Click **Create Branch** in the ribbon
2. Enter a branch name — use something descriptive like `paul-new-layout` or `test-fast-conveyor`
   - Use letters, numbers, hyphens, and underscores only (no spaces or special characters)
3. Confirm the creation

You are now on your new branch. Any changes you commit go to this branch only — main stays untouched.

#### Switching Between Branches

1. Click **Select Branch** in the ribbon
2. A dialog shows all available branches with your current branch indicated
3. Select the branch you want to switch to
4. Confirm the switch
5. **Close and reopen your project** to see the model from that branch

**Important:** If you have unsaved changes, GitFlow will warn you. Commit and push your changes before switching branches to avoid losing work.

#### Promoting Your Branch to Main (Merging)

When your branch is tested and ready:

1. Make sure you are on the branch you want to merge (not on main)
2. Commit and push any remaining changes
3. Click **Promote to Main** in the ribbon
4. GitFlow merges your branch into main

If there are conflicts, GitFlow warns you: *"Merge conflicts detected. Forcing this merge will overwrite the 'main' branch with your current branch's content. This is a destructive action and cannot be undone easily. Do you want to continue?"* Only proceed if you are sure your branch should replace main.

#### Deleting a Branch

After merging, you can clean up by deleting the branch:

1. Click **Remove Branch** in the ribbon
2. Select the branch to delete (you cannot delete the branch you are currently on)
3. Confirm the deletion

The branch is removed locally and from the server. Other team members will need to delete their local copies separately.

### 7.4 Undoing Changes (Local Reset)

If you have made changes you want to throw away entirely:

1. Click **Local Reset** in the ribbon
2. GitFlow reverts all files to the state of your last commit
3. **Close and reopen your project** to see the reverted model

This only affects uncommitted changes. If you already committed and pushed, those commits remain in the history. To undo an entire experiment, switch to main and delete the experimental branch instead.

---

## 8. Understanding Simio Files and Git

### The .simproj File Is Binary

Unlike source code (which is plain text), Simio's `.simproj` file is a binary format. This has a few practical implications:

- **Git cannot show line-by-line differences.** When you view the history, you will see that the file changed, but not exactly which properties or objects were modified. This is why good commit messages are important — they are your record of what changed.
- **Merge conflicts cannot be resolved by merging lines.** If two people edit the same model on the same branch, Git cannot combine their changes automatically. One version must be chosen. This is why branches are so important for Simio — work on separate branches and merge one at a time.
- **One editor per branch.** As a best practice, only one person should edit the model on a given branch at a time. Use branches to divide work, and merge when each person's changes are complete.

### What the .gitignore Excludes

GitFlow creates a `.gitignore` file automatically when you initialize a repository. It tells Git to skip files that do not need to be shared:

- **Log files** (`*.log`) — Simio runtime logs
- **Backup files** (`*.backup`) — Simio auto-backups
- **Model view metadata** (`*_ModelViewInfo.xml`) — View layout data that is specific to your machine
- **Build outputs** (`Builds/`) — Compiled experiment results
- **IDE artifacts** (`.vs/`, `bin/`, `obj/`) — Visual Studio cache files

Everything else in the folder is tracked, including your `.simproj`, data files (`.csv`, `.xlsx`), custom scripts, and documentation.

### Best Practices for Simio with Git

1. **Always save in Simio before committing.** GitFlow tracks the file on disk. If you have not saved in Simio, your latest changes are only in memory.
2. **Write descriptive commit messages.** Since Git cannot show what changed inside the binary `.simproj`, your commit messages are the primary record. Be specific: "Added overtime schedule to worker resources" is much better than "Updated model."
3. **Use branches for experiments.** Trying a new layout? Create a branch. If it does not work out, just switch back to main and delete the branch. No harm done.
4. **One person per branch at a time.** Because `.simproj` is binary, two people editing the same file on the same branch will cause conflicts. Assign branches to individuals.
5. **Pull before you start working.** At the beginning of each session, click **Pull** to make sure you have the latest version before making changes.
6. **Keep data files in the repo.** If your model uses CSV or Excel files for input data, put them in the same repository. This way, the data and model always stay in sync.
7. **Do not put your repo in OneDrive (or other cloud-sync folders).** Cloud sync and Git can conflict with each other, causing corrupted repositories. Use a local folder like `C:\Projects\` instead.

---

## 9. Troubleshooting

### "Nothing to Save" when clicking Commit/Push

You either have not made any changes since your last commit, or you forgot to save your project in Simio. Press **Ctrl+S** in Simio to save, then try **Commit/Push** again.

### Authentication errors ("Error connecting to remote")

- Your PAT may have expired. Generate a new one on GitHub (see Section 6) and click **Connect** to enter it.
- Double-check that your PAT has the **repo** scope selected.
- Verify the remote URL is correct (should start with `https://`).

### "Unable to communicate with provided URL"

- Check that the remote URL is correct and the repository exists on GitHub.
- Check your internet connection.
- If the repository is private, make sure your PAT has access to it.

### Permission denied errors

- Your PAT may not have write permissions. Generate a new token with the **repo** scope.
- If you see "zero permissions," your PAT may have expired or been revoked. Create a new one.

### Conflicts when pulling

If you and a teammate both made changes, GitFlow will ask if you want to overwrite your local copy with the remote version. To avoid this:

- **Pull before you start working** each session
- **Work on separate branches** so changes do not overlap
- **Communicate with your team** about who is editing which branch

### Cannot switch branches ("merge conflicts" or "uncommitted changes")

Commit and push your current changes before switching branches. If you want to discard your changes instead, click **Local Reset** first, then switch branches.

### Branch name errors

Branch names cannot contain spaces or special characters (except hyphens `-` and underscores `_`). Use names like `paul-new-layout` or `test_conveyor_speed`.

### Project does not update after Pull or Reset

Close and reopen your project in Simio. While GitFlow attempts to refresh the project automatically, some changes require a full reopen to take effect.

### "A .git folder was detected" when initializing

The folder already contains a Git repository. Either:

- Use **Connect** to connect to the existing repository, or
- Choose a different empty folder, or
- Delete the `.git` folder from the directory if you want to start over

### Repository folder is inside OneDrive

Move your repository to a local path like `C:\Projects\`. Cloud-synced folders can corrupt Git repositories because the sync service modifies files that Git is also tracking.

---

## 10. Glossary

| Term | Definition |
|---|---|
| **Branch** | A parallel line of development. Work on a branch without affecting the main version. |
| **Clone** | Download a complete copy of a remote repository to your computer. |
| **Commit** | A snapshot of your project at a point in time, with a message describing the changes. |
| **Conflict** | When two people change the same file and Git cannot automatically combine the changes. |
| **Credential Manager** | Windows utility where GitFlow securely stores your Personal Access Token. |
| **Git** | A version control system that tracks changes to files over time. |
| **GitHub** | A popular website for hosting Git repositories online. |
| **.gitignore** | A file that tells Git which files and folders to skip (not track). |
| **Host** | The server where your remote repository lives (e.g., github.com, dev.azure.com). |
| **Local repository** | The copy of the repository on your own computer. |
| **Main branch** | The default, primary branch — typically the stable, "official" version of the project. |
| **Merge** | Combine changes from one branch into another. |
| **PAT (Personal Access Token)** | A secure key that gives GitFlow permission to access your repositories. |
| **Promote** | GitFlow's term for merging your current branch into the main branch. |
| **Pull** | Download and apply the latest changes from the remote repository to your local copy. |
| **Push** | Upload your local commits to the remote repository. |
| **Remote repository** | The copy of the repository on a server (GitHub, Azure DevOps, etc.) shared with the team. |
| **Repo** | Short for repository. |
| **Repository** | A project folder tracked by Git, containing all files and their change history. |
| **Reset** | Revert all uncommitted changes back to the last commit. |
