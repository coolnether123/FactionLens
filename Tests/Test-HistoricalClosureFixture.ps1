[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$AssemblyPath,

    [Parameter(Mandatory = $true)]
    [string]$CecilPath,

    [Parameter(Mandatory = $true)]
    [ValidateSet('legacy', 'modern')]
    [string]$Surface
)

$ErrorActionPreference = 'Stop'
Add-Type -Path $CecilPath
$reader = New-Object Mono.Cecil.ReaderParameters
$reader.ReadSymbols = $false
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($AssemblyPath, $reader)
$type = $assembly.MainModule.GetType(
    'FactionLens.HistoricalClosureFixture.HistoricalClosureFixture')
if ($null -eq $type) {
    throw 'Historical closure fixture type is missing.'
}

$cctor = @($type.Methods | Where-Object IsConstructor | Where-Object IsStatic)
if ($cctor.Count -ne 1) {
    throw "Expected one deterministic fixture static constructor, found $($cctor.Count)."
}

$body = ($cctor[0].Body.Instructions | ForEach-Object { $_.ToString() }) -join "`n"
if ($Surface -eq 'legacy') {
    foreach ($name in @('SpineLegacyBootstrap', 'FactionLensLegacyBootstrap', 'RunClassConstructor')) {
        if ($body -notmatch [regex]::Escape($name)) {
            throw "Legacy fixture does not force expected loader surface '$name'."
        }
    }
} else {
    foreach ($name in @('FactionLensMod', 'ReadModSettings', 'ContextualSettings')) {
        if ($body -notmatch [regex]::Escape($name)) {
            throw "Modern fixture does not exercise expected contract '$name'."
        }
    }
}

Write-Output "PASS: historical closure fixture has a deterministic $Surface loader contract"
