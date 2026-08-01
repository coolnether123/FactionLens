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
        internal static readonly IReadOnlyList<SettingDefinition> Definitions =
            BuildDefinitions();

        internal static readonly SettingsHierarchy Hierarchy =
            new SettingsHierarchy(Definitions);

        private static IReadOnlyList<SettingDefinition> BuildDefinitions()
        {
            FactionLensPalette defaults = FactionLensPalette.Default;
            return new[]
            {
                Header("visuals.header", "Visuals", "FactionLens_Settings_Visuals", 0),
                Toggle("visuals.enabled", nameof(FactionLensSettings.FeatureEnabled),
                    "Enable labels", "FactionLens_Settings_Enabled", "FactionLens_Settings_Enabled_Tip", true, 10),
                Toggle("visuals.background", nameof(FactionLensSettings.ShowBackground),
                    "Show label backgrounds", "FactionLens_Settings_Background", "FactionLens_Settings_Background_Tip", true, 20),
                Toggle("visuals.outline", nameof(FactionLensSettings.ShowOutline),
                    "Show label outlines", "FactionLens_Settings_Outline", "FactionLens_Settings_Outline_Tip", false, 30),
                Toggle("visuals.legend", nameof(FactionLensSettings.ShowLegend),
                    "Show legend", "FactionLens_Settings_Legend", null, false, 40),

                Header("objects.header", "Object types", "FactionLens_Settings_ObjectTypes", 100),
                Toggle("objects.settlements", nameof(FactionLensSettings.ShowSettlements),
                    "Show settlements", "FactionLens_Settings_Settlements", null, true, 110),
                Toggle("objects.sites", nameof(FactionLensSettings.ShowSites),
                    "Show sites", "FactionLens_Settings_Sites", null, true, 120),
                Toggle("objects.other", nameof(FactionLensSettings.ShowOtherFactionObjects),
                    "Show other faction objects", "FactionLens_Settings_Other", "FactionLens_Settings_Other_Tip", true, 130),

                Header("colors.header", "Relationship colors", "FactionLens_Settings_Colors", 200),
                ColorSetting("colors.hostile", nameof(FactionLensSettings.HostileColor),
                    RelationshipCategory.Hostile, defaults.Hostile, 210),
                ColorSetting("colors.neutral", nameof(FactionLensSettings.NeutralColor),
                    RelationshipCategory.Neutral, defaults.Neutral, 220),
                ColorSetting("colors.allied", nameof(FactionLensSettings.AlliedColor),
                    RelationshipCategory.Allied, defaults.Allied, 230),
                ColorSetting("colors.player", nameof(FactionLensSettings.PlayerColor),
                    RelationshipCategory.Player, defaults.Player, 240),
                ColorSetting("colors.factionless", nameof(FactionLensSettings.FactionlessColor),
                    RelationshipCategory.Factionless, defaults.Factionless, 250),
                ColorSetting("colors.unknown", nameof(FactionLensSettings.UnknownColor),
                    RelationshipCategory.Unknown, defaults.Unknown, 260),
                new SettingDefinition
                {
                    Id = "colors.colorblind",
                    Type = SettingType.Button,
                    Label = "Colorblind preset",
                    LabelKey = "FactionLens_Settings_Colorblind",
                    TooltipKey = "FactionLens_Settings_Colorblind_Tip",
                    SortOrder = 270,
                    ShowInSimpleView = true,
                    OnChanged = value => ((FactionLensSettings)value).ApplyColorblindPreset()
                },
                new SettingDefinition
                {
                    Id = "colors.reset",
                    Type = SettingType.Button,
                    Label = "Reset all",
                    LabelKey = "FactionLens_Settings_Reset",
                    SortOrder = 280,
                    ShowInSimpleView = true,
                    OnChanged = value => ((FactionLensSettings)value).ApplyDefaults()
                },
                Header("preview.header", "Preview", "FactionLens_Settings_Preview", 300),
                new SettingDefinition
                {
                    Id = "preview.labels",
                    Type = SettingType.Custom,
                    Label = string.Empty,
                    LabelKey = string.Empty,
                    SortOrder = 310,
                    ShowInSimpleView = true,
                    CustomDrawer = DrawPreview
                }
            };
        }

        private static SettingDefinition Header(
            string id,
            string label,
            string labelKey,
            int order) =>
            new SettingDefinition
            {
                Id = id,
                Type = SettingType.Header,
                Label = label,
                LabelKey = labelKey,
                SortOrder = order,
                ShowInSimpleView = true
            };

        private static SettingDefinition Toggle(
            string id,
            string field,
            string label,
            string labelKey,
            string tooltipKey,
            bool defaultValue,
            int order) =>
            new SettingDefinition
            {
                Id = id,
                FieldName = field,
                ScribeKey = id,
                Type = SettingType.Bool,
                Label = label,
                LabelKey = labelKey,
                TooltipKey = tooltipKey,
                DefaultValue = defaultValue,
                SortOrder = order,
                ShowInSimpleView = true
            };

        private static SettingDefinition ColorSetting(
            string id,
            string field,
            RelationshipCategory category,
            Color defaultValue,
            int order) =>
            new SettingDefinition
            {
                Id = id,
                FieldName = field,
                ScribeKey = id,
                Type = SettingType.Color,
                Label = category.ToString(),
                LabelKey = "FactionLens_Category_" + category,
                TooltipKey = "FactionLens_Settings_Color_Tip",
                DefaultValue = defaultValue,
                SortOrder = order,
                ShowInSimpleView = true
            };

        private static bool DrawPreview(
            Rect rect,
            string label,
            string tooltip,
            object value,
            bool disabled)
        {
            var settings = (FactionLensSettings)value;
            RelationshipCategory[] categories =
            {
                RelationshipCategory.Hostile,
                RelationshipCategory.Neutral,
                RelationshipCategory.Allied,
                RelationshipCategory.Player,
                RelationshipCategory.Factionless,
                RelationshipCategory.Unknown
            };
            float width = rect.width / categories.Length;
            for (int index = 0; index < categories.Length; index++)
            {
                RelationshipCategory category = categories[index];
                Rect cell = new Rect(
                    rect.x + width * index,
                    rect.y,
                    width,
                    rect.height);
                LabelDrawer.Draw(
                    cell.ContractedBy(2f),
                    FactionLensSettingsUi.CategoryLabel(category),
                    settings.ColorFor(category),
                    settings);
            }

            return false;
        }
    }
}
