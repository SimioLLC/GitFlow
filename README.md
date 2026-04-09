# GitFlow - Git Version Control for Simio

A Simio design-time add-in that brings Git version control directly into the Simio ribbon. Designed for simulation modelers who may be using Git for the first time.

## Features

- **One-click Connect** - Smart auto-detection of existing repos, or create/clone with a single dialog
- **GitHub OAuth sign-in** - No need to manually create tokens for GitHub repos
- **PAT support** - Personal Access Tokens for GitHub, Azure DevOps, and Bitbucket
- **Commit & Push** - Save and share model changes with commit messages
- **Pull** - Get the latest changes from your team (safe fast-forward with conflict detection)
- **Branch management** - Create, switch, and delete branches for parallel development
- **Promote to Main** - Merge your branch into main with conflict detection and warnings
- **Local Reset** - Revert uncommitted changes to the last committed state

## Quick Start

1. **Install**: Copy the GitFlow folder to your `Documents/SimioUserExtensions/` directory
2. **Open Simio**: The "Version Control" tab appears in the ribbon
3. **Click Connect**: The add-in auto-detects if your project is already in a Git repo
4. **Start working**: Commit, push, pull, and branch directly from Simio

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

- **GitHub**: Click "Sign in with GitHub" for OAuth device flow (no token needed)
- **Azure DevOps / Bitbucket**: Enter a Personal Access Token (links to token creation pages provided in the UI)
- Credentials are stored securely in Windows Credential Manager

## Version History

### V5 (Current - feature/ux-overhaul)
- Unified "Connect" button replaces Init/Clone/Open -- auto-detects existing repos
- GitHub OAuth Device Flow sign-in
- Git host auto-detection (GitHub, Azure DevOps, Bitbucket)
- PAT fields masked in all forms
- Fixed critical merge logic bug in Promote to Main
- Subfolder support -- .simproj files no longer need to be at the repo root
- Expanded .gitignore template for Simio projects
- Credential storage hardened (session scope)
- Major code cleanup -- extracted helpers, removed dead code
- Current branch indicator in branch selection/deletion dialogs
- Modern build/deploy script (`deploy.ps1`)

### V4
- Added merge conflict detection and warnings
- Main branch push protections

### V3
- Credential management via Windows Credential Manager
- Permission level checking (read-only vs read/write)

### V2
- Branch management (create, switch, delete)
- Safe push/pull with conflict detection

### V1
- Initial release with basic init, clone, commit, push, pull

## License

Apache License 2.0

## Contributing

See [CLAUDE.md](CLAUDE.md) for development setup and architecture details.
