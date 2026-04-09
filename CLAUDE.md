# GitFlow - Simio Git Integration Add-In

## What This Is

A Simio design-time add-in that provides Git version control through Simio's ribbon UI. Target users are simulation modelers who are mostly first-time Git users. Uses LibGit2Sharp for all Git operations (no shell calls).

## Architecture

- **GitFlowAddIn.cs** - Ribbon button classes. Each implements `IDesignAddIn` + `IDesignAddInGuiDetails`. Single `ConnectRepo` entry point replaced the old Init/Clone/Open buttons.
- **LibgitFunctionClass.cs** - All Git operations (init, clone, commit, push, pull, branch, merge). Static methods using LibGit2Sharp.
- **GitContext.cs** - Thread-safe singleton holding repo path, remote URL, PAT, username, email, permission level.
- **ConnectForm.cs** - Unified connect dialog. Auto-detects existing repos, supports GitHub OAuth + PAT auth.
- **OAuthDeviceFlowHandler.cs** - GitHub OAuth 2.0 Device Flow (RFC 8628).
- **GitHostDetector.cs** - Detects GitHub/AzureDevOps/Bitbucket from remote URLs.
- **CredentialHandler.cs** - Windows Credential Manager wrapper (Session persistence).
- **SystemDirectoryHandler.cs** - Directory utilities, `Refresh()` reloads Simio project after Git operations.

## Build & Deploy

```powershell
# Build
dotnet build GitFlow/GitFlow.csproj -c Release

# Build + deploy to Simio extensions folder
.\deploy.ps1
.\deploy.ps1 -Configuration Debug
```

Output goes to `GitFlow/bin/{Configuration}/net9.0-windows7.0/`. The deploy script copies DLLs to `Documents/SimioUserExtensions/GitFlow/`.

## Dependencies

- **.NET 9.0** (net9.0-windows7.0, Windows Forms)
- **LibGit2Sharp 0.31.0** + native binaries - Git operations
- **DevExpress WinForms** - UI components (MRUEdit, SimpleButton, XtraForm)
- **Meziantou.Framework.Win32.CredentialManager** - Windows credential storage
- **SimioAPI.dll / SimioAPI.Extensions.dll** - Referenced from `C:\Program Files\Simio LLC\Simio\`

## Key Conventions

- Every ribbon button is its own class implementing `IDesignAddIn` in GitFlowAddIn.cs
- All Git operations go through `LibgitFunctionClass` (never shell commands)
- Credentials stored in Windows Credential Manager, keyed by repo path
- PAT fields must always use `UseSystemPasswordChar = true`
- Error messages use resource strings from `Resources/Resource1.resx`
- `Refresh()` searches recursively for `.simproj` files (supports subfolder layouts)
- Use `TranslateGitException()` helper for common auth/remote error handling in catch blocks
- Use `CleanupFailedInit()` for init error cleanup instead of copy-pasting cleanup blocks

## OAuth Setup

The GitHub OAuth Device Flow requires a registered GitHub OAuth App. The `client_id` placeholder in `OAuthDeviceFlowHandler.cs` must be replaced with the real value from `github.com/settings/applications/new`.

## Testing

1. Build with `dotnet build`
2. Deploy with `.\deploy.ps1`
3. Open Simio, check the "Version Control" ribbon tab
4. Test Connect flow: existing repo, clone, init
5. Test commit/push, pull, branch create/switch/delete, merge to main
