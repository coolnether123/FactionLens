using FactionLens.Domain;
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
#if RWT_LEGACY_BCL
            // RimWorld 1.0's deep settings serializer does not reliably invoke
            // the shared definition-driven facade for this consumer. Keep the
            // schema/UI ownership in Spine, but use the authoritative 1.0
            // primitive write path for the persisted fields.
            Scribe_Values.Look(ref FeatureEnabled, "featureEnabled", true);
            Scribe_Values.Look(ref ShowSettlements, "showSettlements", true);
            Scribe_Values.Look(ref ShowSites, "showSites", true);
            Scribe_Values.Look(ref ShowOtherFactionObjects, "showOtherFactionObjects", true);
            Scribe_Values.Look(ref ShowLegend, "showLegend", false);
            Scribe_Values.Look(ref ShowBackground, "showBackground", true);
            Scribe_Values.Look(ref RoundedNameplates, "roundedNameplates", true);
            Scribe_Values.Look(ref LabelFontSize, "labelFontSize", FactionLensLabelFontSize.Tiny);
            Scribe_Values.Look(ref LabelOpacity, "labelOpacity", 0.8f);
            Scribe_Values.Look(ref ShowOutline, "showOutline", false);
            Scribe_Values.Look(ref ShowDisplacedLabels, "showDisplacedLabels", false);
            Scribe_Values.Look(ref PrioritizePlayerLabels, "prioritizePlayerLabels", true);
            Scribe_Values.Look(ref LabelsOnHoverOnly, "labelsOnHoverOnly", false);
            Scribe_Values.Look(ref HostileColor, "hostileColor", FactionLensPalette.Default.Hostile);
            Scribe_Values.Look(ref NeutralColor, "neutralColor", FactionLensPalette.Default.Neutral);
            Scribe_Values.Look(ref AlliedColor, "alliedColor", FactionLensPalette.Default.Allied);
            Scribe_Values.Look(ref PlayerColor, "playerColor", FactionLensPalette.Default.Player);
            Scribe_Values.Look(ref FactionlessColor, "factionlessColor", FactionLensPalette.Default.Factionless);
            Scribe_Values.Look(ref UnknownColor, "unknownColor", FactionLensPalette.Default.Unknown);
#else
            FactionLensSettingsRegistry.Schema.Scribe(this);
#endif
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
