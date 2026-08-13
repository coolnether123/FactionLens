[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$AssemblyPath,

    [Parameter(Mandatory = $true)]
    [string]$ManagedDir,

    [Parameter(Mandatory = $true)]
    [string]$HarmonyPath,

    [Parameter(Mandatory = $true)]
    [string]$SpinePath,

    [Parameter(Mandatory = $true)]
    [string]$FactionLensPath,

    [Parameter(Mandatory = $true)]
    [string]$CecilPath
)

$ErrorActionPreference = 'Stop'

foreach ($path in @($AssemblyPath, $ManagedDir, $HarmonyPath, $SpinePath, $FactionLensPath, $CecilPath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required legacy fixture input does not exist: $path"
    }
}

Add-Type -Path $CecilPath
$resolver = New-Object Mono.Cecil.DefaultAssemblyResolver
$resolver.AddSearchDirectory((Split-Path -Parent $AssemblyPath))
$resolver.AddSearchDirectory($ManagedDir)
$resolver.AddSearchDirectory((Split-Path -Parent $HarmonyPath))
$resolver.AddSearchDirectory((Split-Path -Parent $SpinePath))
$resolver.AddSearchDirectory((Split-Path -Parent $FactionLensPath))
$reader = New-Object Mono.Cecil.ReaderParameters
$reader.AssemblyResolver = $resolver
$reader.ReadSymbols = $false
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($AssemblyPath, $reader)

function Get-TypeDefinition([string]$fullName) {
    $type = $assembly.MainModule.GetType($fullName)
    if ($null -eq $type) { throw "Missing legacy fixture type: $fullName" }
    return $type
}

function Require-PublicMethod([Mono.Cecil.TypeDefinition]$type, [string]$name) {
    if (@($type.Methods | Where-Object { $_.IsPublic -and $_.Name -eq $name }).Count -eq 0) {
        throw "Missing public fixture method $name on $($type.FullName)"
    }
}

$component = Get-TypeDefinition 'FactionLens.LegacyContractFixture.FactionLensLegacyContractGameComponent'
Require-PublicMethod $component '.ctor'
Require-PublicMethod $component 'GameComponentTick'
Require-PublicMethod $component 'ExposeData'
[void](Get-TypeDefinition 'FactionLens.LegacyContractFixture.FactionLensLegacyContractFixture')

$forbidden = @(
    'System.Runtime.CompilerServices.IsReadOnlyAttribute',
    'System.Runtime.Versioning.TargetFrameworkAttribute',
    'System.Collections.Generic.IReadOnlyList`1',
    'System.Collections.Generic.IReadOnlyCollection`1',
    'System.Func`6'
)
$references = @($assembly.MainModule.GetTypeReferences() | ForEach-Object { $_.FullName })
foreach ($name in $forbidden) {
    if ($references -contains $name) {
        throw "Legacy fixture retains forbidden old-CLR reference: $name"
    }
}

$assemblyRefs = @($assembly.MainModule.AssemblyReferences | ForEach-Object { $_.Name })
foreach ($requiredReference in @('mscorlib', 'Assembly-CSharp', 'UnityEngine', '0Harmony', 'Spine', 'FactionLens')) {
    if ($assemblyRefs -notcontains $requiredReference) {
        throw "Legacy fixture is missing authoritative runtime reference: $requiredReference"
    }
}

if ($assemblyRefs -contains 'RimWorld Agent') {
    throw 'Legacy 1.0 fixture must not depend on the modern Agent extension ABI'
}

Write-Output "PASS: FactionLens 1.0 legacy contract fixture API and old-CLR references are deterministic"
