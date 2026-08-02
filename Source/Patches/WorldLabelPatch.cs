using HarmonyLib;
using RimWorld.Planet;
using Spine.Api;

namespace FactionLens.Patches
{
    internal static class WorldLabelPatch
    {
        internal const string HarmonyId =
            "CoolNether123.FactionLens";
        private static readonly Spine.Harmony.IHarmonyPatchInstaller Installer =
            SpineApi.Patching.CreateInstaller(HarmonyId, "[Faction Lens]");

        internal static void Install()
        {
            var prefix = new HarmonyMethod(
                typeof(WorldLabelPatch),
                nameof(BeforeExpandableWorldObjectsOnGui));
            var postfix = new HarmonyMethod(
                typeof(WorldLabelPatch),
                nameof(AfterExpandableWorldObjectsOnGui));
            Installer.TryPatch(
                "world-label overlay",
                AccessTools.Method(
                    typeof(ExpandableWorldObjectsUtility),
                    nameof(ExpandableWorldObjectsUtility
                        .ExpandableWorldObjectsOnGUI)),
                prefix: prefix,
                postfix: postfix);
        }

        private static void BeforeExpandableWorldObjectsOnGui()
        {
            Presentation.WorldLabelOverlay.HandleInput();
        }

        private static void AfterExpandableWorldObjectsOnGui()
        {
            Presentation.WorldLabelOverlay.Draw();
        }
    }
}
