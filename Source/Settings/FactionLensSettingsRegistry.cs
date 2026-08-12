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
            new SettingsSchema<FactionLensSettings>(
                SettingsSchemaConventions.LowerCamelCase);

        static FactionLensSettingsRegistry()
        {
            var visuals = Schema.Section("visuals.header", "Visuals",
                "FactionLens_Settings_Visuals",
                header => header.HeaderColor = new Color(0.55f, 0.75f, 0.9f));
            visuals.Toggle("visuals.enabled", s => s.FeatureEnabled, "Enable labels")
                .Localized("FactionLens_Settings_Enabled", "FactionLens_Settings_Enabled_Tip");
            visuals.Toggle("visuals.background", s => s.ShowBackground, "Show label backgrounds")
                .Localized("FactionLens_Settings_Background", "FactionLens_Settings_Background_Tip");
            visuals.Enum("visuals.fontSize", s => s.LabelFontSize, "Label font size",
                labelProvider: FontSizeLabel)
                .Localized("FactionLens_Settings_FontSize", "FactionLens_Settings_FontSize_Tip");
            // Floor of 0.35 rather than 0: a label faded to nothing is
            // indistinguishable from the feature being broken, and the
            // enable toggle already covers "off".
            visuals.Slider("visuals.opacity", s => s.LabelOpacity, "Label opacity")
                .Range(0.35f, 1f).Step(0.05f).ShowsPercent()
                .Localized("FactionLens_Settings_Opacity", "FactionLens_Settings_Opacity_Tip");
            // Corner rounding is a property of the nameplate, so the control
            // is meaningless while the nameplate itself is switched off.
            visuals.Toggle("visuals.rounded", s => s.RoundedNameplates,
                "Rounded nameplate corners").AdvancedOnly()
                .ShownWhen(settings => ((FactionLensSettings)settings).ShowBackground)
                .Localized("FactionLens_Settings_Rounded", "FactionLens_Settings_Rounded_Tip");
            // The nameplate already supplies contrast, so an outline behind
            // it changes nothing a player can see. Hide the control while the
            // background is on rather than offer a setting that does nothing.
            visuals.Toggle("visuals.outline", s => s.ShowOutline, "Show label outlines")
                .ShownWhen(settings => !((FactionLensSettings)settings).ShowBackground)
                .Localized("FactionLens_Settings_Outline", "FactionLens_Settings_Outline_Tip");
            visuals.Toggle("visuals.displaced", s => s.ShowDisplacedLabels,
                "Keep labels that cannot fit under their icon").AdvancedOnly()
                .Localized("FactionLens_Settings_Displaced", "FactionLens_Settings_Displaced_Tip");
            visuals.Toggle("visuals.hoveronly", s => s.LabelsOnHoverOnly,
                "Only show a name when pointed at")
                .Localized("FactionLens_Settings_HoverOnly", "FactionLens_Settings_HoverOnly_Tip");
            visuals.Toggle("visuals.playerpriority", s => s.PrioritizePlayerLabels,
                "Give player colony names first claim").AdvancedOnly()
                .Localized("FactionLens_Settings_PlayerPriority", "FactionLens_Settings_PlayerPriority_Tip");
            visuals.Toggle("visuals.legend", s => s.ShowLegend, "Show legend")
                .Localized("FactionLens_Settings_Legend", "FactionLens_Settings_Legend_Tip");

            var objects = Schema.Section("objects.header", "Object types",
                "FactionLens_Settings_ObjectTypes",
                header => header.HeaderColor = new Color(0.5f, 0.8f, 0.5f));
            objects.Toggle("objects.settlements", s => s.ShowSettlements, "Show settlements")
                .Localized("FactionLens_Settings_Settlements", "FactionLens_Settings_Settlements_Tip");
            objects.Toggle("objects.sites", s => s.ShowSites, "Show sites")
                .Localized("FactionLens_Settings_Sites", "FactionLens_Settings_Sites_Tip");
            objects.Toggle("objects.other", s => s.ShowOtherFactionObjects,
                "Show other faction objects").AdvancedOnly()
                .Localized("FactionLens_Settings_Other", "FactionLens_Settings_Other_Tip");

            var colors = Schema.Section("colors.header", "Relationship colors",
                "FactionLens_Settings_Colors",
                header => header.HeaderColor = new Color(0.9f, 0.7f, 0.4f));
            colors.Colour("colors.hostile", s => s.HostileColor, "Hostile")
                .Localized("FactionLens_Category_Hostile", "FactionLens_Settings_Color_Tip");
            colors.Colour("colors.neutral", s => s.NeutralColor, "Neutral")
                .Localized("FactionLens_Category_Neutral", "FactionLens_Settings_Color_Tip");
            colors.Colour("colors.allied", s => s.AlliedColor, "Allied")
                .Localized("FactionLens_Category_Allied", "FactionLens_Settings_Color_Tip");
            colors.Colour("colors.player", s => s.PlayerColor, "Player")
                .Localized("FactionLens_Category_Player", "FactionLens_Settings_Color_Tip");
            colors.Colour("colors.factionless", s => s.FactionlessColor, "Abandoned")
                .Localized("FactionLens_Category_Factionless", "FactionLens_Settings_Color_Tip");
            colors.Colour("colors.unknown", s => s.UnknownColor, "Unknown")
                .Localized("FactionLens_Category_Unknown", "FactionLens_Settings_Color_Tip");

            colors.Button("colors.colorblind", "Colorblind preset",
                onChanged: settings => settings.ApplyColorblindPreset())
                .Localized("FactionLens_Settings_Colorblind",
                    "FactionLens_Settings_Colorblind_Tip");
            colors.Button("colors.reset", "Reset all",
                onChanged: settings => settings.ApplyDefaults())
                .Localized("FactionLens_Settings_Reset",
                    "FactionLens_Settings_Reset_Tip")
                .AdvancedOnly();
            // Pinned so the swatches stay on screen while the colour rows
            // above them scroll; changing a colour is pointless if you have
            // to scroll away from the preview to reach the control.
            colors.Custom("preview.labels", DrawPreview)
                .Pinned(SettingPin.Bottom);
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
                string categoryLabel = CategoryLabel(category);
                Vector2 labelSize = LabelDrawer.Measure(categoryLabel, settings);
                Rect labelRect = new Rect(
                    cell.center.x - labelSize.x / 2f,
                    cell.center.y - labelSize.y / 2f,
                    labelSize.x,
                    labelSize.y);
                LabelDrawer.Draw(
                    labelRect,
                    categoryLabel,
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
            return ("FactionLens_Settings_FontSize_" + value).Translate();
        }
    }
}
