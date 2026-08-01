using UnityEngine;
using Verse;
using FactionLens.Patches;
using FactionLens.Settings;
using Spine.Api;

namespace FactionLens.Bootstrap
{
    public sealed class FactionLensMod : Mod
    {
        private readonly FactionLensSettings settings;
        private readonly FactionLensSettingsUi settingsUi =
            new FactionLensSettingsUi();

        public static FactionLensSettings Settings { get; private set; }

        public FactionLensMod(ModContentPack content)
            : base(content)
        {
            SpineApi.Runtime.Require(new SpineRequirement(
                "CoolNether123.FactionLens",
                new SemanticVersion(1, 0, 0),
                SpineCapability.Settings |
                SpineCapability.HarmonyPatching));

            settings = GetSettings<FactionLensSettings>();
            Settings = settings;
            WorldLabelPatch.Install();
        }

        public override string SettingsCategory()
        {
            return "Faction Lens";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settingsUi.Draw(inRect, settings);
        }
    }
}
