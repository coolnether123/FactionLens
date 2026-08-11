using FactionLens.Domain;
using Spine.Api;
using UnityEngine;
using Verse;

namespace FactionLens.Settings
{
    public enum FactionLensLabelFontSize
    {
        Tiny,
        Small,
        Medium
    }

    public sealed class FactionLensSettings : ModSettings
    {
        public bool FeatureEnabled = true;
        public bool ShowSettlements = true;
        public bool ShowSites = true;
        public bool ShowOtherFactionObjects = true;
        public bool ShowLegend;
        public bool ShowBackground = true;
        public bool RoundedNameplates = true;
        public FactionLensLabelFontSize LabelFontSize =
            FactionLensLabelFontSize.Tiny;
        public float LabelOpacity = 0.8f;
        public bool ShowOutline;
        public bool ShowDisplacedLabels;
        public bool PrioritizePlayerLabels = true;
        public bool LabelsOnHoverOnly;

        public Color HostileColor = FactionLensPalette.Default.Hostile;
        public Color NeutralColor = FactionLensPalette.Default.Neutral;
        public Color AlliedColor = FactionLensPalette.Default.Allied;
        public Color PlayerColor = FactionLensPalette.Default.Player;
        public Color FactionlessColor =
            FactionLensPalette.Default.Factionless;
        public Color UnknownColor = FactionLensPalette.Default.Unknown;

        public Color ColorFor(RelationshipCategory category)
        {
            switch (category)
            {
                case RelationshipCategory.Hostile:
                    return HostileColor;
                case RelationshipCategory.Neutral:
                    return NeutralColor;
                case RelationshipCategory.Allied:
                    return AlliedColor;
                case RelationshipCategory.Player:
                    return PlayerColor;
                case RelationshipCategory.Factionless:
                    return FactionlessColor;
                default:
                    return UnknownColor;
            }
        }

        public bool IsKindEnabled(WorldObjectKind kind)
        {
            return WorldObjectKindPolicy.IsEnabled(
                kind,
                ShowSettlements,
                ShowSites,
                ShowOtherFactionObjects);
        }

        public void ApplyDefaults()
        {
            FeatureEnabled = true;
            ShowSettlements = true;
            ShowSites = true;
            ShowOtherFactionObjects = true;
            ShowLegend = false;
            ShowBackground = true;
            RoundedNameplates = true;
            LabelFontSize = FactionLensLabelFontSize.Tiny;
            LabelOpacity = 0.8f;
            ShowOutline = false;
            ShowDisplacedLabels = false;
            PrioritizePlayerLabels = true;
            LabelsOnHoverOnly = false;
            ApplyPalette(FactionLensPalette.Default);
        }

        public void ApplyColorblindPreset()
        {
            ApplyPalette(FactionLensPalette.Colorblind);
        }

        public override void ExposeData()
        {
            SpineApi.Settings.Scribe(
                this,
                FactionLensSettingsRegistry.Definitions);
            base.ExposeData();
        }

        private void ApplyPalette(FactionLensPalette palette)
        {
            HostileColor = palette.Hostile;
            NeutralColor = palette.Neutral;
            AlliedColor = palette.Allied;
            PlayerColor = palette.Player;
            FactionlessColor = palette.Factionless;
            UnknownColor = palette.Unknown;
        }
    }
}
