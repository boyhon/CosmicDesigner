[CmdletBinding()]
param([Parameter(Mandatory)][string]$BuildNumber, [Parameter(Mandatory)][string]$OutputRoot,
      [string]$IsccPath = 'C:\tmp\InnoSetup6\ISCC.exe', [switch]$ValidateOnly)
$ErrorActionPreference = 'Stop'
$repoDir = Split-Path $PSScriptRoot -Parent
Set-Location -LiteralPath $repoDir
if ($BuildNumber -notmatch '^[A-Za-z0-9_-]+$') { throw 'Invalid integration build identifier.' }
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$artifactBoundary = [IO.Path]::GetFullPath((Join-Path $repoDir 'artifacts')).TrimEnd('\') + '\'
if (-not $OutputRoot.StartsWith($artifactBoundary, [StringComparison]::OrdinalIgnoreCase)) { throw 'Integration output must be inside repository artifacts.' }
if (Test-Path -LiteralPath $OutputRoot) { throw 'Output root already exists; select a new build/output root.' }
$ledger = Join-Path $repoDir "release/builds/development/$BuildNumber.json"
if (Test-Path -LiteralPath $ledger) { throw 'Integration build identifier already recorded.' }
[xml]$policy = Get-Content -LiteralPath (Join-Path $repoDir 'release/Version.props') -Raw
$developmentVersion = [string]$policy.Project.PropertyGroup.DevelopmentVersion
$numericVersion = "$developmentVersion.0"
if ($ValidateOnly) { @{ kind='INTEGRATION_TEST'; customerVersion='Development'; numericVersion=$numericVersion; output=$OutputRoot } | ConvertTo-Json; return }
$dirty = @(git status --porcelain --untracked-files=all | Where-Object { $_.Substring(3) -notmatch '^release/builds/' })
if ($LASTEXITCODE -ne 0 -or $dirty.Count) { throw 'Commit source before building an integration installer.' }
if (-not (Test-Path -LiteralPath $IsccPath)) { throw 'Inno Setup compiler missing.' }
$compilerInfo = (& $IsccPath (Join-Path $PSScriptRoot 'ReleaseVersion.iss') 2>&1 | Out-String)
if ($compilerInfo -notmatch ('Compiler engine version: (?:Inno Setup )?' + [regex]::Escape([string]$policy.Project.PropertyGroup.InstallerCompilerVersion) + '(?:\r?\n|$)')) { throw 'Installer compiler version mismatch.' }
$sourceCommit = (git rev-parse HEAD).Trim()
$buildIdentity = "$sourceCommit.integration.$BuildNumber"
$sourceInputs = @{}
foreach ($name in @(git -c core.quotepath=false ls-files)) {
    if ($name -notmatch '^release/(builds|freezes)/' -and (Test-Path -LiteralPath $name -PathType Leaf)) { $sourceInputs[$name] = (Get-FileHash -LiteralPath $name -Algorithm SHA256).Hash }
}
New-Item -ItemType Directory -Path $OutputRoot | Out-Null
$stage = Join-Path $OutputRoot 'stage'
$output = Join-Path $OutputRoot 'output'
New-Item -ItemType Directory -Path $stage,$output | Out-Null
$projects = @('VCutting.Viewer','VCutting.Explorer','VCutting.Simulator','VCutting.Drawer','VCutting','CosmicConvert')
foreach ($project in $projects) {
    $publish = Join-Path $OutputRoot "publish/$project"
    & dotnet publish (Join-Path $repoDir "$project/$project.csproj") -c Release -r win-x64 --self-contained true `
        -p:PublishSingleFile=false -p:DebugType=None -p:DebugSymbols=false `
        '-p:VCuttingDisplayVersion=Development' "-p:VCuttingNumericVersion=$numericVersion" `
        '-p:VCuttingCustomerVersion=' '-p:VCuttingFreezeProps=' "-p:VCuttingBuildIdentity=$buildIdentity" `
        -o $publish --configfile (Join-Path $PSScriptRoot 'NuGet.Publish.Config')
    if ($LASTEXITCODE -ne 0) { throw "Integration publish failed: $project" }
    foreach ($file in Get-ChildItem -LiteralPath $publish -Recurse -File) {
        $relative = $file.FullName.Substring($publish.Length).TrimStart('\')
        $destination = Join-Path $stage $relative
        if (Test-Path -LiteralPath $destination) {
            if ((Get-FileHash -LiteralPath $file.FullName).Hash -ne (Get-FileHash -LiteralPath $destination).Hash) { throw "Publish collision: $relative" }
        } else {
            New-Item -ItemType Directory -Path (Split-Path $destination -Parent) -Force | Out-Null
            Copy-Item -LiteralPath $file.FullName -Destination $destination
        }
    }
}
$metadata = @{}
foreach ($name in @('VCutting.Viewer','VCutting.Explorer','VCutting.Simulator','VCutting.Drawer','CosmicDesigner','CosmicConvert')) {
    if (-not (Test-Path -LiteralPath (Join-Path $stage "$name.exe"))) { throw "Missing executable: $name" }
    $assembly = [Reflection.Assembly]::LoadFrom((Join-Path $stage "$name.dll"))
    $values = @{}
    foreach ($attribute in $assembly.GetCustomAttributesData() | Where-Object { $_.AttributeType.Name -eq 'AssemblyMetadataAttribute' }) { $values[[string]$attribute.ConstructorArguments[0].Value] = [string]$attribute.ConstructorArguments[1].Value }
    if ($values.CustomerVersion -ne 'Development' -or $values.DevelopmentVersion -ne $developmentVersion -or $values.BuildIdentity -ne $buildIdentity) { throw "Integration metadata mismatch: $name" }
    $version = [Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $stage "$name.dll"))
    if ("$($version.FileMajorPart).$($version.FileMinorPart).$($version.FileBuildPart).$($version.FilePrivatePart)" -ne $numericVersion) { throw "Integration numeric version mismatch: $name" }
    $metadata[$name] = $values
}
if ((Get-Content -LiteralPath (Join-Path $stage 'help/js/version.js') -Raw).Trim() -ne 'window.VCuttingVersion = "Development";') { throw 'Integration Help version mismatch.' }
& (Join-Path $PSScriptRoot 'Verify-Help.ps1') -Root $stage
foreach ($name in $sourceInputs.Keys) { if ((Get-FileHash -LiteralPath $name).Hash -ne $sourceInputs[$name]) { throw "Source changed during build: $name" } }
if ((git rev-parse HEAD).Trim() -ne $sourceCommit) { throw 'Source commit changed during build.' }
$finalDirty = @(git status --porcelain --untracked-files=all | Where-Object { $_.Substring(3) -notmatch '^release/builds/' })
if ($LASTEXITCODE -ne 0 -or $finalDirty.Count) { throw 'Source tree changed during integration build.' }
$definitions = Join-Path $OutputRoot 'IntegrationVersion.iss'
@('#define AppVersion "Development"', "#define AppNumericVersion `"$numericVersion`"", '#define SetupBaseFilename "VCuttingSetup-IntegrationTest"') | Set-Content -LiteralPath $definitions -Encoding utf8
& $IsccPath "/DStageRoot=$stage" "/DReleaseVersionFile=$definitions" "/O$output" (Join-Path $PSScriptRoot 'VCutting.iss')
if ($LASTEXITCODE -ne 0) { throw 'Integration installer compilation failed.' }
$setup = Join-Path $output 'VCuttingSetup-IntegrationTest.exe'
& (Join-Path $PSScriptRoot 'Verify-Installer.ps1') -Path $setup -Definitions $definitions
$checksums = @(Get-ChildItem -LiteralPath $stage -Recurse -File | ForEach-Object { @{ path=$_.FullName.Substring($stage.Length+1); sha256=(Get-FileHash -LiteralPath $_.FullName).Hash; bytes=$_.Length } })
$evidence = @{ kind='INTEGRATION_TEST'; customerVersion='Development'; developmentVersion=$developmentVersion; numericVersion=$numericVersion; sourceCommit=$sourceCommit; buildNumber=$BuildNumber; buildIdentity=$buildIdentity; sourceInputs=$sourceInputs; assemblyMetadata=$metadata; stageChecksums=$checksums; installer=@{ path=$setup; bytes=(Get-Item -LiteralPath $setup).Length; sha256=(Get-FileHash -LiteralPath $setup).Hash }; builtAt=[DateTime]::UtcNow.ToString('o'); humanVerification='PENDING_MANUAL'; freezeAllocated=$false }
New-Item -ItemType Directory -Path (Split-Path $ledger -Parent) -Force | Out-Null
$json = $evidence | ConvertTo-Json -Depth 8
$stream = [IO.File]::Open($ledger,[IO.FileMode]::CreateNew)
try { $bytes=[Text.Encoding]::UTF8.GetBytes($json);$stream.Write($bytes,0,$bytes.Length) } finally { $stream.Dispose() }
$json | Set-Content -LiteralPath (Join-Path $OutputRoot 'build-evidence.json') -Encoding utf8
Get-Item -LiteralPath $setup | Select-Object FullName,Length
Write-Output "BuildIdentity=$buildIdentity"
