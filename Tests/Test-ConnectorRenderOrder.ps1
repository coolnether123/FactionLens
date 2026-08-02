param()

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$overlay = [System.IO.File]::ReadAllText(
    (Join-Path $root 'Source\Presentation\WorldLabelOverlay.cs'))

$connectorPass = $overlay.IndexOf('DrawConnector(placedLabel);')
$labelPass = $overlay.IndexOf('DrawPlacedLabel(')
if ($connectorPass -lt 0 -or $labelPass -lt 0 -or
    $connectorPass -ge $labelPass)
{
    Write-Error (
        'Every displaced-label connector must be drawn before any ' +
        'nameplate so a line cannot cover another name.')
    exit 1
}

if ($overlay -notmatch
    'NaturalLabelRect\.y\)\s*<=\s*0\.5f' -or
    $overlay -notmatch 'Color connectorColor\s*=\s*Color\.white')
{
    Write-Error (
        'Connectors must be white and limited to collision-displaced labels.')
    exit 1
}

Write-Output (
    'PASS: displaced connectors render behind all nameplates and normal ' +
    'labels remain unconnected.')
