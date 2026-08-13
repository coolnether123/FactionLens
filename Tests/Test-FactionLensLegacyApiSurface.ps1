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
    [string]$CecilPath
)

$ErrorActionPreference = 'Stop'

foreach ($path in @($AssemblyPath, $ManagedDir, $HarmonyPath, $SpinePath, $CecilPath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required FactionLens 1.0 API-surface input does not exist: $path"
    }
}

Add-Type -Path $CecilPath
$resolver = New-Object Mono.Cecil.DefaultAssemblyResolver
$resolver.AddSearchDirectory((Split-Path -Parent $AssemblyPath))
$resolver.AddSearchDirectory($ManagedDir)
$resolver.AddSearchDirectory((Split-Path -Parent $HarmonyPath))
$resolver.AddSearchDirectory((Split-Path -Parent $SpinePath))
$reader = New-Object Mono.Cecil.ReaderParameters
$reader.AssemblyResolver = $resolver
$reader.ReadSymbols = $false
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($AssemblyPath, $reader)

function Get-TypeDefinition([string]$fullName) {
    $type = $assembly.MainModule.GetType($fullName)
    if ($null -eq $type) { throw "Missing FactionLens type: $fullName" }
    return $type
}

function Require-PublicMethod([Mono.Cecil.TypeDefinition]$type, [string]$name) {
    if (@($type.Methods | Where-Object { $_.IsPublic -and $_.Name -eq $name }).Count -eq 0) {
        throw "Missing public method $name on $($type.FullName)"
    }
}

$api = Get-TypeDefinition 'FactionLens.Api.FactionLensApi'
Require-PublicMethod $api 'RegisterOwnershipResolver'
Require-PublicMethod $api 'UnregisterOwnershipResolver'
$resolution = Get-TypeDefinition 'FactionLens.Api.OwnershipResolution'
Require-PublicMethod $resolution 'get_Kind'
Require-PublicMethod $resolution 'get_Faction'
Require-PublicMethod $resolution 'get_NotHandled'
Require-PublicMethod $resolution 'get_Unknown'
Require-PublicMethod $resolution 'Disclosed'
Require-PublicMethod (Get-TypeDefinition 'FactionLens.Api.OwnershipResolver') 'Invoke'
[void](Get-TypeDefinition 'FactionLens.Api.OwnershipResolutionKind')
[void](Get-TypeDefinition 'FactionLens.Bootstrap.FactionLensMod')

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
        throw "FactionLens 1.0 retains forbidden old-CLR reference: $name"
    }
}

$assemblyRefs = @($assembly.MainModule.AssemblyReferences | ForEach-Object { $_.Name })
foreach ($requiredReference in @('mscorlib', 'System', 'System.Core', 'Assembly-CSharp', 'UnityEngine', '0Harmony', 'Spine')) {
    if ($assemblyRefs -notcontains $requiredReference) {
        throw "FactionLens 1.0 is missing authoritative runtime reference: $requiredReference"
    }
}

Write-Output ("PASS: FactionLens 1.0 API surface and old-runtime references resolve " +
    "against managed dir '$ManagedDir', Harmony '$HarmonyPath', and Spine '$SpinePath'")
