<#
.SYNOPSIS
    Builds and deploys the GitFlow Simio add-in.

.DESCRIPTION
    Builds the GitFlow project in Release configuration and copies
    the output to the Simio user extensions folder. Supports both
    standard Documents and OneDrive-synced Documents folders.

.PARAMETER Configuration
    Build configuration. Default: Release

.EXAMPLE
    .\deploy.ps1
    .\deploy.ps1 -Configuration Debug
#>

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$ProjectDir = Join-Path $PSScriptRoot "GitFlow"
$ProjectFile = Join-Path $ProjectDir "GitFlow.csproj"

# --- Build ---
Write-Host "Building GitFlow ($Configuration)..." -ForegroundColor Cyan
dotnet build $ProjectFile -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed." -ForegroundColor Red
    exit 1
}
Write-Host "Build succeeded." -ForegroundColor Green

# --- Locate output ---
$BuildOutput = [System.IO.Path]::Combine($ProjectDir, "bin", $Configuration, "net9.0-windows7.0")
if (-not (Test-Path $BuildOutput)) {
    Write-Host "Build output not found at: $BuildOutput" -ForegroundColor Red
    exit 1
}

# --- Find Simio extensions folder ---
$SimioExtDir = $null

# Check OneDrive paths first (common for Simio LLC employees)
$OneDrivePaths = Get-ChildItem "$env:USERPROFILE" -Directory -Filter "OneDrive*" -ErrorAction SilentlyContinue
foreach ($od in $OneDrivePaths) {
    $candidate = [System.IO.Path]::Combine($od.FullName, "Documents", "SimioUserExtensions")
    if (Test-Path $candidate) {
        $SimioExtDir = $candidate
        break
    }
}

# Fall back to standard Documents
if (-not $SimioExtDir) {
    $candidate = Join-Path ([Environment]::GetFolderPath("MyDocuments")) "SimioUserExtensions"
    if (Test-Path $candidate) {
        $SimioExtDir = $candidate
    }
}

if (-not $SimioExtDir) {
    Write-Host "Simio user extensions folder not found." -ForegroundColor Red
    Write-Host "Expected: Documents\SimioUserExtensions" -ForegroundColor Yellow
    exit 1
}

$DeployDir = Join-Path $SimioExtDir "GitFlow"

# --- Deploy ---
Write-Host "Deploying to: $DeployDir" -ForegroundColor Cyan

if (Test-Path $DeployDir) {
    Remove-Item $DeployDir -Recurse -Force
}
New-Item $DeployDir -ItemType Directory -Force | Out-Null

# Copy main DLLs and deps
$filesToCopy = @(
    "GitFlow.dll",
    "GitFlow.pdb",
    "GitFlow.deps.json",
    "LibGit2Sharp.dll",
    "Meziantou.Framework.Win32.CredentialManager.dll",
    ".gitignore"
)

foreach ($file in $filesToCopy) {
    $src = Join-Path $BuildOutput $file
    if (Test-Path $src) {
        Copy-Item $src $DeployDir
        Write-Host "  Copied: $file" -ForegroundColor DarkGray
    }
}

# Copy native runtimes (libgit2 native binaries)
$runtimesSrc = Join-Path $BuildOutput "runtimes"
if (Test-Path $runtimesSrc) {
    Copy-Item $runtimesSrc (Join-Path $DeployDir "runtimes") -Recurse
    Write-Host "  Copied: runtimes/" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host "Deployed successfully to: $DeployDir" -ForegroundColor Green
Write-Host "Restart Simio to load the updated add-in." -ForegroundColor Yellow
