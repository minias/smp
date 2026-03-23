<#
.SYNOPSIS
SMP 자동 릴리즈 스크립트 (Advanced)

.PARAMETER Type
Version bump type: patch | minor | major

.PARAMETER DryRun
실제 변경 없이 시뮬레이션

.PARAMETER NoPush
git push 생략

.PARAMETER SkipChangelog
CHANGELOG 업데이트 생략
#>

param (
    [Parameter(Mandatory=$true)]
    [ValidateSet("patch", "minor", "major")]
    [string]$Type,

    [switch]$DryRun,
    [switch]$NoPush,
    [switch]$SkipChangelog
)

# -----------------------------
# Usage
# -----------------------------
function Show-Usage {
    Write-Host ""
    Write-Host "SMP Release Script"
    Write-Host "==================="
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\release.ps1 -Type patch"
    Write-Host "  .\release.ps1 -Type minor"
    Write-Host "  .\release.ps1 -Type major"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -DryRun          실제 변경 없이 시뮬레이션"
    Write-Host "  -NoPush          git push 생략"
    Write-Host "  -SkipChangelog   CHANGELOG 업데이트 생략"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\release.ps1 -Type patch -DryRun"
    Write-Host "  .\release.ps1 -Type minor -NoPush"
    Write-Host "  .\release.ps1 -Type major -SkipChangelog"
    Write-Host ""
    exit 1
}

# -----------------------------
# Git 저장소 체크
# -----------------------------
if (-not (Test-Path ".git")) {
    Write-Error "Git 저장소가 아닙니다."
    exit 1
}

# -----------------------------
# 최신 태그 조회
# -----------------------------
$latestTag = git describe --tags --abbrev=0 2>$null
if (-not $latestTag) {
    $latestTag = "v0.0.0"
}

$version = $latestTag.TrimStart("v").Split(".")

[int]$major = $version[0]
[int]$minor = $version[1]
[int]$patch = $version[2]

# -----------------------------
# Semantic Version 증가
# -----------------------------
switch ($Type) {
    "major" {
        $major++
        $minor = 0
        $patch = 0
    }
    "minor" {
        $minor++
        $patch = 0
    }
    "patch" {
        $patch++
    }
}

$newVersion = "$major.$minor.$patch"
$newTag = "v$newVersion"

Write-Host "🚀 Target Version: $newTag"

if ($DryRun) {
    Write-Host "🧪 DRY RUN - 실제 변경 없음"
    exit 0
}

# -----------------------------
# CHANGELOG 업데이트
# -----------------------------
if (-not $SkipChangelog) {

    $changelogPath = "CHANGELOG.md"

    if (-not (Test-Path $changelogPath)) {
        Write-Error "CHANGELOG.md 파일이 없습니다."
        exit 1
    }

    $today = Get-Date -Format "yyyy-MM-dd"
    $content = Get-Content $changelogPath -Raw

    $header = @"
## [$newVersion] - $today

### Bug Fixes
- 

### Improvements
- 

"@

    $newContent = $content -replace "# Changelog", "# Changelog`n`n$header"
    Set-Content -Path $changelogPath -Value $newContent

    Write-Host "📝 CHANGELOG 업데이트 완료"
}
else {
    Write-Host "⏭️ CHANGELOG 업데이트 스킵"
}

# -----------------------------
# Git add / commit
# -----------------------------
git add .
git commit -m "chore(release): $newTag"

# -----------------------------
# Git tag 생성
# -----------------------------
git tag -a $newTag -m "Release $newTag"

# -----------------------------
# Push
# -----------------------------
if (-not $NoPush) {

    $branch = git rev-parse --abbrev-ref HEAD

    Write-Host "📤 Push branch: $branch"
    git push origin $branch

    Write-Host "📤 Push tag: $newTag"
    git push origin $newTag

}
else {
    Write-Host "⏭️ Push 스킵"
}

Write-Host "✅ 릴리즈 완료: $newTag"