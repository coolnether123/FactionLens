param()

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$patch = [System.IO.File]::ReadAllText(
    (Join-Path $root 'Source\Patches\WorldLabelPatch.cs'))
$overlay = [System.IO.File]::ReadAllText(
    (Join-Path $root 'Source\Presentation\WorldLabelOverlay.cs'))

$failures = [System.Collections.Generic.List[string]]::new()
if ($patch -notmatch
    'prefix:\s*prefix' -or
    $patch -notmatch
    'BeforeExpandableWorldObjectsOnGui')
{
    $failures.Add(
        'Label clicks must run before vanilla can consume MouseDown.')
}
if ($overlay -notmatch
    'EventType\.MouseDown' -or
    $overlay -notmatch
    'currentEvent\.button\s*!=\s*0')
{
    $failures.Add(
        'Only left-button MouseDown may select a label.')
}
if ($overlay -notmatch
    'Find\.WorldSelector\.ClearSelection\(\)' -or
    $overlay -notmatch
    'Find\.WorldSelector\.Select\(')
{
    $failures.Add(
        'Label clicks must replace selection through WorldSelector.')
}
if ($overlay -notmatch
    'Event\.current\.Use\(\)')
{
    $failures.Add(
        'A handled nameplate click must be consumed.')
}

if ($failures.Count -gt 0)
{
    foreach ($failure in $failures)
    {
        Write-Error $failure
    }
    exit 1
}

Write-Output (
    'PASS: world-object nameplates select through a non-cancelling ' +
    'pre-vanilla input path.')
