using FactionLens.Domain;
using Spine.Api;
using UnityEngine;
using Verse;

namespace FactionLens.Settings
{
    public sealed class FactionLensSettings : ModSettings
    {
        public bool FeatureEnabled = true;
        public bool ShowSettlements = true;
        public bool ShowSites = true;
        public bool ShowOtherFactionObjects = true;
        public bool ShowLegend;
        public bool ShowBackground = true;
        public bool ShowOutline;

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
            ShowOutline = false;
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
