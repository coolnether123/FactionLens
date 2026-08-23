# Faction Lens

Faction Lens adds relationship-colored names below world-map objects in
RimWorld 1.6. Hostile, neutral, allied, player-owned, abandoned, and unknown
locations each have a configurable color. The colors update immediately when
ownership or relationships change.

## Player features

- Live labels for settlements, sites, and compatible faction-owned objects.
- Click any displayed name to select that exact world-map object.
- Separate colors for all six ownership/relationship categories.
- An Okabe-Ito-derived colorblind preset.
- Independent switches for settlements, sites, and other world objects.
- Optional compact legend, dark nameplate background, and text outline.
- Selectable label font size that follows RimWorld's global UI scaling.
- An opacity slider so labels can sit lightly over the terrain.
- A hover-only mode that shows a name only for the world object you point at.
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

Placement order keeps the map steady. The object under the pointer keeps its
place first, followed by your colonies and labels that were already visible.
New labels take the remaining space, which reduces flicker while panning and
zooming.

In hover-only mode, no names appear until you point at an icon. That object's name
fades in, remains fully visible while the pointer rests on the name, and fades
out when the pointer moves away.

Label opacity defaults to 80% and applies to the nameplate, outline, and text
together. This preserves the contrast between them at every setting. The
slider stops at 35%; use the enable switch to hide labels completely.

The Advanced section contains rounded nameplate corners, displaced labels,
player-colony placement priority, and the reset button. Rounded corners are on
by default. The outline switch is hidden while nameplates are enabled because
the solid plate covers the outline.

## Requirements

- RimWorld 1.6
- Harmony
- SpineLib (`CoolNether123.Spine`)

## Installation

Faction Lens needs Harmony and SpineLib. Harmony is on the Steam Workshop, and
[SpineLib is available there too](https://steamcommunity.com/sharedfiles/filedetails/?id=3778463813).
For a repository build, use [coolnether123/Spine](https://github.com/coolnether123/Spine).

Copy both `FactionLens` and `Spine` into RimWorld's `Mods` directory, then
enable Harmony, SpineLib, and Faction Lens in that order. No other mod is
required, and Faction Lens depends on no other gameplay mod.

## Build and test

```powershell
dotnet run --project Tests\Mod.Tests.csproj -c Release

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command `
  "& (Join-Path `$env:RIMWORLD_TOOLING_ROOT 'tools\Invoke-RimWorldBuild.ps1') `
  -Project '$PWD\Source\Mod.csproj' -Configuration 1.6 -Version 1.6 `
  -OutputRoot '$PWD\Engineering\artifacts\build' -Engine DotNet `
  -Dependency @('harmony','spine')"
```

The normal project output is `1.6\Assemblies\FactionLens.dll`.

Compatibility authors should see
[`docs/compatibility-api.md`](docs/compatibility-api.md).
