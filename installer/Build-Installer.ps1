[CmdletBinding()]
param([string]$Runtime = "win-x64", [string]$Configuration = "Release", [string]$IsccPath, [string]$OutputRoot,
      [Parameter(Mandatory)][string]$FreezeRecord, [Parameter(Mandatory)][string]$BuildNumber, [string]$PythonPath = "python")
$ErrorActionPreference = "Stop"
$installerDir = $PSScriptRoot
$repoDir = Split-Path $installerDir -Parent
if ($Runtime -ne "win-x64" -or $Configuration -ne "Release") { throw "Freeze build settings mismatch." }
$FreezeRecord = [IO.Path]::GetFullPath($FreezeRecord)
& $PythonPath (Join-Path $repoDir "release/versioning.py") validate --freeze $FreezeRecord
if ($LASTEXITCODE -ne 0) { throw "Freeze validation failed; no output modified." }
$env:DOTNET_CLI_HOME = Join-Path $repoDir ".dotnet-home"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "1"
if (-not $OutputRoot) { $OutputRoot = Join-Path $repoDir ("artifacts\release\" + $BuildNumber) }
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$repoBoundary = [IO.Path]::GetFullPath($repoDir).TrimEnd('\') + '\'
if (-not $OutputRoot.StartsWith($repoBoundary, [StringComparison]::OrdinalIgnoreCase)) { throw "Installer output must be inside this repository." }
$publishDir = Join-Path $OutputRoot "publish"
$stageDir = Join-Path $OutputRoot "stage"
$setupOutput = Join-Path $OutputRoot "output"
if (Test-Path -LiteralPath (Join-Path $setupOutput "VCuttingSetup.exe")) { throw "An installer already exists at this output root; choose a new output root." }
$versionDir = Join-Path $OutputRoot "version"
& $PythonPath (Join-Path $repoDir "release/versioning.py") prepare --freeze $FreezeRecord --output $versionDir --build-number $BuildNumber
if ($LASTEXITCODE -ne 0) { throw "Freeze preparation failed." }
$freezeProps = Join-Path $versionDir "Freeze.props"
$releaseDefinitions = Join-Path $versionDir "ReleaseVersion.iss"
function Reset-Directory([string]$Path) {
    $resolved = [IO.Path]::GetFullPath($Path)
    if ($resolved -notin @($publishDir, $stageDir) -or -not $resolved.StartsWith($OutputRoot.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) { throw "Unsafe installer reset path: $resolved" }
    if (Test-Path -LiteralPath $Path) { Remove-Item -LiteralPath $Path -Recurse -Force }
    New-Item -ItemType Directory -Path $Path | Out-Null
}
Reset-Directory $publishDir
Reset-Directory $stageDir
$projects = @(
    @{ Name = "viewer"; Path = "VCutting.Viewer\VCutting.Viewer.csproj" },
    @{ Name = "explorer"; Path = "VCutting.Explorer\VCutting.Explorer.csproj" },
    @{ Name = "simulator"; Path = "VCutting.Simulator\VCutting.Simulator.csproj" },
    @{ Name = "drawer"; Path = "VCutting.Drawer\VCutting.Drawer.csproj" },
    @{ Name = "cosmic"; Path = "VCutting\VCutting.csproj" },
    @{ Name = "convert"; Path = "CosmicConvert\CosmicConvert.csproj" }
)
foreach ($project in $projects) {
    $projectPath = Join-Path $repoDir $project.Path
    $projectOutput = Join-Path $publishDir $project.Name
    dotnet publish $projectPath -c $Configuration -r $Runtime --self-contained true `
        -p:PublishSingleFile=false -p:DebugType=None -p:DebugSymbols=false `
        "-p:VCuttingFreezeProps=$freezeProps" "-p:VCuttingReleasePython=$PythonPath" `
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
foreach ($name in @("VCutting.Explorer.exe", "VCutting.Viewer.exe", "VCutting.Simulator.exe", "VCutting.Drawer.exe", "CosmicDesigner.exe", "CosmicConvert.exe")) {
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
# ISCC's PE version is 0.0.0.0 and /? omits the engine version. The guarded
# source deliberately stops in preprocessing, before any package can be emitted.
$compilerInfo = (& $iscc (Join-Path $installerDir "ReleaseVersion.iss") 2>&1 | Out-String)
$expectedCompiler = (Get-Content -LiteralPath $FreezeRecord -Raw | ConvertFrom-Json).record.toolchain.innoSetup
if ($compilerInfo -notmatch ("Compiler engine version: (?:Inno Setup )?" + [regex]::Escape($expectedCompiler) + "(?:\r?\n|$)")) { throw "Freeze installer compiler mismatch." }
New-Item -ItemType Directory -Path $setupOutput -Force | Out-Null
& $PythonPath (Join-Path $repoDir "release/versioning.py") validate --freeze $FreezeRecord --definitions $releaseDefinitions
if ($LASTEXITCODE -ne 0) { throw "Freeze changed during publishing." }
& $iscc "/DStageRoot=$stageDir" "/DReleaseVersionFile=$releaseDefinitions" "/O$setupOutput" (Join-Path $installerDir "VCutting.iss")
if ($LASTEXITCODE -ne 0) { throw "Inno Setup compilation failed." }
$setup = Join-Path $setupOutput "VCuttingSetup.exe"
if (-not (Test-Path -LiteralPath $setup)) { throw "Installer output was not created: $setup" }
& (Join-Path $installerDir "Verify-Installer.ps1") -Path $setup -Definitions $releaseDefinitions
& $PythonPath (Join-Path $repoDir "release/versioning.py") record --freeze $FreezeRecord --build-info (Join-Path $versionDir "build.json") --artifact $setup
if ($LASTEXITCODE -ne 0) { throw "Build evidence recording failed." }
$helpSource = $stageDir
$outputDir = Split-Path $setup -Parent
Get-ChildItem -LiteralPath $helpSource -File -Filter "*_help.html" | Copy-Item -Destination $outputDir -Force
Copy-Item -LiteralPath (Join-Path $helpSource "help") -Destination $outputDir -Recurse -Force
Get-Item -LiteralPath $setup | Select-Object FullName, Length, LastWriteTime


