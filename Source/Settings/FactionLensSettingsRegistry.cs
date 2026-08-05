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
                // The nameplate already supplies contrast, so an outline behind
                // it changes nothing a player can see. Hide the control while the
                // background is on rather than offer a setting that does nothing.
                SettingDefinitions.Toggle("visuals.outline", nameof(FactionLensSettings.ShowOutline),
                    "Show label outlines", "FactionLens_Settings_Outline", tooltipKey: "FactionLens_Settings_Outline_Tip", scribeKey: "showOutline",
                    visibleWhen: settings => !((FactionLensSettings)settings).ShowBackground),
                SettingDefinitions.Toggle("visuals.displaced", nameof(FactionLensSettings.ShowDisplacedLabels),
                    "Keep labels that cannot fit under their icon", "FactionLens_Settings_Displaced", tooltipKey: "FactionLens_Settings_Displaced_Tip", simple: false, scribeKey: "showDisplacedLabels"),
                SettingDefinitions.Toggle("visuals.hoveronly", nameof(FactionLensSettings.LabelsOnHoverOnly),
                    "Only show a name when pointed at", "FactionLens_Settings_HoverOnly", tooltipKey: "FactionLens_Settings_HoverOnly_Tip", scribeKey: "labelsOnHoverOnly"),
                SettingDefinitions.Toggle("visuals.playerpriority", nameof(FactionLensSettings.PrioritizePlayerLabels),
                    "Give player colony names first claim", "FactionLens_Settings_PlayerPriority", tooltipKey: "FactionLens_Settings_PlayerPriority_Tip", simple: false, scribeKey: "prioritizePlayerLabels"),
                SettingDefinitions.Toggle("visuals.legend", nameof(FactionLensSettings.ShowLegend),
                    "Show legend", "FactionLens_Settings_Legend", tooltipKey: "FactionLens_Settings_Legend_Tip", scribeKey: "showLegend"),

                SettingDefinitions.Header("objects.header", "Object types", "FactionLens_Settings_ObjectTypes"),
                SettingDefinitions.Toggle("objects.settlements", nameof(FactionLensSettings.ShowSettlements),
                    "Show settlements", "FactionLens_Settings_Settlements", tooltipKey: "FactionLens_Settings_Settlements_Tip", scribeKey: "showSettlements"),
                SettingDefinitions.Toggle("objects.sites", nameof(FactionLensSettings.ShowSites),
                    "Show sites", "FactionLens_Settings_Sites", tooltipKey: "FactionLens_Settings_Sites_Tip", scribeKey: "showSites"),
                SettingDefinitions.Toggle("objects.other", nameof(FactionLensSettings.ShowOtherFactionObjects),
                    "Show other faction objects", "FactionLens_Settings_Other", tooltipKey: "FactionLens_Settings_Other_Tip", simple: false, scribeKey: "showOtherFactionObjects"),

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
                    "FactionLens_Settings_Reset", tooltipKey: "FactionLens_Settings_Reset_Tip", simple: false),
                // Pinned so the swatches stay on screen while the colour rows
                // above them scroll; changing a colour is pointless if you have
                // to scroll away from the preview to reach the control.
                SettingDefinitions.Custom(
                    "preview.labels",
                    DrawPreview,
                    pin: SettingPin.Bottom)
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
