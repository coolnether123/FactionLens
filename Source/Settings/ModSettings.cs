using Verse;

namespace FactionLens.Settings
{
    public sealed class FactionLensSettings : ModSettings
    {
        public bool FeatureEnabled;

        public override void ExposeData()
        {
            Scribe_Values.Look(
                ref FeatureEnabled,
                "featureEnabled",
                false);
            base.ExposeData();
        }
    }
}
