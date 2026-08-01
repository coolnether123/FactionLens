using FactionLens.Domain;
using Spine.UI.SettingsFramework;
using UnityEngine;
using Verse;

namespace FactionLens.Settings
{
    internal sealed class FactionLensSettingsUi
    {
        private readonly SettingsListDrawer drawer =
            new SettingsListDrawer(FactionLensSettingsRegistry.Hierarchy)
            {
                SimpleLabel = "Simple",
                AdvancedLabel = "Advanced",
                NoResultsLabel = "No settings match",
                ResetToDefaultLabel = "Reset to default",
                EditColorLabel = "FactionLens_Settings_Edit".Translate(),
                GetLabel = definition => TranslateOrFallback(
                    definition.LabelKey,
                    definition.Label),
                GetTooltip = definition => TranslateOrFallback(
                    definition.TooltipKey,
                    definition.Tooltip),
                RowHeight = 38f
            };
        private SettingsViewMode viewMode = SettingsViewMode.Simple;

        internal SettingsListDrawer Drawer => drawer;

        internal void Draw(Rect inRect, FactionLensSettings settings)
        {
            drawer.Draw(inRect, settings, ref viewMode, settings.Write);
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

        private static string TranslateOrFallback(
            string key,
            string fallback) =>
            string.IsNullOrEmpty(key)
                ? fallback ?? string.Empty
                : key.Translate().ToString();
    }
}
