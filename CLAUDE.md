# GitFlow - Simio Git Integration Add-In

## What This Is

A Simio design-time add-in that provides Git version control through Simio's ribbon UI. Target users are simulation modelers who are mostly first-time Git users. Uses LibGit2Sharp for all Git operations (no shell calls).

## Architecture

- **GitFlowAddIn.cs** - Ribbon button classes. Each implements `IDesignAddIn` + `IDesignAddInGuiDetails`. `AddInHelper.EnsureConnected()` provides auto-connect logic for all action buttons.
- **LibgitFunctionClass.cs** - All Git operations (init, clone, commit, push, pull, branch, merge). Static methods using LibGit2Sharp. `GetRemote(repo)` handles repos with non-standard remote names.
- **GitContext.cs** - Thread-safe singleton holding repo path, remote URL, PAT, username, email, permission level.
- **GitFlowConfig.cs** - Persistent host-based credential storage. PATs in Windows Credential Manager keyed by host (e.g. `GitFlow:github.com`). Username/email in `gitflow-config.json`.
- **ConnectForm.cs** - Unified connect dialog. Auto-detects repos from active project, guided step-by-step UI. `FindActiveProjectDirectory()` tries reflection, then file search by project name.
- **OAuthDeviceFlowHandler.cs** - GitHub OAuth 2.0 Device Flow (RFC 8628). Requires registered OAuth App client_id.
- **GitHostDetector.cs** - Detects GitHub/AzureDevOps/Bitbucket from remote URLs. Provides token creation URLs.
- **CredentialHandler.cs** - Legacy per-repo Windows Credential Manager wrapper (kept for backward compatibility).
- **SystemDirectoryHandler.cs** - Directory utilities, `Refresh()` reloads Simio project after Git operations. Searches recursively for `.simproj` files (subfolder support).
- **Forms** - CreateBranchForm, CommitForm, BranchSelectForm, BranchRemoveForm. All have confirmation dialogs and friendly error messages.

## Build & Deploy

```powershell
# Build
dotnet build GitFlow/GitFlow.csproj -c Release

# Build + deploy to Simio extensions folder
.\deploy.ps1
.\deploy.ps1 -Configuration Debug
```

Output goes to `GitFlow/bin/{Configuration}/net9.0-windows7.0/`. The deploy script copies DLLs to `Documents/SimioUserExtensions/GitFlow/`.

**Note**: Close Simio before deploying -- it locks the DLL.

## Dependencies

- **.NET 9.0** (net9.0-windows7.0, Windows Forms)
- **LibGit2Sharp 0.31.0** + native binaries - Git operations
- **DevExpress WinForms** - UI components (MRUEdit, SimpleButton, XtraForm)
- **Meziantou.Framework.Win32.CredentialManager** - Windows credential storage
- **SimioAPI.dll / SimioAPI.Extensions.dll** - Referenced from `C:\Program Files\Simio LLC\Simio\`

## Key Conventions

- Every ribbon button is its own class implementing `IDesignAddIn` in GitFlowAddIn.cs
- All action buttons call `AddInHelper.EnsureConnected(context, requiredPermission)` which handles auto-connect
- All Git operations go through `LibgitFunctionClass` (never shell commands)
- Use `GetRemote(repo)` instead of `repo.Network.Remotes["origin"]` -- some repos use non-standard remote names
- Credentials stored by host in Windows Credential Manager (e.g. `GitFlow:github.com`), not per-repo
- PAT fields must always use `UseSystemPasswordChar = true`
- Error messages use resource strings from `Resources/Resource1.resx`
- `Refresh()` searches recursively for `.simproj` files (supports subfolder layouts)
- Use `TranslateGitException()` helper for common auth/remote error handling in catch blocks
- Use `CleanupFailedInit()` for init error cleanup
- All destructive actions (delete branch, merge, switch branch) require confirmation dialogs
- All success messages include context (branch name, repo path, etc.)
- `FindActiveProjectDirectory()` handles the case where SimioAPI doesn't expose file paths via reflection

## Credential Flow

1. User authenticates once per host (GitHub, Azure DevOps, etc.)
2. PAT stored in Windows Credential Manager keyed as `GitFlow:{hostname}`
3. Username/email stored in `gitflow-config.json` in the UserExtensions folder
4. Auto-connect reads host credential, legacy per-repo credential, or falls back to ConnectForm
5. Old per-repo credentials (keyed by folder path) still work as fallback

## OAuth Setup

The GitHub OAuth Device Flow requires a registered GitHub OAuth App. Replace the placeholder `client_id` in `OAuthDeviceFlowHandler.cs` with the real value from `github.com/settings/applications/new`. Until configured, clicking "Sign in with GitHub" shows PAT creation instructions instead.

## Testing

1. Build with `dotnet build` or `.\deploy.ps1`
2. Close Simio, deploy, restart Simio
3. Check the "Version Control" ribbon tab has: Connect, Commit & Push, Pull, Reset, Create Branch, Select Branch, Promote to Main, Remove Branch
4. Test auto-connect: open a project in a cloned repo, click any action -- should auto-connect
5. Test Connect form: with no project open, click Connect, browse to a repo folder
6. Test subfolder layout: .simproj in a subfolder of the repo root
7. Test credential reuse: connect to one GitHub repo, then open a different GitHub repo -- should auto-connect without re-entering PAT
