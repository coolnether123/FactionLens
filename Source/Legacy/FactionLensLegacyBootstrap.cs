#if RWT_LEGACY_BOOTSTRAP
using Verse;
using HarmonyLib;

namespace FactionLens.Legacy
{
#if !RWT_RIMWORLD_ALPHA4
    [StaticConstructorOnStartup]
#endif
    public static class FactionLensLegacyBootstrap
    {
        static FactionLensLegacyBootstrap()
        {
            try
            {
                new Harmony("CoolNether123.FactionLens").PatchAll(
                    typeof(FactionLensLegacyBootstrap).Assembly);
                Log.Message(
                    "[FactionLens] Legacy world-label compatibility initialized.");
            }
            catch (System.Exception exception)
            {
                Log.Error(
                    "[FactionLens] Legacy compatibility initialization failed: " +
                    exception);
            }
        }
    }
}
#endif
