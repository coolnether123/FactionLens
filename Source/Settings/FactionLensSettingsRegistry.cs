using System.Collections.Generic;
using FactionLens.Domain;
using FactionLens.Presentation;
using Spine.UI.SettingsFramework;
using UnityEngine;
using Verse;

namespace FactionLens.Settings
{
    internal static class FactionLensSettingsRegistry
    {
        private static readonly RelationshipCategory[] PreviewCategories =
        {
            RelationshipCategory.Hostile,
            RelationshipCategory.Neutral,
            RelationshipCategory.Allied,
            RelationshipCategory.Player,
            RelationshipCategory.Factionless,
            RelationshipCategory.Unknown
        };

        internal static readonly IReadOnlyList<SettingDefinition> Definitions =
            BuildDefinitions();

        private static IReadOnlyList<SettingDefinition> BuildDefinitions()
        {
            return new[]
            {
                SettingDefinitions.Header("visuals.header", "Visuals", "FactionLens_Settings_Visuals"),
                SettingDefinitions.Toggle("visuals.enabled", nameof(FactionLensSettings.FeatureEnabled),
                    "Enable labels", "FactionLens_Settings_Enabled", tooltipKey: "FactionLens_Settings_Enabled_Tip", scribeKey: "featureEnabled"),
                SettingDefinitions.Toggle("visuals.background", nameof(FactionLensSettings.ShowBackground),
                    "Show label backgrounds", "FactionLens_Settings_Background", tooltipKey: "FactionLens_Settings_Background_Tip", scribeKey: "showBackground"),
                SettingDefinitions.Toggle("visuals.outline", nameof(FactionLensSettings.ShowOutline),
                    "Show label outlines", "FactionLens_Settings_Outline", tooltipKey: "FactionLens_Settings_Outline_Tip", scribeKey: "showOutline"),
                SettingDefinitions.Toggle("visuals.displaced", nameof(FactionLensSettings.ShowDisplacedLabels),
                    "Show displaced labels", "FactionLens_Settings_Displaced", tooltipKey: "FactionLens_Settings_Displaced_Tip", scribeKey: "showDisplacedLabels"),
                SettingDefinitions.Toggle("visuals.legend", nameof(FactionLensSettings.ShowLegend),
                    "Show legend", "FactionLens_Settings_Legend", scribeKey: "showLegend"),

                SettingDefinitions.Header("objects.header", "Object types", "FactionLens_Settings_ObjectTypes"),
                SettingDefinitions.Toggle("objects.settlements", nameof(FactionLensSettings.ShowSettlements),
                    "Show settlements", "FactionLens_Settings_Settlements", scribeKey: "showSettlements"),
                SettingDefinitions.Toggle("objects.sites", nameof(FactionLensSettings.ShowSites),
                    "Show sites", "FactionLens_Settings_Sites", scribeKey: "showSites"),
                SettingDefinitions.Toggle("objects.other", nameof(FactionLensSettings.ShowOtherFactionObjects),
                    "Show other faction objects", "FactionLens_Settings_Other", tooltipKey: "FactionLens_Settings_Other_Tip", scribeKey: "showOtherFactionObjects"),

                SettingDefinitions.Header("colors.header", "Relationship colors", "FactionLens_Settings_Colors"),
                ColorSetting("colors.hostile", nameof(FactionLensSettings.HostileColor), "hostileColor", RelationshipCategory.Hostile),
                ColorSetting("colors.neutral", nameof(FactionLensSettings.NeutralColor), "neutralColor", RelationshipCategory.Neutral),
                ColorSetting("colors.allied", nameof(FactionLensSettings.AlliedColor), "alliedColor", RelationshipCategory.Allied),
                ColorSetting("colors.player", nameof(FactionLensSettings.PlayerColor), "playerColor", RelationshipCategory.Player),
                ColorSetting("colors.factionless", nameof(FactionLensSettings.FactionlessColor), "factionlessColor", RelationshipCategory.Factionless),
                ColorSetting("colors.unknown", nameof(FactionLensSettings.UnknownColor), "unknownColor", RelationshipCategory.Unknown),
                SettingDefinitions.Button("colors.colorblind", "Colorblind preset",
                    value => ((FactionLensSettings)value).ApplyColorblindPreset(),
                    "FactionLens_Settings_Colorblind", "FactionLens_Settings_Colorblind_Tip"),
                SettingDefinitions.Button("colors.reset", "Reset all",
                    value => ((FactionLensSettings)value).ApplyDefaults(),
                    "FactionLens_Settings_Reset"),
                SettingDefinitions.Header("preview.header", "Preview", "FactionLens_Settings_Preview"),
                SettingDefinitions.Custom("preview.labels", DrawPreview)
            };
        }

        private static SettingDefinition ColorSetting(
            string id,
            string field,
            string scribeKey,
            RelationshipCategory category) =>
            SettingDefinitions.Colour(
                id,
                field,
                category.ToString(),
                "FactionLens_Category_" + category,
                "FactionLens_Settings_Color_Tip",
                scribeKey);

        private static bool DrawPreview(
            Rect rect,
            string label,
            string tooltip,
            object value,
            bool disabled)
        {
            var settings = (FactionLensSettings)value;
            float width = rect.width / PreviewCategories.Length;
            for (int index = 0; index < PreviewCategories.Length; index++)
            {
                RelationshipCategory category = PreviewCategories[index];
                Rect cell = new Rect(
                    rect.x + width * index,
                    rect.y,
                    width,
                    rect.height);
                LabelDrawer.Draw(
                    cell.ContractedBy(2f),
                    CategoryLabel(category),
                    settings.ColorFor(category),
                    settings);
            }

            return false;
        }

        internal static string CategoryLabel(RelationshipCategory category)
        {
            return ("FactionLens_Category_" + category).Translate();
        }
    }
}
