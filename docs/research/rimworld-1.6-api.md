# RimWorld 1.6 API investigation

Inspected installed runtime:

- Game: RimWorld 1.6.4871 rev574 (runtime log)
- `Assembly-CSharp.dll` file version: `1.6.9676.17238`
- `Assembly-CSharp.dll` SHA-256:
  `4A170804FBFEFABDB620D8914E584E58F822A58C6E304DCB76A67003588DAB28`
- Spine SHA-256:
  `2441959E82AA5CAC5C96E7456213B21D1FB67881E314F85F54373A4DB8C0E2AA`
- Spine source commit:
  `650fb95835d187777fae314e1de361b8991b33ee`

The assembly was inspected through reflection and ILSpy 9.1.0.7988.

Relevant findings:

- `WorldInterface.WorldInterfaceOnGUI` calls
  `ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI` before
  selection overlays, route planning, targeters, and global controls.
- `ExpandableWorldObjectsOnGUI` is a static `void` method. It already guards
  for repaint, world-object debug visibility, zoom transition, hidden terrain,
  targeter highlighting, and per-object failures.
- `ExpandableWorldObjectsUtility.TransitionPct(WorldObject)` and
  `ExpandedIconScreenRect(WorldObject, float)` are public and provide the
  vanilla visibility transition and exact icon-space anchor.
- `WorldObject.Faction`, `AppendFactionToInspectString`, `LabelCap`,
  `Destroyed`, and `def.canHaveFaction` are public.
- `Faction.PlayerRelationKind` exposes the current `Hostile`, `Neutral`, or
  `Ally` relationship with the player and therefore needs no cache or
  relation-change patch.
- `WorldObject.SetFaction` is virtual and ownership can also come from modded
  storage, supporting an additive resolver API rather than a setter patch.
- `Site.AppendFactionToInspectString` derives disclosure from its main
  `SitePartDef` (`applyFactionColorToSiteTexture` and
  `showFactionInInspectString`).
- Vanilla `AbandonedSettlement` retains its former faction reference when
  created by `Settlement.Abandon`, so its visible object type—not a null
  faction—is the authoritative abandoned state.

Decision: one Faction-Lens-owned postfix on
`ExpandableWorldObjectsOnGUI`, with current-state resolution in the additive
overlay. No world tick, goodwill, faction, ownership, save, generation,
material, or selection patch is justified.
