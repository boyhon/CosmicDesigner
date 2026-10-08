[CmdletBinding()]
param([Parameter(Mandatory)][string]$Root)
$ErrorActionPreference = "Stop"
$rootPath = (Resolve-Path -LiteralPath $Root).Path
$required = @("VCuttingSetup_help.html", "VCutting.Explorer_help.html", "VCutting.Viewer_help.html", "VCutting.Simulator_help.html", "VCutting.Drawer_help.html", "VCutting_help.html", "help\index.html", "help\getting-started.html", "help\reference\shortcuts.html", "help\troubleshooting\common-errors.html", "help\css\help.css", "help\js\help.js")
foreach ($relative in $required) {
    if (-not (Test-Path -LiteralPath (Join-Path $rootPath $relative))) { throw "Required Help file is missing: $relative" }
}
if (Get-ChildItem -LiteralPath $rootPath -Recurse -File | Where-Object Name -Match "hepl") { throw "A Help filename contains the forbidden 'hepl' typo." }
$htmlFiles = Get-ChildItem -LiteralPath $rootPath -Recurse -Filter "*.html" -File
foreach ($html in $htmlFiles) {
    $content = Get-Content -LiteralPath $html.FullName -Raw
    if ($content -match '(?i)(href|src)\s*=\s*["'']https?://') { throw "External dependency found: $($html.FullName)" }
    foreach ($match in [regex]::Matches($content, '(?i)(?:href|src)\s*=\s*["'']([^"''#]+)(?:#[^"'']*)?["'']')) {
        $target = $match.Groups[1].Value
        if ($target -match '^(?:mailto:|javascript:|data:)') { continue }
        $resolved = [System.IO.Path]::GetFullPath((Join-Path $html.DirectoryName $target))
        if (-not $resolved.StartsWith($rootPath.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) { throw "Help link escapes root: $target in $($html.Name)" }
        if (-not (Test-Path -LiteralPath $resolved)) { throw "Broken Help link: $target in $($html.FullName)" }
    }
}
Write-Host "Help verification passed: $($htmlFiles.Count) HTML files, all local links resolved."
