# Harmony patches

## `ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI`

- Kind: non-cancelling prefix for label-click capture plus postfix for
  post-vanilla selection and repaint
- Installer: `Spine.Harmony.HarmonyHelper.TryPatchMethod`
- Harmony owner: `CoolNether123.FactionLens`
- Purpose: draw additive world-object labels immediately after vanilla draws
  expanded icons and make each displayed name an exact selection target.
- Safety: the prefix returns `void`, so the original method always runs. It
  consumes only a left click inside a displayed label and stores that exact
  object in transient memory. The postfix applies the selection after vanilla
  input handling. No arguments, return values, icon materials, or IL are
  changed.
- Failure behavior: installation logs one clear error and leaves the vanilla
  world map unchanged.
- Ownership: Faction Lens creates and passes its own stable Harmony instance;
  Spine provides installation policy but does not own the patch.

No faction relation, goodwill, ownership, tick, save, or world-generation
method is patched.
