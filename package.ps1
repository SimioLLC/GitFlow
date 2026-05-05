<#
.SYNOPSIS
    Builds GitFlow and packages it into a distributable zip file.

.DESCRIPTION
    Builds the GitFlow project in Release configuration and creates a zip
    containing all files needed to install the add-in. Users extract the zip
    to their Documents/SimioUserExtensions/ folder.

.PARAMETER Configuration
    Build configuration. Default: Release

.PARAMETER Version
    Version label for the zip filename (e.g. "1.0.0"). If not specified,
    reads from CHANGELOG.txt or defaults to "dev".

.EXAMPLE
    .\package.ps1
    .\package.ps1 -Version "1.2.0"
#>

param(
    [string]$Configuration = "Release",
    [string]$Version = ""
)

$ErrorActionPreference = "Stop"

$ProjectDir = Join-Path $PSScriptRoot "GitFlow"
$ProjectFile = Join-Path $ProjectDir "GitFlow.csproj"
$OutputDir = Join-Path $PSScriptRoot "dist"

# --- Determine version ---
if (-not $Version) {
    # Try to read from CHANGELOG.txt
    $changelog = Join-Path $PSScriptRoot "CHANGELOG.txt"
    if (Test-Path $changelog) {
        $firstVersionLine = Get-Content $changelog | Where-Object { $_ -match '^\s*v?\d+\.\d+' } | Select-Object -First 1
        if ($firstVersionLine -match '(v?\d+\.\d+(\.\d+)?)') {
            $Version = $matches[1] -replace '^v', ''
        }
    }
    if (-not $Version) { $Version = "dev" }
}

Write-Host "Packaging GitFlow v$Version ($Configuration)..." -ForegroundColor Cyan

# --- Build ---
Write-Host "Building..." -ForegroundColor Cyan
dotnet build $ProjectFile -c $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed." -ForegroundColor Red
    exit 1
}
Write-Host "Build succeeded." -ForegroundColor Green

# --- Locate build output ---
$BuildOutput = [System.IO.Path]::Combine($ProjectDir, "bin", $Configuration, "net9.0-windows7.0")
if (-not (Test-Path $BuildOutput)) {
    Write-Host "Build output not found at: $BuildOutput" -ForegroundColor Red
    exit 1
}

# --- Stage files into a temp folder ---
$StagingDir = Join-Path $OutputDir "GitFlow"
if (Test-Path $StagingDir) {
    Remove-Item $StagingDir -Recurse -Force
}
New-Item $StagingDir -ItemType Directory -Force | Out-Null

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
        Copy-Item $src $StagingDir
        Write-Host "  Staged: $file" -ForegroundColor DarkGray
    } else {
        Write-Host "  Missing: $file (skipped)" -ForegroundColor Yellow
    }
}

# Copy native runtimes (libgit2 native binaries)
$runtimesSrc = Join-Path $BuildOutput "runtimes"
if (Test-Path $runtimesSrc) {
    Copy-Item $runtimesSrc (Join-Path $StagingDir "runtimes") -Recurse
    Write-Host "  Staged: runtimes/" -ForegroundColor DarkGray
}

# --- Create zip ---
if (-not (Test-Path $OutputDir)) {
    New-Item $OutputDir -ItemType Directory -Force | Out-Null
}

$ZipName = "GitFlow-v$Version.zip"
$ZipPath = Join-Path $OutputDir $ZipName

if (Test-Path $ZipPath) {
    Remove-Item $ZipPath -Force
}

Compress-Archive -Path $StagingDir -DestinationPath $ZipPath
Remove-Item $StagingDir -Recurse -Force

Write-Host ""
Write-Host "Package created: dist/$ZipName" -ForegroundColor Green
Write-Host "Upload this file to a GitHub Release for distribution." -ForegroundColor Yellow
