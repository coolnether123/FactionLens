using HarmonyLib;
using RimWorld.Planet;
using Spine.Harmony;
using Verse;

namespace FactionLens.Patches
{
    internal static class WorldLabelPatch
    {
        internal const string HarmonyId =
            "CoolNether123.FactionLens";
        private static readonly HarmonyLib.Harmony HarmonyInstance =
            new HarmonyLib.Harmony(HarmonyId);
        private static bool installed;

        internal static void Install()
        {
            if (installed)
            {
                return;
            }

            var postfix = new HarmonyMethod(
                typeof(WorldLabelPatch),
                nameof(AfterExpandableWorldObjectsOnGui));
            bool success = HarmonyHelper.TryPatchMethod(
                HarmonyInstance,
                typeof(ExpandableWorldObjectsUtility),
                nameof(ExpandableWorldObjectsUtility
                    .ExpandableWorldObjectsOnGUI),
                postfix: postfix);
            if (!success)
            {
                Log.Error(
                    "[Faction Lens] Could not install the world-label " +
                    "postfix. Vanilla world-map rendering is unchanged.");
                return;
            }

            installed = true;
        }

        private static void AfterExpandableWorldObjectsOnGui()
        {
            Presentation.WorldLabelOverlay.Draw();
        }
    }
}
