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
status signals untouched. A label placed at its natural position keeps the
normal unconnected appearance. When collision avoidance shifts it downward,
the overlay draws a thin white connector from the icon to the displaced label.
Rendering is deliberately two-pass: every connector is drawn first, followed
by every nameplate and its text. A connector therefore remains behind all
labels regardless of world-object enumeration order.

The same collision-adjusted rectangle is the label's click target. A
left-button `MouseDown` replaces the current world selection with that exact
object through RimWorld's `WorldSelector` and consumes the event. A
non-cancelling prefix captures the target before vanilla can consume
`MouseDown`; the postfix applies the captured selection after vanilla has
finished its own input handling, so vanilla cannot overwrite the label click.
Drawing still occurs only during postfix repaint and click processing never
issues draw calls.

Label font size is a Faction Lens presentation preference backed by RimWorld's
`GameFont` values. The selected font is used consistently for measurement,
nameplate drawing, the settings preview, and the legend. RimWorld's global UI
scale remains the only coordinate transform: Faction Lens works in the same
logical UI space as vanilla, so it does not multiply label geometry by
`Prefs.UIScale` a second time. Larger fonts naturally produce larger collision
and click rectangles and can therefore reduce the number of labels that fit in
a crowded view.
The pre-0.17 compatibility renderer remains Tiny because those builds do not
have the Spine-backed settings page; the modern renderer is the owner of this
preference.

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

Measured label sizes use Spine's bounded LRU cache instead of an unbounded or
magic-threshold dictionary clear. The cache has a 64 KiB budget and therefore
retains useful repeated names without periodic full-cache churn. It is reset
when the effective `GameFont` changes so a saved Tiny measurement cannot be
reused for Small or Medium text.

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
- `WorldLabelOverlay.TryPlaceObject`, `BindAndHandleInput`, `DrawConnector`,
  `DrawPlacedLabel`, `ApplyPendingSelection`, and `DrawLegend` separate layout,
  input, the connector-first pass, the label-second pass, post-vanilla
  selection, and panel rendering from GUI-state lifetime management.
- `FactionLensSettingsUi.DrawPreview` separates the reusable label preview
  from control layout.
- `FactionLensSettingsRegistry.FontSizeLabel` keeps enum-value translation
  local to the setting registration, and `WorldLabelOverlay.LegendPanelWidth`
  keeps legend text measurement separate from panel placement.
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
