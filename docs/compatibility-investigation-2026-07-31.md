# Faction Lens compatibility investigation — 2026-07-31

## Scope and decision rule

This is an evidence-only Core pairwise pass. It used the canonical DRM-free
runtime at `<rimworld-install>` (`1.6.4871 rev573`) and did not
start an individual full-DLC Steam lane. Steam Workshop folders were read-only
mod inputs. The grouped DLC assertions at the end are the remaining DLC lane;
unrelated DLC combinations are deliberately out of scope.

The classifications are:

- **compatible**: the shared feature surface was exercised and remained correct;
- **compatible with a documented limitation**: useful surface passed but a stated
  boundary remains;
- **inconclusive**: startup or generic behavior passed, but the mod-specific shared
  surface was not created;
- the other requested classes were not observed in this pass.

No compatibility code and no external-mod files were changed.

## Exact inputs

| Mod | Package ID | Source/version identity | Download or touch date | Content date | Tested game/DLC |
|---|---|---|---|---|---|
| Faction Lens | `CoolNether123.FactionLens` | local repository commit `4e889c44498360f8fc091512e2e532724c7c7731`; shipping DLL SHA-256 `6E6523E1B7E36658BA593A2E7142537B8C1E6FA56A759F5B4C515C37A4FC8541` | local source, not Workshop | commit 2026-07-30 22:11:17 -05:00 | RW 1.6.4871, Core only |
| Spine | `CoolNether123.Spine` | local dependency resolved by the harness | local source, not Workshop | current local checkout | RW 1.6.4871, Core only |
| Rim War | `Torann.RimWar` | Workshop item `2222935097`, manifest `6401883583617753695`; no `modVersion` in `About.xml`; 1.6 DLL SHA-256 `393D7CCCACA992AE28E427C4BC8068460A3CBF31F4E2B61D58A004D1B0E5A4CA` | Steam `timetouched` 2026-07-31 19:50:53 -05:00 (folder creation 2021-02-07) | Steam `timeupdated` 2026-03-29 07:40:59 -05:00 | RW 1.6.4871, Core only |
| Vanilla Expanded Framework | `OskarPotocki.VanillaFactionsExpanded.Core` | Workshop item `2023507013`, manifest `5608276909967839962`; no `modVersion` in `About.xml` | Steam `timetouched` 2026-07-31 19:50:53 -05:00 | Steam `timeupdated` 2026-07-15 07:50:03 -05:00 | RW 1.6.4871, Core only |
| Vanilla Outposts Expanded | `vanillaexpanded.outposts` | Workshop item `2688941031`, manifest `8980092163706155411`; no `modVersion` in `About.xml`; 1.6 DLL SHA-256 `5898527AB8F820F0FDE5145CFAF197D7FC473714A610AF1EB0FAD17AD46D74F6` | Steam `timetouched` 2026-07-31 19:50:53 -05:00 (folder creation 2026-05-21) | Steam `timeupdated` 2026-05-25 04:35:19 -05:00 | RW 1.6.4871, Core only |

The Steam manifest evidence is
`<steam-library>\steamapps\workshop\appworkshop_294100.acf`. Dates above are
the manifest's Unix timestamps converted to America/Chicago. This is more exact
than treating copied folder timestamps as a release version.

## Results

### Faction Lens alone — compatible

Load order was Core, Harmony, RimWorld Agent, Spine, Faction Lens. Existing
isolated evidence in `docs/verification.md` remains the authoritative target-alone
run. It covers all six relationship categories, a genuinely undisclosed site,
live goodwill and ownership transitions while the world map remained open,
click selection, settings routing and persistence, save/reload, safe removal from
a copied save, 320-settlement collision placement, and closed/open-map performance.

The dense enabled scene measured 141.353 FPS / 7.074 ms per frame versus
144.033 FPS / 6.943 ms with the feature disabled. No persistent work outside the
world-map draw path was found. Removal preserved the save's maps and vanilla
world objects.

Primary evidence:

- `<repo-root>\docs\verification.md`
- `<harness-evidence-root>\FactionLens-4f5d92ffd9b541b3b8bc481bd663ee4c`
- `<harness-evidence-root>\Spine-9d99af6907e641ff95d1c971f2323316`

### Rim War alone — external baseline confirmed

