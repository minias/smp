<#
.SYNOPSIS
csproj Version 기반 자동 publish 스크립트

.DESCRIPTION
- SMP.csproj의 <Version> 값을 읽어서 사용
- AssemblyVersion / FileVersion 동기화
- self-contained single file publish
#>

param (
    [string]$ProjectPath = "SMP/SMP.csproj",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputPath = "Output"
)

# -----------------------------
# csproj에서 Version 추출
# -----------------------------
function Get-VersionFromCsproj {
    param ([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "csproj not found: $Path"
    }

    # XML 로드
    [xml]$xml = Get-Content $Path

    # Version 노드 탐색
    $versionNode = $xml.Project.PropertyGroup.Version

    if (-not $versionNode) {
        throw "<Version> not found in csproj"
    }

    $version = $versionNode.Trim()

    if ([string]::IsNullOrWhiteSpace($version)) {
        throw "<Version> is empty"
    }

    return $version
}

# -----------------------------
# Version 읽기
# -----------------------------
$VERSION = Get-VersionFromCsproj -Path $ProjectPath

Write-Host "📌 csproj Version: $VERSION"

# -----------------------------
# Output 폴더 준비
# -----------------------------
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $OutputPath | Out-Null

# -----------------------------
# Publish 실행
# -----------------------------
dotnet publish $ProjectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -o $OutputPath `
    /p:PublishSingleFile=true `
    /p:Version=$VERSION `
    /p:AssemblyVersion="$VERSION.0" `
    /p:FileVersion="$VERSION.0"

# -----------------------------
# 결과 확인
# -----------------------------
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed"
}

Write-Host "✅ Publish completed successfully"
Write-Host "📦 Output: $OutputPath"