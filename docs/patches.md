# Harmony patches

## `ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI`

- Kind: postfix
- Installer: `Spine.Harmony.HarmonyHelper.TryPatchMethod`
- Harmony owner: `CoolNether123.FactionLens`
- Purpose: draw additive world-object labels immediately after vanilla draws
  expanded icons.
- Safety: the original method always runs; no arguments, return values, fields,
  icon materials, or IL are changed.
- Failure behavior: installation logs one clear error and leaves the vanilla
  world map unchanged.
- Ownership: Faction Lens creates and passes its own stable Harmony instance;
  Spine provides installation policy but does not own the patch.

No faction relation, goodwill, ownership, tick, save, or world-generation
method is patched.
