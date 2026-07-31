# Architecture

Faction Lens is split by responsibility:

- `Domain` contains pure relationship classification and object-type policy.
- `Api` owns the public nonstandard-ownership registration contract.
- `Ownership` adapts RimWorld factions and registered resolvers into the pure
  classification model.
- `Settings` persists policy and colors and renders controls through Spine.
- `Presentation` lays out labels, backgrounds/outlines, collision avoidance,
  and the optional legend.
- `Patches` owns a stable `CoolNether123.FactionLens` Harmony instance and
  installs one narrow postfix through Spine.

## Rendering integration

RimWorld 1.6 renders expanded world-object icons in
`ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI`. Vanilla does not
provide a persistent settlement-name hook. Faction Lens therefore postfixes
that method and draws labels after the vanilla icons. It does not skip,
replace, transpile, or reproduce the vanilla method.

The overlay uses `TransitionPct`, `HiddenBehindTerrainNow`, the world-object
debug visibility flag, and screen culling. Labels appear below icons, leaving
icon material colors, selection brackets, search highlighting, and other
status signals untouched.

The same collision-adjusted rectangle is the label's click target. A
left-button `MouseDown` replaces the current world selection with that exact
object through RimWorld's `WorldSelector` and consumes the event. Drawing
still occurs only during repaint; click processing never issues draw calls.

## Immediate updates and performance

Ownership and `Faction.PlayerRelationKind` are read directly during each
repaint. No world-tick component, relation patch, ownership patch, polling
scan, or invalidation cache is needed. Work is absent while the world UI is
closed and scales with visible objects plus nearby collision candidates while
visible.

The renderer performs no LINQ or per-object collection allocation. A reused
64-pixel screen-space bucket index limits overlap checks to cells touched by a
candidate label. Accepted rectangles, visit marks, dictionary storage, and
bucket lists are retained and cleared between repaints; bucket lists are
pooled after their high-water allocation. Placement preserves the original
policy of trying the requested position followed by at most three
height-plus-gap downward shifts. Settings-only color pickers and registration
sorting use Spine and allocation outside the render hot path.

## Hidden-information rule

A vanilla faction is treated as disclosed only when the object has no faction
or `WorldObject.AppendFactionToInspectString` permits vanilla to expose it.
Otherwise the label uses the unknown category. Compatibility resolvers must
explicitly return either disclosed ownership (including disclosed
factionless ownership) or unknown. Resolver exceptions fail closed to unknown
and log once.

Vanilla `AbandonedSettlement` objects retain their former faction reference.
The adapter intentionally classifies that explicit visible object state as
abandoned/factionless instead of treating the stale reference as current
ownership.

## Persistence and removal

Only RimWorld `ModSettings` are persisted. No `WorldComponent`,
`GameComponent`, world-object comp, quest part, or save-game record is added.
Disabling or removing the mod therefore leaves saves unchanged.

## One-caller helper audit

The implementation intentionally retains these private one-caller helpers:

- `OwnershipService.Classify`, `IsVanillaOwnershipMeaningful`, and `KindOf`
  separate compatibility results, vanilla applicability, and UI type policy
  from the main adapter flow.
- `WorldLabelOverlay.DrawObject` and `DrawLegend` separate per-object and
  panel rendering from GUI-state lifetime management.
- `FactionLensSettingsUi.DrawPreview` separates the reusable label preview
  from control layout.
- `WorldLabelPatch.AfterExpandableWorldObjectsOnGui` is a Harmony callback,
  so its single caller is external by design.
- `ScreenBounds.Overlaps` and `ShiftDown`, plus
  `ScreenBounds.Contains`,
  plus
  `ScreenCollisionIndex.Intersects`, `Add`, `BeginVisit`, and `IsValid`,
  isolate the testable geometry, query, insertion, visit-generation, and
  validation phases of the bucket index.

Recommendation: keep these helpers private and cohesive. Inlining would mix
responsibilities into larger UI/adapter methods, while promoting them into a
shared abstraction would be premature until a second production consumer
exists.
