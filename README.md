# Faction Lens

Faction Lens adds relationship-colored names below applicable world-map
objects in RimWorld 1.6. Hostile, neutral, allied, player-owned,
abandoned/factionless, and unknown locations use independently configurable
colors that are resolved from current game state every repaint.

## Player features

- Live labels for settlements, sites, and compatible faction-owned objects.
- Click any displayed name to select that exact world-map object.
- Separate colors for all six ownership/relationship categories.
- An Okabe-Ito-derived colorblind preset.
- Independent switches for settlements, sites, and other world objects.
- Optional compact legend, dark nameplate background, and text outline.
- An immediate settings preview and one-click reset.
- No game-save component and no diplomacy changes.

Alt-click a label, its relationship-colored text, or the legend to open and
highlight the narrowest presentation setting. Alt-click never selects the
world object, and routing uses only the already disclosed relationship class.

Labels follow vanilla world-map zoom and visibility rules. The mod adds a name
below an object's icon; it does not alter the icon, terrain, planet, or any
vanilla status color. A subtle white connector appears when collision
avoidance has to move a label away from its icon; labels in their normal
position remain unchanged.

## Requirements

- RimWorld 1.6
- Harmony
- Spine (`CoolNether123.Spine`)

## Installation

Spine does not yet have a public Workshop or download URL, so this verified
build is distributed through the locally produced release collection. Copy both
`FactionLens` and `Spine` into RimWorld's `Mods` directory, then enable
Harmony, Spine, and Faction Lens in that order. No other gameplay mod in the
collection is required.

## Build and test

```powershell
dotnet run --project Tests\Mod.Tests.csproj -c Release

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command `
  "& (Join-Path `$env:RIMWORLD_TOOLING_ROOT 'tools\Invoke-RimWorldBuild.ps1') `
  -Project '$PWD\Source\Mod.csproj' -Configuration Release -Version 1.6 `
  -OutputRoot '$PWD\Engineering\artifacts\build' -Engine DotNet `
  -Dependency @('harmony','spine')"
```

The normal project output is `1.6\Assemblies\FactionLens.dll`.

Compatibility authors should see
[`docs/compatibility-api.md`](docs/compatibility-api.md).
