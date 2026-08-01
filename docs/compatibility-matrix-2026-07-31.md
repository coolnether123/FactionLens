# Faction Lens compatibility matrix — 2026-07-31

All executed rows used RW `1.6.4871 rev573`, Core only, the canonical H-drive
runtime, Harmony, RimWorld Agent, and Spine where Faction Lens was active. Full
evidence and exact identities are in
`docs/compatibility-investigation-2026-07-31.md`. Runtime lane paths below are
children of the exact root
`C:\Users\PrecisionX\AppData\Local\Temp\RimWorldAgentTasks\1.6`.

| Combination | Orders tested | Classification | Evidence / remaining boundary |
|---|---:|---|---|
| Faction Lens alone | required order | **compatible** | `docs/verification.md`; settings, hidden owner, save/load/removal, click, dense perf |
| Rim War alone | external baseline | baseline confirmed | lane `2222935097-a21d1f7327434dacbd4730e501fbf3eb` |
| Faction Lens + Rim War | both | **compatible** | lanes `FactionLens-fc49f39f242748b2aa19411c74e4df94` and `2222935097-1c5cfb8c363a4b70be4e9e90d3422756` |
| Faction Lens + VEF + Vanilla Outposts Expanded | both dependency-valid orders | **inconclusive** | startup/generic surface clean; no real VOE `Outpost` instance; lanes `FactionLens-7af9b45e5dc04fdab436d7b16e99af48` and `2023507013-dff7bb86835740de918a649879a59041` |
| Map Mode Framework original | none | **inconclusive** | input not locally available; must be separate from fork |
| Map Mode Framework Forked | none | **inconclusive** | input not locally available; must be separate from original |
| Faction Territories and Vassalage | none | **inconclusive** | no runtime input/pass |
| Empire | none | **inconclusive** | requires Royalty-relevant grouped pass |
| Roads of the Rim | none | **inconclusive** | no runtime input/pass |
| Dynamic Diplomacy | none | **inconclusive** | no runtime input/pass |
| More Faction Interaction | none | **inconclusive** | no runtime input/pass |
| Faction Control (1.4–1.6), `thereallemon.factioncontrol` | none | **inconclusive** | locally present Workshop item `2882785581`, but not reached before bounded stop |
| RimCities, `Cabbage.RimCities` | none | **inconclusive** | locally present Workshop item `1775170117`, but not reached before bounded stop |
| RimWorld Exploration Mode / maintained hidden-information equivalent | none | **inconclusive** | no runtime input/pass; hidden-information gate is mandatory |
| Realistic Planets 2 | none | **inconclusive** | no runtime input/pass; must match its MMF implementation |
| Set Up Camp / maintained camp replacement | none | **inconclusive** | no runtime input/pass |
| quest/event temporary sites | vanilla fixture only | **compatible with documented limitation** | vanilla undisclosed-site semantics pass; no named external generator tested |
| faction/outpost/adventure world-object generators | VOE startup only | **inconclusive** | actual custom objects and lifecycle not exercised |
| dense world / overlap / zoom / UI scale / Multiplayer stack | target-alone density only | **inconclusive** | pairwise Rim War viewport passed; grouped stress and MP remain |

No expected hard conflict, confirmed hard conflict, target defect, external defect,
or patch-required combination was found. The absence of an executed row is not a
compatibility claim.
