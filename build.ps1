param(
    [string]$Configuration = "Release",
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSCommandPath
if (-not $OutputPath) {
    $OutputPath = Join-Path "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods" "Virtu-Awake"
}

$project = Join-Path (Join-Path $root "Source") "VirtuAwake.csproj"
Write-Host "Building $project ($Configuration)..."
dotnet build $project -c $Configuration

$buildDir = Join-Path (Join-Path (Join-Path (Join-Path $root "Source") "bin") $Configuration) "net472"
$dll = Join-Path $buildDir "VirtuAwake.dll"
if (-not (Test-Path $dll)) {
    throw "Build output not found at $dll"
}

$assemblyOut = Join-Path $OutputPath "Assemblies"
New-Item -ItemType Directory -Force -Path $assemblyOut | Out-Null
Copy-Item -Path $dll -Destination $assemblyOut -Force
$pdb = Join-Path $buildDir "VirtuAwake.pdb"
if (Test-Path $pdb) {
    Copy-Item -Path $pdb -Destination $assemblyOut -Force
}

$contentDirs = @("About", "Defs", "Textures", "Languages")
foreach ($dir in $contentDirs) {
    $sourceDir = Join-Path $root $dir
    if (-not (Test-Path $sourceDir)) {
        continue
    }

    $destDir = Join-Path $OutputPath $dir
    New-Item -ItemType Directory -Force -Path $destDir | Out-Null
    Copy-Item -Path (Join-Path $sourceDir "*") -Destination $destDir -Recurse -Force
}

Write-Host "Packaged Virtu-Awake to $OutputPath"