Load order was Core, Harmony, RimWorld Agent, Rim War. A colony reached Playing,
a hostile settlement was created and selected, and the world map showed Rim War's
own map tab and markers without additive Faction Lens labels. Rim War did not
patch `ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI`. No exception
matches were present.

Evidence:

- `<harness-evidence-root>\2222935097-a21d1f7327434dacbd4730e501fbf3eb\lane.json`
- `<harness-evidence-root>\2222935097-a21d1f7327434dacbd4730e501fbf3eb\ipc\captures\rimwar-alone-baseline-20260801-012236-342.png`

### Faction Lens + Rim War, target first — compatible

Exact load order was Core, Harmony, RimWorld Agent, Spine, Faction Lens, Rim War.
The run created neutral and hostile settlements, a factionless object, and a real
site whose internal faction was not disclosed by vanilla. The captured world map
showed Faction Lens labels alongside Rim War's world markers and tab without a
duplicate label or obscured control in the inspected viewport.

Clicking the collision-adjusted `RimWarPair_Hostile` name selected world object
102, after which an ordinary goodwill transition changed its structured category
from hostile to ally while the map remained open. Saving and loading
`FactionLens_RimWar_Roundtrip` retained all objects. After reload the unknown
site still reported `ownerDisclosedByVanilla=False`; no concealed faction was
shown by Faction Lens. The settlement returned to hostile because the save was
taken before the later live ally transition, which is expected rather than lost
state.

Harmony ownership on the shared method was exactly Faction Lens's non-cancelling
prefix and postfix. Rim War owned 30 other world-related patches, but none on that
method. No exception or error matches attributable to either mod were present.

The paused, open-map pair measured 143.849 FPS / 6.952 ms per frame over 19.395
seconds. Compared with the target-alone 144 FPS capped probes, this gives no
regression signal, but it is not a dense Rim War stress benchmark.

Evidence:

- `<harness-evidence-root>\FactionLens-fc49f39f242748b2aa19411c74e4df94\lane.json`
- `<harness-evidence-root>\FactionLens-fc49f39f242748b2aa19411c74e4df94\ipc\captures\rimwar-pair-labels-valid-20260801-011918-104.png`
- `<harness-evidence-root>\FactionLens-fc49f39f242748b2aa19411c74e4df94\ipc\captures\rimwar-pair-live-relation-selection-20260801-011944-800.png`
- `<harness-evidence-root>\FactionLens-fc49f39f242748b2aa19411c74e4df94\ipc\captures\rimwar-pair-roundtrip-valid-20260801-012339-799.png`

### Rim War + Faction Lens, external first — compatible

Exact load order was Core, Harmony, RimWorld Agent, Rim War, Spine, Faction Lens.
A colony reached Playing. A hostile settlement and an undisclosed site were
created; the selected settlement rendered correctly with the world map centered
on it. The same exact Faction Lens-only prefix/postfix ownership was present on
`ExpandableWorldObjectsOnGUI`, and the log contained no exception match. There
was no load-order-dependent presentation or ownership difference in the tested
surface.

Evidence:

- `<harness-evidence-root>\2222935097-1c5cfb8c363a4b70be4e9e90d3422756\lane.json`
- `<harness-evidence-root>\2222935097-1c5cfb8c363a4b70be4e9e90d3422756\ipc\captures\rimwar-first-pair-20260801-012218-003.png`

### Faction Lens + VEF + Vanilla Outposts Expanded, both orders — inconclusive

The two exact orders were:

1. Core, Harmony, RimWorld Agent, Spine, Faction Lens, VEF, Outposts.
2. Core, Harmony, RimWorld Agent, VEF, Outposts, Spine, Faction Lens.

Both reached Playing and rendered a player-owned generic world object plus a
genuinely undisclosed site. Both retained Faction Lens as the only owner on
`ExpandableWorldObjectsOnGUI`; both exception and error searches were clean.
The first lane took longer to finish map initialization while both lanes ran,
but completed normally and produced no error. That is a false alarm, not evidence
of a load-order defect.

