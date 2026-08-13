[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Configuration
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$toolingRoot = $env:RWT_CASCADE_TOOLING_ROOT
$outputRoot = $env:RWT_CASCADE_BUILD_OUTPUT_ROOT
if ([string]::IsNullOrWhiteSpace($toolingRoot) -or
    [string]::IsNullOrWhiteSpace($outputRoot))
{
    throw 'Faction Lens build must run through the Cascade executor.'
}

$spineAssembly = Join-Path `
    (Join-Path $repoRoot '..\Spine') `
    "$Configuration\Assemblies\Spine.dll"
if (-not (Test-Path -LiteralPath $spineAssembly -PathType Leaf))
{
    throw "The matching SpineLib payload must be built first: $spineAssembly"
}

Import-Module (Join-Path $toolingRoot 'modules\RimWorld.Tooling.Build\RimWorld.Tooling.Build.psd1') -Force
Import-Module (Join-Path $toolingRoot 'modules\RimWorld.Tooling.Depot\RimWorld.Tooling.Depot.psd1') -Force

$environment = Resolve-RwtEnvironment `
    -Version $Configuration `
    -Purpose Compile `
    -Dependency @('harmony') `
    -VersionManifestPath (Join-Path $toolingRoot 'manifests\rimworld-versions.json') `
    -DependencyManifestPath (Join-Path $toolingRoot 'manifests\dependencies.json')
$environment.Dependencies = @($environment.Dependencies) + [PSCustomObject]@{
    Id = 'spine'
    Path = $spineAssembly
    Sha256 = (Get-FileHash -LiteralPath $spineAssembly -Algorithm SHA256).Hash
}

$result = Invoke-RwtBuild `
    -Project (Join-Path $repoRoot 'Source\Mod.csproj') `
    -Configuration $Configuration `
    -Environment $environment `
    -OutputRoot $outputRoot `
    -Engine DotNet
if (-not $result.Succeeded)
{
    throw "Faction Lens build failed for ${Configuration}: $($result.ExitCode)."
}

$artifact = Join-Path $outputRoot 'build\FactionLens.dll'
if (-not (Test-Path -LiteralPath $artifact -PathType Leaf))
{
    throw "Expected Faction Lens artifact is missing: $artifact"
}
$payloadRoot = Join-Path $repoRoot "$Configuration\Assemblies"
New-Item -ItemType Directory -Path $payloadRoot -Force | Out-Null
Copy-Item -LiteralPath $artifact -Destination (Join-Path $payloadRoot 'FactionLens.dll') -Force
