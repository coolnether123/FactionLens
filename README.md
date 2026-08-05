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
- A quiet mode that shows a name only for the world object you point at.
- A pinned colour preview that stays visible while you scroll the settings.
- No game-save component and no diplomacy changes.

Alt-click a label, its relationship-colored text, or the legend to open and
highlight the narrowest presentation setting. Alt-click never selects the
world object, and routing uses only the already disclosed relationship class.

Labels follow vanilla world-map zoom and visibility rules. The mod adds a name
below an object's icon; it does not alter the icon, terrain, planet, or any
vanilla status color.

A name only appears where it can sit directly beneath its own icon. When space
runs out the name is dropped rather than pushed somewhere misleading, and it
reserves no screen space, so dropping one never displaces its neighbours. Enable
displaced labels to get the older behaviour back, where a crowded name moves
down and a subtle white connector joins it to its icon; connectors draw behind
nameplates so they never cross a neighbouring name.

Placement is ordered so the map stays steady. Whatever you are pointing at keeps
its place, your own colonies claim theirs next, and a name that was already
showing holds its slot before any newcomer competes for it — so panning and
zooming does not make names flicker in and out.

In quiet mode no names are drawn at all until you point at an icon, and then
only that one appears. It fades out as the pointer moves away, and holds at full
strength while the pointer rests on the name itself.

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
