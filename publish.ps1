<#
.SYNOPSIS
csproj Version 기반 자동 publish 스크립트

.DESCRIPTION
- SMP.csproj의 <Version> 값을 읽어서 사용
- AssemblyVersion / FileVersion 동기화
- self-contained single file publish
- Git과 완전히 분리된 빌드 산출 스크립트
#>

param (
    [string]$ProjectPath = "SMP/SMP.csproj",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
    #[string]$OutputPath = "Output"
)

# -----------------------------
# csproj Version 추출 (안정화)
# -----------------------------
function Get-VersionFromCsproj {
    param ([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "csproj not found: $Path"
    }

    # XML 안정 로딩
    [xml]$xml = [xml](Get-Content $Path -Raw)

    foreach ($pg in $xml.Project.PropertyGroup) {
        if ($pg.Version -and -not [string]::IsNullOrWhiteSpace($pg.Version)) {
            return $pg.Version.Trim()
        }
    }

    throw "<Version> not found in csproj"
}

# -----------------------------
# Version 읽기
# -----------------------------
$VERSION = Get-VersionFromCsproj -Path $ProjectPath

Write-Host "📌 csproj Version: $VERSION"

# -----------------------------
# Version 포맷 검증 및 분리
# -----------------------------
if ($VERSION -notmatch '^\d+\.\d+\.\d+$') {
    throw "Invalid version format (expected x.y.z): $VERSION"
}

$versionParts = $VERSION.Split(".")

$assemblyVersion = "$($versionParts[0]).$($versionParts[1]).$($versionParts[2]).0"
$fileVersion     = "$($versionParts[0]).$($versionParts[1]).$($versionParts[2]).0"

Write-Host "📌 AssemblyVersion: $assemblyVersion"
Write-Host "📌 FileVersion: $fileVersion"

# -----------------------------
# Output 폴더 정리
# -----------------------------
if (Test-Path $OutputPath) {
    try {
        Remove-Item $OutputPath -Recurse -Force -ErrorAction Stop
    } catch {
        throw "Failed to clean output folder: $OutputPath"
    }
}

New-Item -ItemType Directory -Path $OutputPath | Out-Null

# -----------------------------
# Publish 실행
# -----------------------------
Write-Host "🚀 Starting publish..."

& dotnet publish $ProjectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -o $OutputPath `
    /p:PublishSingleFile=true `
    /p:Version=$VERSION `
    /p:AssemblyVersion=$assemblyVersion `
    /p:FileVersion=$fileVersion

# -----------------------------
# 결과 확인
# -----------------------------
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed"
}

Write-Host "✅ Publish completed successfully"
Write-Host "📦 Output: $OutputPath"