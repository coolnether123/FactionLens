# Verification record

## Automated

- Pure classification contracts:
  `dotnet run --project Tests\Mod.Tests.csproj -c Release`
  completed with
  `PASS: Faction Lens pure classification and layout contracts`.
  Layout coverage includes strict edge-touching behavior, all three allowed
  downward shifts and rejection after those shifts, cross-cell overlap, and a
  3,200-label scaling fixture whose comparison count is bounded below
  `label count * 24`.
- Central build:
  `Invoke-RimWorldBuild.ps1` for RimWorld 1.6 with resolved
  `harmony,spine` dependencies completed with exit code 0. It resolved Spine
  SHA-256
  `2441959E82AA5CAC5C96E7456213B21D1FB67881E314F85F54373A4DB8C0E2AA`
  and tooling commit
  `0a17a0484c6d1c9b048647f87167b3a0b49da738`. The tooling worktree was
  reported dirty; Faction Lens did not modify it.
- Normal Release build completed with 0 warnings and 0 errors.
- Package validation:
  `Test-RwtPackage -ModRoot <repo> -Version 1.6
  -ExpectedAssemblyName FactionLens` returned
  `RWT-BUILD-PACKAGE-VALID`.
- Packaged `FactionLens.dll` is 28,160 bytes with SHA-256
  `FD1E6D0D2C8918337D4CC22D1039F7C744BD02146D311772C38A82ABC0666E1C`.

## Isolated in-game acceptance

Primary lane:
`FactionLens-02fd38443c1c42feaf6232ecd6cea744`

- Active mods were exactly Core, Harmony, RimWorld Agent, Spine, and
  Faction Lens.
- Harness payload commit `40b60b6`; harness DLL SHA-256
  `D9DAE415A2752A80899656274E15B451AE131127B0B5A8CD4BCB093368C834B9`.
- A generated colony reached `Playing`, with one ready map and three free
  colonists at 1920x1080 and UI scale 1.
- Deterministic fixtures covered player, allied, neutral, hostile,
  factionless, unknown, settlement, site, and other world-object paths.
  The abandoned-settlement fixture retained a neutral former faction while
  Faction Lens rendered it as abandoned/factionless.
- `live-relation-neutral`, `live-relation-allied`, and
  `live-relation-hostile` show the same settlement changing color immediately
  through ordinary goodwill APIs, without reopening the map.
- `live-ownership-player` and `live-ownership-factionless` show the same
  non-settlement world object changing color immediately through
  `WorldObject.SetFaction`.
- `settings-default` and `settings-colorblind-legend-outline` show the
  ordinary RimWorld mod-settings UI, all six color controls, all object-type
  switches, the preset/reset controls, and a live legend/outline update.
- `settings-settlements-off-confirm` plus
  `switch-settlements-off-verified` prove the actual settlement UI switch
  removed additive names while leaving vanilla icons untouched. Generalized
  mod-setting readback confirmed every boolean and persisted the restored
  settings through RimWorld's ordinary settings writer.
- `save-load-roundtrip-complete` shows all fixtures and labels after saving
  and loading `FactionLens_Roundtrip`; persisted legend and outline settings
  read back as enabled.
- An 11.666-second world-map performance probe measured 1,680 frames,
  144.014 FPS, 6.944 ms/frame, no working-set/private-byte growth, and zero
  Gen0, Gen1, or Gen2 collections. The game was paused, so tick metrics were
  intentionally not applicable.
- Log review found no exception matches and no Faction Lens runtime errors.
  RimWorld emitted only its standard metadata warnings that the local Harmony
  and Spine dependency declarations have no download URL; no URL was
  fabricated.
- The lane stopped normally with exit code 0 and no forced termination.

Final ownership/regression lane:
`FactionLens-e2ebcc807f584f8caeacdd21a90ddfee`

- Loaded the seeded `FactionLens_Roundtrip` save to `Playing` with a ready
  map and all seven deterministic fixture states.
- Structured readback reconfirmed player, ally, neutral, hostile,
  factionless, unknown, and abandoned-settlement states. The abandoned
  settlement still retained a neutral former faction.
- `final-owned-regression` shows representative labels after the final
  caller-ownership rebuild. The exact capture is
  `C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6\FactionLens-e2ebcc807f584f8caeacdd21a90ddfee\ipc\captures\final-owned-regression-20260730-211521-314.png`
  (SHA-256
  `55C1F622ED3D31E8A187AB765E50D0029E39B223FDDD44094BD8C4B905B35D00`).
- Harmony inspection found exactly one relevant patch:
  `ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI`, postfix
  `CoolNether123.FactionLens::FactionLens.Patches.WorldLabelPatch.AfterExpandableWorldObjectsOnGui`.
  Spine's own success log independently reported owner
  `CoolNether123.FactionLens`.
- Generalized settings readback reconfirmed the master, settlement, site, and
  other-object switches enabled in the fresh profile.
- A final 6.874-second paused world-map probe measured 990 frames,
  144.012 FPS, 6.944 ms/frame, no working-set/private-byte growth, and zero
  Gen0, Gen1, or Gen2 collections.
- Log review found no exception matches.
- The lane stopped normally with exit code 0 and no forced termination.

Dense collision-index lane:
`FactionLens-4f5d92ffd9b541b3b8bc481bd663ee4c`

- Active mods were exactly Core, Harmony, RimWorld Agent, Spine, and
  Faction Lens.
- The lane created exactly 320 additional vanilla player settlements
  (`Dense000` through `Dense319`); the final fixture was world-object ID 418.
  A fully zoomed-out world-map capture,
  `dense-world-320-20260730-214832-142.png`, shows the dense set of rendered
  labels with collision displacement active.
- With Faction Lens enabled, an 11.779-second probe measured 1,665 frames,
  141.353 FPS, 7.074 ms/frame, 707 ticks, and 60.022 TPS.
- In the identical scene with only Faction Lens's feature switch disabled,
  an 11.352-second baseline measured 1,635 frames, 144.033 FPS,
  6.943 ms/frame, 681 ticks, and 59.992 TPS. The feature switch was restored
  afterward.
- Both probes recorded no working-set or private-byte growth and zero Gen0,
  Gen1, or Gen2 collections. Managed-memory deltas were 9,007,104 bytes
  enabled and 8,073,216 bytes disabled.
- Harmony inspection reconfirmed the sole relevant postfix owner as
  `CoolNether123.FactionLens`. Log review found no exception or
  collision-index error matches and no Faction Lens runtime errors.
- The lane stopped normally with exit code 0 and no forced termination.

Earlier category-focused captures retained in
`FactionLens-0f316d413dd64293be7ba07952b5e31d` include
`state-player`, `state-allied`, `state-neutral`, `state-hostile`,
`state-abandoned`, `state-unknown`, and `state-other`. They provide close
visual evidence for every displayed category, including unknown ownership
that vanilla deliberately withholds.

## Removal check

Removal lane:
`Spine-9d99af6907e641ff95d1c971f2323316`

- Active mods were exactly Core, Harmony, RimWorld Agent, and Spine; Faction
  Lens was absent.
- The seeded `FactionLens_Roundtrip` save loaded to `Playing` with the map,
  colonists, and all world fixtures intact.
- `removal-save-load` shows the same world map and untouched vanilla icons
  without Faction Lens labels.
- Log review found no exception matches.
- The lane stopped normally with exit code 0 and no forced termination.
