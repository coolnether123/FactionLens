using System;
using FactionLens.Domain;
using FactionLens.Presentation;
using Spine.UI.ColourPicker;
using Spine.UI.SettingsFramework;
using UnityEngine;
using Verse;

namespace FactionLens.Settings
{
    internal sealed class FactionLensSettingsUi
    {
        private const float RowHeight = 30f;
        private const float Gap = 5f;
        private Vector2 scrollPosition;

        internal void Draw(
            Rect inRect,
            FactionLensSettings settings)
        {
            Rect viewRect = new Rect(
                0f,
                0f,
                inRect.width - 18f,
                735f);
            Widgets.BeginScrollView(
                inRect,
                ref scrollPosition,
                viewRect);

            float y = 0f;
            SettingWidgets.DrawHeader(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Visuals".Translate());
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Enabled".Translate(),
                ref settings.FeatureEnabled,
                "FactionLens_Settings_Enabled_Tip".Translate());
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Background".Translate(),
                ref settings.ShowBackground,
                "FactionLens_Settings_Background_Tip".Translate());
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Outline".Translate(),
                ref settings.ShowOutline,
                "FactionLens_Settings_Outline_Tip".Translate());
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Legend".Translate(),
                ref settings.ShowLegend);

            y += Gap;
            SettingWidgets.DrawHeader(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_ObjectTypes".Translate());
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Settlements".Translate(),
                ref settings.ShowSettlements);
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Sites".Translate(),
                ref settings.ShowSites);
            SettingWidgets.DrawBool(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Other".Translate(),
                ref settings.ShowOtherFactionObjects,
                "FactionLens_Settings_Other_Tip".Translate());

            y += Gap;
            SettingWidgets.DrawHeader(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Colors".Translate());
            DrawColorRow(
                Next(ref y, viewRect.width),
                "FactionLens_Category_Hostile".Translate(),
                () => settings.HostileColor,
                value => settings.HostileColor = value);
            DrawColorRow(
                Next(ref y, viewRect.width),
                "FactionLens_Category_Neutral".Translate(),
                () => settings.NeutralColor,
                value => settings.NeutralColor = value);
            DrawColorRow(
                Next(ref y, viewRect.width),
                "FactionLens_Category_Allied".Translate(),
                () => settings.AlliedColor,
                value => settings.AlliedColor = value);
            DrawColorRow(
                Next(ref y, viewRect.width),
                "FactionLens_Category_Player".Translate(),
                () => settings.PlayerColor,
                value => settings.PlayerColor = value);
            DrawColorRow(
                Next(ref y, viewRect.width),
                "FactionLens_Category_Factionless".Translate(),
                () => settings.FactionlessColor,
                value => settings.FactionlessColor = value);
            DrawColorRow(
                Next(ref y, viewRect.width),
                "FactionLens_Category_Unknown".Translate(),
                () => settings.UnknownColor,
                value => settings.UnknownColor = value);

            Rect buttonRow = Next(ref y, viewRect.width);
            float buttonWidth = (buttonRow.width - Gap) / 2f;
            if (SettingWidgets.DrawButton(
                new Rect(
                    buttonRow.x,
                    buttonRow.y,
                    buttonWidth,
                    buttonRow.height),
                "FactionLens_Settings_Colorblind".Translate(),
                "FactionLens_Settings_Colorblind_Tip".Translate()))
            {
                settings.ApplyColorblindPreset();
            }
            if (SettingWidgets.DrawButton(
                new Rect(
                    buttonRow.x + buttonWidth + Gap,
                    buttonRow.y,
                    buttonWidth,
                    buttonRow.height),
                "FactionLens_Settings_Reset".Translate()))
            {
                settings.ApplyDefaults();
            }

            y += Gap;
            SettingWidgets.DrawHeader(
                Next(ref y, viewRect.width),
                "FactionLens_Settings_Preview".Translate());
            DrawPreview(
                new Rect(0f, y, viewRect.width, 92f),
                settings);

            Widgets.EndScrollView();
        }

        private static Rect Next(ref float y, float width)
        {
            Rect row = new Rect(0f, y, width, RowHeight);
            y += RowHeight + Gap;
            return row;
        }

        private static void DrawColorRow(
            Rect rect,
            string label,
            Func<Color> getter,
            Action<Color> setter)
        {
            Color value = getter();
            SettingWidgets.DrawColor(
                rect,
                label,
                ref value,
                "FactionLens_Settings_Color_Tip".Translate(),
                false,
                (initial, ignored) =>
                {
                    Color original = getter();
                    var picker = new Dialog_ColourPicker(
                        initial,
                        (selected, closing) => setter(selected),
                        previewCallback: setter);
                    picker.onCancel = () => setter(original);
                    Find.WindowStack.Add(picker);
                },
                "FactionLens_Settings_Edit".Translate());
        }

        private static void DrawPreview(
            Rect rect,
            FactionLensSettings settings)
        {
            RelationshipCategory[] categories =
            {
                RelationshipCategory.Hostile,
                RelationshipCategory.Neutral,
                RelationshipCategory.Allied,
                RelationshipCategory.Player,
                RelationshipCategory.Factionless,
                RelationshipCategory.Unknown
            };

            float columnWidth = rect.width / 3f;
            for (int index = 0; index < categories.Length; index++)
            {
                RelationshipCategory category = categories[index];
                string label = CategoryLabel(category);
                Vector2 size = LabelDrawer.Measure(label);
                int column = index % 3;
                int row = index / 3;
                Rect labelRect = new Rect(
                    rect.x + column * columnWidth +
                        (columnWidth - size.x) / 2f,
                    rect.y + row * 42f,
                    size.x,
                    size.y);
                LabelDrawer.Draw(
                    labelRect,
                    label,
                    settings.ColorFor(category),
                    settings);
            }
        }

        internal static string CategoryLabel(
            RelationshipCategory category)
        {
            switch (category)
            {
                case RelationshipCategory.Hostile:
                    return "FactionLens_Category_Hostile".Translate();
                case RelationshipCategory.Neutral:
                    return "FactionLens_Category_Neutral".Translate();
                case RelationshipCategory.Allied:
                    return "FactionLens_Category_Allied".Translate();
                case RelationshipCategory.Player:
                    return "FactionLens_Category_Player".Translate();
                case RelationshipCategory.Factionless:
                    return "FactionLens_Category_Factionless".Translate();
                default:
                    return "FactionLens_Category_Unknown".Translate();
            }
        }
    }
}
