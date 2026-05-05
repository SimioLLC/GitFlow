<#
.SYNOPSIS
    Creates a GitHub Release and uploads the GitFlow package.

.DESCRIPTION
    Builds the distributable zip with package.ps1, creates a git tag,
    and publishes a GitHub Release with the zip attached.
    Requires the GitHub CLI (gh) to be installed and authenticated.

.PARAMETER Version
    Version number for the release (e.g. "1.2.0"). Required.

.PARAMETER Notes
    Release notes. If not specified, pulls from CHANGELOG.txt.

.PARAMETER Draft
    Create as a draft release (default: false).

.EXAMPLE
    .\release.ps1 -Version "1.2.0"
    .\release.ps1 -Version "1.2.0" -Notes "Bug fixes and improvements" -Draft
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$Notes = "",

    [switch]$Draft
)

$ErrorActionPreference = "Stop"

# --- Verify gh CLI is available ---
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "GitHub CLI (gh) is not installed." -ForegroundColor Red
    Write-Host "Install it from: https://cli.github.com/" -ForegroundColor Yellow
    exit 1
}

$Tag = "v$Version"
$ZipPath = Join-Path $PSScriptRoot "dist" "GitFlow-$Tag.zip"

# --- Build the package ---
Write-Host "Building package..." -ForegroundColor Cyan
& (Join-Path $PSScriptRoot "package.ps1") -Version $Version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Packaging failed." -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $ZipPath)) {
    Write-Host "Expected zip not found: $ZipPath" -ForegroundColor Red
    exit 1
}

# --- Extract notes from CHANGELOG.txt if not provided ---
if (-not $Notes) {
    $changelog = Join-Path $PSScriptRoot "CHANGELOG.txt"
    if (Test-Path $changelog) {
        $lines = Get-Content $changelog
        $capturing = $false
        $noteLines = @()
        foreach ($line in $lines) {
            if ($line -match "^\s*v?$([regex]::Escape($Version))") {
                $capturing = $true
                continue
            }
            if ($capturing -and $line -match '^\s*v?\d+\.\d+') {
                break
            }
            if ($capturing) {
                $noteLines += $line
            }
        }
        if ($noteLines.Count -gt 0) {
            $Notes = ($noteLines | Out-String).Trim()
        }
    }
    if (-not $Notes) {
        $Notes = "GitFlow $Tag"
    }
}

# --- Create tag if it doesn't exist ---
$existingTag = git tag -l $Tag 2>$null
if (-not $existingTag) {
    Write-Host "Creating tag $Tag..." -ForegroundColor Cyan
    git tag $Tag
    git push origin $Tag
} else {
    Write-Host "Tag $Tag already exists." -ForegroundColor DarkGray
}

# --- Validate version format to prevent injection via tag ---
if ($Version -notmatch '^[\w.\-+]+$') {
    Write-Host "Invalid version format: $Version" -ForegroundColor Red
    Write-Host "Version may only contain letters, digits, dots, dashes, underscores, and plus signs." -ForegroundColor Yellow
    exit 1
}

# --- Create GitHub Release ---
Write-Host "Creating GitHub Release $Tag..." -ForegroundColor Cyan

# Build argument list for safe invocation (no Invoke-Expression -- avoids
# command injection via $Notes containing backticks or PowerShell metacharacters).
$ghArgs = @(
    "release", "create",
    $Tag,
    $ZipPath,
    "--title", "GitFlow $Tag",
    "--notes", $Notes
)
if ($Draft) {
    $ghArgs += "--draft"
}

& gh @ghArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Release $Tag published successfully!" -ForegroundColor Green
    Write-Host "View at: https://github.com/SimioLLC/GitFlow/releases/tag/$Tag" -ForegroundColor Cyan
} else {
    Write-Host "Release creation failed." -ForegroundColor Red
    exit 1
}
