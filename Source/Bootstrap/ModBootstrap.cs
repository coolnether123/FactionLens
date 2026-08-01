using UnityEngine;
using Verse;
using FactionLens.Patches;
using FactionLens.Settings;
using Spine.Api;
using Spine.UI.ContextualSettings;

namespace FactionLens.Bootstrap
{
    public sealed class FactionLensMod : Mod
    {
        private readonly FactionLensSettings settings;
        private readonly FactionLensSettingsUi settingsUi =
            new FactionLensSettingsUi();
        private static IContextualSettingsLease contextualSettingsLease;

        public static FactionLensSettings Settings { get; private set; }

        public FactionLensMod(ModContentPack content)
            : base(content)
        {
            SpineApi.Runtime.Require(new SpineRequirement(
                "CoolNether123.FactionLens",
                new SemanticVersion(1, 1, 0),
                SpineCapability.Settings |
                SpineCapability.HarmonyPatching |
                SpineCapability.ContextualSettings));

            settings = GetSettings<FactionLensSettings>();
            Settings = settings;
            if (contextualSettingsLease == null)
            {
                contextualSettingsLease = SpineApi.ContextualSettings.Acquire(
                    "CoolNether123.FactionLens",
                    this,
                    settingsUi.Drawer,
                    settings);
            }
            WorldLabelPatch.Install();
        }

        internal static IContextualSettingsLease ContextualSettings =>
            contextualSettingsLease;

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