This is not enough to call Vanilla Outposts Expanded compatible. The generic
fixture creates a vanilla `PeaceTalks`, not VOE's real `Outpost` subclass, and the
current harness cannot instantiate an arbitrary `WorldObjectDef`. Therefore
custom outpost ownership changes, name, selection, tooltip, production-state
overlay, save/reload, and removal were not exercised. Classification remains
**inconclusive**, with the narrower finding that startup, generic labels, hidden
ownership, and shared Harmony ownership are compatible in both orders.

Evidence:

- `<harness-evidence-root>\FactionLens-7af9b45e5dc04fdab436d7b16e99af48\lane.json`
- `<harness-evidence-root>\FactionLens-7af9b45e5dc04fdab436d7b16e99af48\ipc\captures\factionlens-first-voe-pair-20260801-012704-795.png`
- `<harness-evidence-root>\2023507013-dff7bb86835740de918a649879a59041\lane.json`
- `<harness-evidence-root>\2023507013-dff7bb86835740de918a649879a59041\ipc\captures\voe-first-pair-20260801-012632-984.png`

## Findings and smallest defensible response

1. **No release-blocking defect was reproduced in an executed combination.**
2. **Coverage blocker:** the original and forked Map Mode Framework inputs were
   absent locally. Do not promise either, and do not infer the fork from the
   original. Their mutual-exclusion boundary and Faction Lens overlay/button/
   legend interaction need a real runtime pass.
3. **Coverage blocker:** VOE's real custom `Outpost` instance was not created.
   The smallest response is a deterministic fixture or an ordinary gameplay save,
   not compatibility code.
4. **No compatibility layer is justified by current evidence.** If a future map
   mode needs presentation coordination, prefer an optional compatibility assembly
   or a narrow Faction Lens provider. Do not put faction/world-object semantics in
   Spine. Spine is only appropriate for genuinely generic settings routing/color UI.
5. **False alarm:** searching the in-game log for `Exception` can return RimWorld
   Agent's own earlier command transcript containing that word. Player logs had no
   matching line; this is not a game exception.
6. **False alarm:** the hidden site's `sitePartsKnown` became true after reload in
   one Rim War save, while `ownerDisclosedByVanilla` remained false and no faction
   was exposed. The disclosure predicate, not that internal flag alone, is the
   security boundary.

No target-mod defect, external-mod defect, patch-required result, unsupported hard
conflict, or integration opportunity was confirmed.

## Grouped Steam/DLC assertions still required

Run these together where practical rather than one DLC lane per pair. Record the
same package/version/order metadata in that grouped result.

- **Royalty group:** Rim War and Empire with Royalty active. Create ordinary,
  royal-quest, allied, neutral, hostile, defeated, removed, captured, and
  vassalized settlements/sites; change goodwill and ownership while the world map
  remains open. Assert public labels update and undisclosed ownership never affects
  text color, tooltip, legend, selection, logs, or cache.
- **Ideology/Biotech world-site group, only where a loaded mod/DLC creates relevant
  world objects:** exercise relic/complex, mechanitor, pollution, abandoned, and
  factionless sites. Skip DLC with no world-object ownership or display surface and
  record the reason instead of manufacturing a pass.
- **Odyssey group:** keep all DLC enabled in one representative stack. Open every
  planet/space layer before, during, and after a gravship transition; move between
  two colony maps and any temporary map. Assert Faction Lens draws only in the
  correct world-map context, does not leak surface ownership into hidden layers,
  preserves exact selection, and performs no closed-interface scanning.
- **Map Mode Framework group:** test original and fork in separate runs; never load
  them together. Include Realistic Planets 2 only in the run matching its embedded
  implementation. Check buttons, legends, territory overlays, search highlights,
  click rectangles, dense-label collision, zoom/pan culling, UI scale, and live
  diplomacy changes.
- **VOE group:** seed a copied save containing at least one real outpost. Test both
  Faction Lens positions around VEF/VOE, create/rename/change/remove the outpost,
  save/reload, then remove Faction Lens from another copied save. Confirm VOE's own
  tooltip, gizmos, production behavior, and selection remain unchanged.
- **Multiplayer, if available:** two clients click overlapping labels and change
  goodwill/ownership through synchronized gameplay. Presentation must remain local;
  Faction Lens must not introduce a new synchronized command or desync.

UI scale/resolution sweeps, extreme settlement density with Rim War/territory
overlays, 30-minute accelerated simulation, and language fallback remain grouped
stack work rather than evidence from this bounded Core pass.
