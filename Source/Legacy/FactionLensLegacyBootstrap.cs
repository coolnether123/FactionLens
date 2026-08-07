using Verse;

namespace FactionLens.Legacy
{
#if !RWT_RIMWORLD_ALPHA4
    [StaticConstructorOnStartup]
#endif
    public static class FactionLensLegacyBootstrap
    {
        static FactionLensLegacyBootstrap()
        {
            Log.Message("[FactionLens] Loaded legacy compatibility assembly.");
        }
    }
}
