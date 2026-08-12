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

        internal static readonly SettingsSchema<FactionLensSettings> Schema =
            BuildSchema();

        internal static readonly IReadOnlyList<SettingDefinition> Definitions =
            BuildDefinitions(Schema);

        private static SettingsSchema<FactionLensSettings> BuildSchema()
        {
            var schema = new SettingsSchema<FactionLensSettings>(
                SettingsSchemaConventions.LowerCamelCase);

            SettingsScope<FactionLensSettings> visuals = Section(
                schema,
                "visuals.header",
                "Visuals",
                "FactionLens_Settings_Visuals",
                new Color(0.55f, 0.75f, 0.9f));
            Localize(
                visuals.Toggle(
                    "visuals.enabled",
                    settings => settings.FeatureEnabled,
                    "Enable labels"),
                "FactionLens_Settings_Enabled",
                "FactionLens_Settings_Enabled_Tip");
            Localize(
                visuals.Toggle(
                    "visuals.background",
                    settings => settings.ShowBackground,
                    "Show label backgrounds"),
                "FactionLens_Settings_Background",
                "FactionLens_Settings_Background_Tip");
            Localize(
                visuals.Enum(
                    "visuals.fontSize",
                    settings => settings.LabelFontSize,
                    "Label font size",
                    labelProvider: FontSizeLabel),
                "FactionLens_Settings_FontSize",
                "FactionLens_Settings_FontSize_Tip");
                // Floor of 0.35 rather than 0: a label faded to nothing is
                // indistinguishable from the feature being broken, and the
                // enable toggle already covers "off".
            Localize(
                visuals.Slider(
                        "visuals.opacity",
                        settings => settings.LabelOpacity,
                        "Label opacity")
                    .Range(0.35f, 1f)
                    .Step(0.05f)
                    .ShowsPercent(),
                "FactionLens_Settings_Opacity",
                "FactionLens_Settings_Opacity_Tip");
                // Corner rounding is a property of the nameplate, so the control
                // is meaningless while the nameplate itself is switched off.
            Localize(
                visuals.Toggle(
                        "visuals.rounded",
                        settings => settings.RoundedNameplates,
                        "Rounded nameplate corners")
                    .AdvancedOnly()
                    .ShownWhen(settings => ((FactionLensSettings)settings).ShowBackground),
                "FactionLens_Settings_Rounded",
                "FactionLens_Settings_Rounded_Tip");
                // The nameplate already supplies contrast, so an outline behind
                // it changes nothing a player can see. Hide the control while the
                // background is on rather than offer a setting that does nothing.
            Localize(
                visuals.Toggle(
                        "visuals.outline",
                        settings => settings.ShowOutline,
                        "Show label outlines")
                    .ShownWhen(settings => !((FactionLensSettings)settings).ShowBackground),
                "FactionLens_Settings_Outline",
                "FactionLens_Settings_Outline_Tip");
            Localize(
                visuals.Toggle(
                        "visuals.displaced",
                        settings => settings.ShowDisplacedLabels,
                        "Keep labels that cannot fit under their icon")
                    .AdvancedOnly(),
                "FactionLens_Settings_Displaced",
                "FactionLens_Settings_Displaced_Tip");
            Localize(
                visuals.Toggle(
                    "visuals.hoveronly",
                    settings => settings.LabelsOnHoverOnly,
                    "Only show a name when pointed at"),
                "FactionLens_Settings_HoverOnly",
                "FactionLens_Settings_HoverOnly_Tip");
            Localize(
                visuals.Toggle(
                        "visuals.playerpriority",
                        settings => settings.PrioritizePlayerLabels,
                        "Give player colony names first claim")
                    .AdvancedOnly(),
                "FactionLens_Settings_PlayerPriority",
                "FactionLens_Settings_PlayerPriority_Tip");
            Localize(
                visuals.Toggle(
                    "visuals.legend",
                    settings => settings.ShowLegend,
                    "Show legend"),
                "FactionLens_Settings_Legend",
                "FactionLens_Settings_Legend_Tip");

            SettingsScope<FactionLensSettings> objects = Section(
                schema,
                "objects.header",
                "Object types",
                "FactionLens_Settings_ObjectTypes",
                new Color(0.5f, 0.8f, 0.5f));
            Localize(
                objects.Toggle(
                    "objects.settlements",
                    settings => settings.ShowSettlements,
                    "Show settlements"),
                "FactionLens_Settings_Settlements",
                "FactionLens_Settings_Settlements_Tip");
            Localize(
                objects.Toggle(
                    "objects.sites",
                    settings => settings.ShowSites,
                    "Show sites"),
                "FactionLens_Settings_Sites",
                "FactionLens_Settings_Sites_Tip");
            Localize(
                objects.Toggle(
                        "objects.other",
                        settings => settings.ShowOtherFactionObjects,
                        "Show other faction objects")
                    .AdvancedOnly(),
                "FactionLens_Settings_Other",
                "FactionLens_Settings_Other_Tip");

            SettingsScope<FactionLensSettings> colors = Section(
                schema,
                "colors.header",
                "Relationship colors",
                "FactionLens_Settings_Colors",
                new Color(0.9f, 0.7f, 0.4f));
            Localize(
                colors.Colour(
                    "colors.hostile",
                    settings => settings.HostileColor,
                    "Hostile"),
                "FactionLens_Category_Hostile",
                "FactionLens_Settings_Color_Tip");
            Localize(
                colors.Colour(
                    "colors.neutral",
                    settings => settings.NeutralColor,
                    "Neutral"),
                "FactionLens_Category_Neutral",
                "FactionLens_Settings_Color_Tip");
            Localize(
                colors.Colour(
                    "colors.allied",
                    settings => settings.AlliedColor,
                    "Allied"),
                "FactionLens_Category_Allied",
                "FactionLens_Settings_Color_Tip");
            Localize(
                colors.Colour(
                    "colors.player",
                    settings => settings.PlayerColor,
                    "Player"),
                "FactionLens_Category_Player",
                "FactionLens_Settings_Color_Tip");
            Localize(
                colors.Colour(
                    "colors.factionless",
                    settings => settings.FactionlessColor,
                    "Abandoned"),
                "FactionLens_Category_Factionless",
                "FactionLens_Settings_Color_Tip");
            Localize(
                colors.Colour(
                    "colors.unknown",
                    settings => settings.UnknownColor,
                    "Unknown"),
                "FactionLens_Category_Unknown",
                "FactionLens_Settings_Color_Tip");

            return schema;
        }

        private static IReadOnlyList<SettingDefinition> BuildDefinitions(
            SettingsSchema<FactionLensSettings> schema)
        {
            var definitions = new List<SettingDefinition>(schema.Definitions);

            SettingDefinition colorblind = SettingDefinitions.Button(
                "colors.colorblind",
                "Colorblind preset",
                value => ((FactionLensSettings)value).ApplyColorblindPreset(),
                "FactionLens_Settings_Colorblind",
                "FactionLens_Settings_Colorblind_Tip");
            colorblind.ParentId = "colors.header";
            definitions.Add(colorblind);

            SettingDefinition reset = SettingDefinitions.Button(
                "colors.reset",
                "Reset all",
                value => ((FactionLensSettings)value).ApplyDefaults(),
                "FactionLens_Settings_Reset",
                "FactionLens_Settings_Reset_Tip");
            reset.ParentId = "colors.header";
            definitions.Add(reset.AdvancedOnly());

            // Pinned so the swatches stay on screen while the colour rows
            // above them scroll; changing a colour is pointless if you have
            // to scroll away from the preview to reach the control.
            SettingDefinition preview = SettingDefinitions.Custom(
                "preview.labels",
                DrawPreview);
            preview.ParentId = "colors.header";
            definitions.Add(preview.Pinned(SettingPin.Bottom));

            return definitions;
        }

        private static SettingsScope<FactionLensSettings> Section(
            SettingsSchema<FactionLensSettings> schema,
            string id,
            string label,
            string labelKey,
            Color color)
        {
            SettingsScope<FactionLensSettings> section = schema.Section(
                id,
                label,
                labelKey);
            schema.Definitions[schema.Definitions.Count - 1].HeaderColor = color;
            return section;
        }

        private static SettingDefinition Localize(
            SettingDefinition definition,
            string labelKey,
            string tooltipKey)
        {
            definition.LabelKey = labelKey;
            definition.TooltipKey = tooltipKey;
            return definition;
        }

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

        private static string FontSizeLabel(FactionLensLabelFontSize value)
        {
            return ("FactionLens_Settings_FontSize_" +
                value).Translate();
        }
    }
}
