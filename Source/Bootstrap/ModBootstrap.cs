using UnityEngine;
using Verse;
using FactionLens.Compatibility;
using FactionLens.Patches;
using FactionLens.Settings;

namespace FactionLens.Bootstrap
{
    public sealed class FactionLensMod : Mod
    {
        private readonly FactionLensSettings settings;

        public FactionLensMod(ModContentPack content)
            : base(content)
        {
            settings = GetSettings<FactionLensSettings>();
            CompatibilityRegistry.InitializeAll();
            PatchInstaller.InstallAll();
        }

        public override string SettingsCategory()
        {
            return "Faction Lens";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.CheckboxLabeled(
                "Feature enabled",
                ref settings.FeatureEnabled);
            listing.End();
        }
    }
}
