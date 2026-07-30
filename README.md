# Faction Lens

Faction Lens adds relationship-colored names below applicable world-map
objects in RimWorld 1.6. Hostile, neutral, allied, player-owned,
abandoned/factionless, and unknown locations use independently configurable
colors that are resolved from current game state every repaint.

## Player features

- Live labels for settlements, sites, and compatible faction-owned objects.
- Separate colors for all six ownership/relationship categories.
- An Okabe-Ito-derived colorblind preset.
- Independent switches for settlements, sites, and other world objects.
- Optional compact legend, dark nameplate background, and text outline.
- An immediate settings preview and one-click reset.
- No game-save component and no diplomacy changes.

Labels follow vanilla world-map zoom and visibility rules. The mod adds a
name below an object's icon; it does not alter the icon, terrain, planet, or
any vanilla status color.

## Requirements

- RimWorld 1.6
- Harmony
- Spine (`CoolNether123.Spine`)

## Build and test

```powershell
dotnet run --project Tests\Mod.Tests.csproj -c Release

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command `
  "& 'A:\Dev\RimWorld\Worktrees\RimWorld-Tooling\phase-a\tools\Invoke-RimWorldBuild.ps1' `
  -Project '$PWD\Source\Mod.csproj' -Configuration Release -Version 1.6 `
  -OutputRoot '$PWD\Engineering\artifacts\build' -Engine DotNet `
  -Dependency @('harmony','spine')"
```

The normal project output is `1.6\Assemblies\FactionLens.dll`.

Compatibility authors should see
[`docs/compatibility-api.md`](docs/compatibility-api.md).
