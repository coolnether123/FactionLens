using Verse;
using FactionLens.Patches;
using FactionLens.Settings;
using Spine.Api;
using Spine.UI.SettingsFramework;

namespace FactionLens.Bootstrap
{
    public sealed class FactionLensMod : SpineMod<FactionLensSettings>
    {
        public FactionLensMod(ModContentPack content)
            : base(
                content,
                "CoolNether123.FactionLens",
                new SemanticVersion(1, 0, 1),
                FactionLensSettingsRegistry.Definitions,
                SpineCapability.HarmonyPatching |
                SpineCapability.BoundedCaches,
                new ModSettingsPageOptions { RowHeight = 38f })
        {
            WorldLabelPatch.Install();
        }

        protected override string SettingsCategoryLabel =>
            "Faction Lens";
    }
}
