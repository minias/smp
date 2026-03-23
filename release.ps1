<#
.SYNOPSIS
SMP 자동 릴리즈 스크립트 (Production Ready)

.DESCRIPTION
- semantic version bump
- changelog update
- git commit / tag / push
- PowerShell 5.1 / 7 호환
- UTF-8 출력 지원
- 안정성 강화
#>

param (
    [ValidateSet("patch", "minor", "major")]
    [string]$Type,

    [switch]$DryRun,
    [switch]$NoPush,
    [switch]$SkipChangelog,
    [switch]$Help
)

# -----------------------------
# Help
# -----------------------------
function Show-Help {
    Write-Host @"
SMP Release Script

USAGE:
  .\release.ps1 -Type <patch|minor|major> [options]

OPTIONS:
  -Type              Version bump type (patch | minor | major)
  -DryRun            Simulate without applying changes
  -NoPush            Skip git push
  -SkipChangelog     Do not update CHANGELOG.md
  -Help              Show help

EXAMPLES:
  .\release.ps1 -Type patch
  .\release.ps1 -Type minor -DryRun
  .\release.ps1 -Type major -NoPush
"@
}

# -----------------------------
# Help / Argument Validation
# -----------------------------
if ($Help -or -not $Type) {
    Show-Help
    exit 0
}

# -----------------------------
# Encoding Setup (UTF-8)
# -----------------------------
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
cmd /c chcp 65001 | Out-Null

# -----------------------------
# Global Settings
# -----------------------------
$ErrorActionPreference = "Stop"

# -----------------------------
# Git 실행 Wrapper
# -----------------------------
function Invoke-Git {
    param (
        [Parameter(Mandatory=$true)]
        [string[]]$GitArgs
    )

    $result = & git @GitArgs 2>&1

    if ($LASTEXITCODE -ne 0) {
        throw "Git command failed: git $($GitArgs -join ' ')`n$result"
    }

    return $result
}

# -----------------------------
# Get Latest Tag
# -----------------------------
function Get-LatestTag {
    try {
        $tag = Invoke-Git @("describe", "--tags", "--abbrev=0")
        return $tag.Trim()
    }
    catch {
        return "v0.0.0"
    }
}

# -----------------------------
# Parse Version
# -----------------------------
function Get-VersionInfo {
    param ([string]$tag)

    $clean = $tag.TrimStart("v")
    $parts = $clean.Split(".")

    if ($parts.Count -ne 3) {
        throw "Invalid version format: $tag"
    }

    return @{
        Major = [int]$parts[0]
        Minor = [int]$parts[1]
        Patch = [int]$parts[2]
    }
}

# -----------------------------
# Version Bump
# -----------------------------
function Update-Version {
    param ($version, $type)

    switch ($type) {
        "major" {
            $version.Major++
            $version.Minor = 0
            $version.Patch = 0
        }
        "minor" {
            $version.Minor++
            $version.Patch = 0
        }
        "patch" {
            $version.Patch++
        }
    }

    return $version
}

# -----------------------------
# Git Repo Check
# -----------------------------
if (-not (Test-Path ".git")) {
    throw "Git repository not found."
}

# -----------------------------
# Version Calculation
# -----------------------------
$latestTag = Get-LatestTag
Write-Host "📌 Latest Tag: $latestTag"

$version = Get-VersionInfo $latestTag
$newVersionObj = Update-Version $version $Type

$newVersion = "{0}.{1}.{2}" -f $newVersionObj.Major, $newVersionObj.Minor, $newVersionObj.Patch
$newTag = "v$newVersion"

Write-Host "🚀 Target Version: $newTag"

if ($DryRun) {
    Write-Host "🧪 DRY RUN - no changes applied"
    exit 0
}

# -----------------------------
# CHANGELOG Update
# -----------------------------
if (-not $SkipChangelog) {

    $changelogPath = "CHANGELOG.md"

    if (-not (Test-Path $changelogPath)) {
        throw "CHANGELOG.md not found."
    }

    $today = Get-Date -Format "yyyy-MM-dd"
    $content = Get-Content $changelogPath -Raw

    if ([string]::IsNullOrWhiteSpace($content)) {
        throw "CHANGELOG.md is empty."
    }

    # 중복 버전 방지
    if ($content -match "## \[$newVersion\]") {
        throw "CHANGELOG already contains version $newVersion"
    }

    if ($content -notmatch "# Changelog") {
        throw "CHANGELOG header not found."
    }

    $header = @"
## [$newVersion] - $today

### Bug Fixes
- 

### Improvements
- 

"@

    $updated = $content -replace "# Changelog", "# Changelog`n`n$header"

    Set-Content -Path $changelogPath -Value $updated -Encoding UTF8

    Write-Host "📝 CHANGELOG updated"
}
else {
    Write-Host "⏭️ Skip CHANGELOG"
}

# -----------------------------
# Git Add
# -----------------------------
Invoke-Git @("add", ".")

# -----------------------------
# Git Commit
# -----------------------------
Invoke-Git @("commit", "-m", "chore(release): $newTag")
Write-Host "✅ Commit created"

# -----------------------------
# Git Tag
# -----------------------------
Invoke-Git @("tag", "-a", $newTag, "-m", "Release $newTag")
Write-Host "🏷️ Tag created: $newTag"

# -----------------------------
# Push
# -----------------------------
if (-not $NoPush) {

    $branch = Invoke-Git @("rev-parse", "--abbrev-ref", "HEAD")

    Write-Host "📤 Push branch: $branch"
    Invoke-Git @("push", "origin", $branch)

    Write-Host "📤 Push tag: $newTag"
    Invoke-Git @("push", "origin", $newTag)

}
else {
    Write-Host "⏭️ Push skipped"
}

Write-Host "🎉 Release completed: $newTag"