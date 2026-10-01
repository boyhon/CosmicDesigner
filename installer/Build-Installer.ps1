[CmdletBinding()]
param([string]$Runtime = "win-x64", [string]$Configuration = "Release", [string]$IsccPath)
$ErrorActionPreference = "Stop"
$installerDir = $PSScriptRoot
$repoDir = Split-Path $installerDir -Parent
$env:DOTNET_CLI_HOME = Join-Path $repoDir ".dotnet-home"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
$publishDir = Join-Path $installerDir "publish"
$stageDir = Join-Path $installerDir "stage"
function Reset-Directory([string]$Path) {
    if (Test-Path -LiteralPath $Path) { Remove-Item -LiteralPath $Path -Recurse -Force }
    New-Item -ItemType Directory -Path $Path | Out-Null
}
Reset-Directory $publishDir
Reset-Directory $stageDir
$projects = @(
    @{ Name = "viewer"; Path = "DXFExplorer\DXFExplorer.csproj" },
    @{ Name = "explorer"; Path = "DXFFileExplorer\DXFFileExplorer.csproj" },
    @{ Name = "simulator"; Path = "DXFSimulater\DXFSimulater.csproj" },
    @{ Name = "drawer"; Path = "DXFDrawer\DXFDrawer.csproj" }
)
foreach ($project in $projects) {
    $projectPath = Join-Path $repoDir $project.Path
    $projectOutput = Join-Path $publishDir $project.Name
    dotnet publish $projectPath -c $Configuration -r $Runtime --self-contained true `
        -p:PublishSingleFile=false -p:DebugType=None -p:DebugSymbols=false `
        -o $projectOutput --configfile (Join-Path $installerDir "NuGet.Publish.Config")
    if ($LASTEXITCODE -ne 0) { throw "Publish failed: $($project.Path)" }
}
foreach ($project in $projects) {
    $sourceRoot = Join-Path $publishDir $project.Name
    Get-ChildItem -LiteralPath $sourceRoot -Recurse -File | ForEach-Object {
        $relative = $_.FullName.Substring($sourceRoot.Length).TrimStart('\')
        $destination = Join-Path $stageDir $relative
        if (Test-Path -LiteralPath $destination) {
            $sourceHash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
            $destinationHash = (Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash
            if ($sourceHash -ne $destinationHash) { throw "Publish collision with different content: $relative" }
        } else {
            New-Item -ItemType Directory -Path (Split-Path $destination -Parent) -Force | Out-Null
            Copy-Item -LiteralPath $_.FullName -Destination $destination
        }
    }
}
foreach ($name in @("DXFExplorer.exe", "DXFViewer.exe", "DXFSimulator.exe", "DXFDrawer.exe")) {
    if (-not (Test-Path -LiteralPath (Join-Path $stageDir $name))) { throw "Required executable is missing: $name" }
}
& (Join-Path $installerDir "Verify-Help.ps1") -Root $stageDir
if ($LASTEXITCODE -ne 0) { throw "Help verification failed." }
$isccCandidates = @(
    $IsccPath,
    (Get-Command ISCC.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source -First 1),
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }
$iscc = $isccCandidates | Select-Object -First 1
if (-not $iscc) { throw "Inno Setup 6 compiler (ISCC.exe) was not found." }
& $iscc (Join-Path $installerDir "DXFExplorer.iss")
if ($LASTEXITCODE -ne 0) { throw "Inno Setup compilation failed." }
$setup = Join-Path $installerDir "output\DXFExplorerSetup.exe"
if (-not (Test-Path -LiteralPath $setup)) { throw "Installer output was not created: $setup" }
$helpSource = Join-Path $repoDir "help-content"
$outputDir = Split-Path $setup -Parent
Get-ChildItem -LiteralPath $helpSource -File | Copy-Item -Destination $outputDir -Force
Copy-Item -LiteralPath (Join-Path $helpSource "help") -Destination $outputDir -Recurse -Force
Get-Item -LiteralPath $setup | Select-Object FullName, Length, LastWriteTime


