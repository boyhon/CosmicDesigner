[CmdletBinding()]
param([Parameter(Mandatory)][string]$Path, [Parameter(Mandatory)][string]$Definitions)
$ErrorActionPreference = "Stop"
$definitions = Get-Content -LiteralPath $Definitions -Raw
function Read-Define([string]$Name) {
    $match = [regex]::Match($definitions, ('(?m)^#define ' + [regex]::Escape($Name) + ' "([^"]+)"'))
    if (-not $match.Success) { throw "Missing release definition: $Name" }
    return $match.Groups[1].Value
}
$expectedName = (Read-Define "SetupBaseFilename") + ".exe"
$expectedVersion = Read-Define "AppVersion"
$expectedNumeric = Read-Define "AppNumericVersion"
$file = Get-Item -LiteralPath $Path
if ($file.Name -cne $expectedName) { throw "Wrong installer filename: $($file.Name)" }
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($file.FullName)
$numeric = "$($version.FileMajorPart).$($version.FileMinorPart).$($version.FileBuildPart).$($version.FilePrivatePart)"
if ($numeric -ne $expectedNumeric) { throw "Wrong installer numeric file version: $numeric" }
if ($version.FileVersion.Trim() -ne $expectedVersion) { throw "Wrong installer displayed file version: $($version.FileVersion)" }
if ($version.ProductVersion.Trim() -ne $expectedVersion) { throw "Wrong installer product version: $($version.ProductVersion)" }
Write-Output "PASS TC-065-001: $expectedName; FileVersion=$expectedNumeric; ProductVersion=$expectedVersion"
