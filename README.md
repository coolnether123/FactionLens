# Faction Lens

Faction Lens adds relationship-colored names below applicable world-map objects
in RimWorld 1.6. Hostile, neutral, allied, player-owned, abandoned/factionless,
and unknown locations use independently configurable colors that are resolved
from current game state every repaint.

## Player features

- Live labels for settlements, sites, and compatible faction-owned objects.
- Click any displayed name to select that exact world-map object.
- Separate colors for all six ownership/relationship categories.
- An Okabe-Ito-derived colorblind preset.
- Independent switches for settlements, sites, and other world objects.
- Optional compact legend, dark nameplate background, and text outline.
- An immediate settings preview and one-click reset.
- No game-save component and no diplomacy changes.

Labels follow vanilla world-map zoom and visibility rules. The mod adds a name
below an object's icon; it does not alter the icon, terrain, planet, or any
vanilla status color. A subtle white connector appears when collision avoidance
has to move a label away from its icon. Connector lines are drawn behind
nameplates, so they keep the settlement relationship clear without crossing
neighboring names. Labels in their normal position remain unchanged.

Alt-click a label, its relationship-colored text, or the legend to open and
highlight the narrowest presentation setting. Alt-click never selects the world
object, and routing uses only the already disclosed relationship class.

## Requirements

- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Spine](https://github.com/coolnether123/Spine) — the shared runtime used by
  CoolNether123 mods

## Installation

Install Harmony and Spine, copy `FactionLens` into RimWorld's `Mods` folder,
then enable Harmony, Spine, and Faction Lens in that order.

Faction Lens stores only global display preferences and adds no game-save
component, so it is safe to add to or remove from an existing save.

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

## Documentation

- [Architecture](docs/architecture.md)
- [Compatibility API for other mod authors](docs/compatibility-api.md)
- [Patch inventory](docs/patches.md)
- [Verification record](docs/verification.md)

## Developer fixture

Live debug actions are isolated in `Developer/FactionLens.TestFixture`, a
separately loadable developer mod. Build and load that folder only for harness
verification; it is never part of the Faction Lens shipping package.

## License

Released under the [MIT License](LICENSE). Harmony and Spine are used under
their own licenses.
