# GitFlow - Git Version Control for Simio

A Simio design-time add-in that brings Git version control directly into the Simio ribbon. Designed for simulation modelers who may be using Git for the first time.

## Features

- **Auto-connect** - Open your project and start working. GitFlow detects the repo and connects automatically using stored credentials.
- **One-click Connect** - Smart auto-detection of existing repos, or create/clone with a guided step-by-step dialog
- **Authenticate once** - Enter your GitHub PAT once and it works across all your GitHub repos. Credentials stored securely in Windows Credential Manager.
- **GitHub OAuth sign-in** - Sign in with GitHub directly (requires OAuth App registration, see below)
- **PAT support** - Personal Access Tokens for GitHub, Azure DevOps, and Bitbucket
- **Commit & Push** - Save and share model changes with confirmation dialogs and clear messages
- **Pull** - Get the latest changes from your team (safe fast-forward with conflict detection)
- **Branch management** - Create, switch, and delete branches with confirmations and current branch indicators
- **Promote to Main** - Merge your branch into main with conflict detection and warnings
- **Local Reset** - Revert uncommitted changes to the last committed state
- **Subfolder support** - Your .simproj can live in a subfolder of the repo

## Quick Start

### If your project is already in a Git repo (cloned via GitHub Desktop, etc.)

1. **Install**: Copy the GitFlow folder to your `Documents/SimioUserExtensions/` directory
2. **Open Simio** and open your project
3. **Click any action** (Commit & Push, Pull, Create Branch, etc.) - GitFlow auto-detects the repo
4. **Enter your PAT once** when prompted - it's saved for all future repos on that host

### Starting fresh

1. **Install** the add-in
2. **Open Simio** - the "Version Control" tab appears in the ribbon
3. **Click Connect** - browse to your folder, enter the remote URL, authenticate
4. **Start working** - commit, push, pull, and branch directly from Simio

## Ribbon Layout

| Repo Actions | Actions | Branching Actions |
|---|---|---|
| Connect | Commit & Push | Create Branch |
| | Pull | Select Branch |
| | Reset | Promote to Main |
| | | Remove Branch |

## Build & Deploy

Requires .NET 9.0 SDK and Simio installed at the default location.

```powershell
# Build and deploy to Simio extensions folder
.\deploy.ps1

# Build only
dotnet build GitFlow/GitFlow.csproj -c Release
```

## Why Use Git with Simio?

1. **Commit log** - Document every model change with a message. Your team sees what changed and why.
2. **Peer review** - Work on a dev branch, push it, and let a teammate review before merging to main.
3. **Undo changes** - Delete a dev branch to revert to main. No more "Model_v3_final_FINAL.simproj".
4. **Clean file management** - Models, data files, scripts -- everything in one repo. Team members clone once and pull updates.

## Authentication

- **First time**: Click Connect (or any action), enter your Personal Access Token
- **After that**: Credentials are stored per-host (e.g. all GitHub repos share one token). Auto-connect handles the rest.
- **GitHub OAuth** (optional): Register a GitHub OAuth App and set the client_id in `OAuthDeviceFlowHandler.cs` for browser-based sign-in
- **Supported hosts**: GitHub, Azure DevOps, Bitbucket

Credentials are stored in:
- **Windows Credential Manager** - PATs (secure, keyed by host like `GitFlow:github.com`)
- **gitflow-config.json** - Username and email only (in the UserExtensions/GitFlow folder)

## Version History

See [CHANGELOG.txt](CHANGELOG.txt) for detailed version history.

## License

Apache License 2.0

## Contributing

See [CLAUDE.md](CLAUDE.md) for development setup and architecture details.
