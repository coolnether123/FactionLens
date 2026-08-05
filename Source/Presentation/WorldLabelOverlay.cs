using System;
using System.Collections.Generic;
using FactionLens.Domain;
using FactionLens.Ownership;
using FactionLens.Settings;
using RimWorld.Planet;
using Spine.Caching;
using Spine.UI.ContextualSettings;
using UnityEngine;
using Verse;

namespace FactionLens.Presentation
{
    internal static class WorldLabelOverlay
    {
        private static readonly ScreenCollisionIndex CollisionIndex =
            new ScreenCollisionIndex();
        private static readonly BoundedLruCache<string, Vector2> LabelSizes =
            new BoundedLruCache<string, Vector2>(
                64 * 1024,
                StringComparer.Ordinal);
        private static readonly RelationshipCategory[] LegendCategories =
        {
            RelationshipCategory.Player,
            RelationshipCategory.Allied,
            RelationshipCategory.Neutral,
            RelationshipCategory.Hostile,
            RelationshipCategory.Factionless,
            RelationshipCategory.Unknown
        };
        private static readonly List<PlacedLabel> PlacedLabels =
            new List<PlacedLabel>();
        private static WorldObject pendingSelection;

        internal static void Draw()
        {
            ApplyPendingSelection();

            Event currentEvent = Event.current;
            FactionLensSettings settings =
                Bootstrap.FactionLensMod.Settings;
            if (settings == null ||
                !settings.FeatureEnabled ||
                currentEvent.type != EventType.Repaint ||
                !DebugViewSettings.drawWorldObjects ||
                Find.WorldObjects == null)
            {
                return;
            }

            Process(
                settings,
                repaint: true,
                leftClick: false,
                currentEvent.mousePosition);
        }

        internal static void HandleInput()
        {
            Event currentEvent = Event.current;
            FactionLensSettings settings =
                Bootstrap.FactionLensMod.Settings;
            if (settings == null ||
                !settings.FeatureEnabled ||
                currentEvent.type != EventType.MouseDown ||
                currentEvent.button != 0 ||
                !DebugViewSettings.drawWorldObjects ||
                Find.WorldObjects == null)
            {
                return;
            }

            Process(
                settings,
                repaint: false,
                leftClick: true,
                currentEvent.mousePosition);
        }

        private static void Process(
            FactionLensSettings settings,
            bool repaint,
            bool leftClick,
            Vector2 mousePosition)
        {
            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            Color previousColor = GUI.color;
            try
            {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleCenter;
                CollisionIndex.Clear();
                CollisionIndex.MaxVerticalShifts =
                    settings.ShowDisplacedLabels
                        ? ScreenCollisionIndex.DefaultVerticalShifts
                        : 0;
                PlacedLabels.Clear();

                if (settings.ShowLegend &&
                    Bootstrap.FactionLensMod.ContextualSettings?.Bind(
                        LegendPanelRect(),
                        ContextualSettingsTarget.Exact(
                            "visuals.legend",
                            "visuals.header"),
                        ContextualSettingsBindingOptions.HintOnly(
                            priority: 5)) == true)
                {
                    return;
                }

                List<WorldObject> objects =
                    Find.WorldObjects.AllWorldObjects;
                for (int index = 0; index < objects.Count; index++)
                {
                    WorldObject worldObject = objects[index];
                    try
                    {
                        if (TryPlaceObject(
                            worldObject,
                            settings,
                            out PlacedLabel placedLabel))
                        {
                            PlacedLabels.Add(placedLabel);
                        }
                    }
                    catch (Exception exception)
                    {
                        LogSkippedObject(worldObject, exception);
                    }
                }

                for (int index = 0; index < PlacedLabels.Count; index++)
                {
                    PlacedLabel placedLabel = PlacedLabels[index];
                    try
                    {
                        if (BindAndHandleInput(
                            placedLabel,
                            leftClick,
                            mousePosition))
                        {
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        LogSkippedObject(
                            placedLabel.WorldObject,
                            exception);
                    }
                }

                if (repaint)
                {
                    for (int index = 0;
                        index < PlacedLabels.Count;
                        index++)
                    {
                        PlacedLabel placedLabel = PlacedLabels[index];
                        try
                        {
                            DrawConnector(placedLabel);
                        }
                        catch (Exception exception)
                        {
                            LogSkippedObject(
                                placedLabel.WorldObject,
                                exception);
                        }
                    }

                    for (int index = 0;
                        index < PlacedLabels.Count;
                        index++)
                    {
                        PlacedLabel placedLabel = PlacedLabels[index];
                        try
                        {
                            DrawPlacedLabel(
                                placedLabel,
                                settings,
                                mousePosition);
                        }
                        catch (Exception exception)
                        {
                            LogSkippedObject(
                                placedLabel.WorldObject,
                                exception);
                        }
                    }
                }

                if (repaint && settings.ShowLegend)
                {
                    DrawLegend(settings);
                }
            }
            catch (Exception exception)
            {
                Log.ErrorOnce(
                    "[Faction Lens] World label overlay failed: " +
                    exception,
                    201607301);
            }
            finally
            {
                CollisionIndex.Clear();
                PlacedLabels.Clear();
                Text.Font = previousFont;
                Text.Anchor = previousAnchor;
                GUI.color = previousColor;
            }
        }

        private static bool TryPlaceObject(
            WorldObject worldObject,
            FactionLensSettings settings,
            out PlacedLabel placedLabel)
        {
            placedLabel = default;
            if (worldObject == null ||
                worldObject.Destroyed ||
                worldObject.def == null ||
                worldObject.HiddenBehindTerrainNow())
            {
                return false;
            }

            float transition =
                ExpandableWorldObjectsUtility.TransitionPct(worldObject);
            if (transition <= 0.08f)
            {
                return false;
            }

            if (!OwnershipService.TryClassify(
                worldObject,
                out RelationshipCategory category,
                out WorldObjectKind kind) ||
                !settings.IsKindEnabled(kind))
            {
                return false;
            }

            string label = worldObject.LabelCap;
            if (label.NullOrEmpty())
            {
                return false;
            }

            if (!LabelSizes.TryGet(label, out Vector2 size))
            {
                size = LabelDrawer.Measure(label);
                LabelSizes.AddOrUpdate(
                    label,
                    size,
                    Math.Max(16, label.Length * sizeof(char) + 16));
            }
            Rect iconRect =
                ExpandableWorldObjectsUtility.ExpandedIconScreenRect(
                    worldObject);
            Rect labelRect = new Rect(
                iconRect.center.x - size.x / 2f,
                iconRect.yMax + 2f,
                size.x,
                size.y);

            if (labelRect.xMax < 0f ||
                labelRect.x > UI.screenWidth ||
                labelRect.yMax < 0f ||
                labelRect.y > UI.screenHeight)
            {
                return false;
            }

            var candidate = new ScreenBounds(
                labelRect.x,
                labelRect.y,
                labelRect.width,
                labelRect.height);
            if (!CollisionIndex.TryPlace(
                candidate,
                out ScreenBounds placed))
            {
                return false;
            }

            labelRect.y = placed.Y;
            placedLabel = new PlacedLabel(
                worldObject,
                category,
                kind,
                label,
                transition,
                iconRect,
                new Rect(
                    candidate.X,
                    candidate.Y,
                    candidate.Width,
                    candidate.Height),
                labelRect);
            return true;
        }

        private static bool BindAndHandleInput(
            PlacedLabel placedLabel,
            bool leftClick,
            Vector2 mousePosition)
        {
            string objectSettingId;
            switch (placedLabel.Kind)
            {
                case WorldObjectKind.Settlement:
                    objectSettingId = "objects.settlements";
                    break;
                case WorldObjectKind.Site:
                    objectSettingId = "objects.sites";
                    break;
                default:
                    objectSettingId = "objects.other";
                    break;
            }

            if (Bootstrap.FactionLensMod.ContextualSettings?.Bind(
                placedLabel.LabelRect,
                ContextualSettingsTarget.Exact(
                    objectSettingId,
                    "objects.header"),
                ContextualSettingsBindingOptions.HintOnly(
                    priority: 5)) == true)
            {
                return true;
            }

            Bootstrap.FactionLensMod.ContextualSettings?.Bind(
                placedLabel.LabelRect.ContractedBy(4f, 1f),
                ContextualSettingsTarget.Exact(
                    ColorSettingId(placedLabel.Category),
                    "colors.header"),
                new ContextualSettingsBindingOptions(priority: 10));
            if (leftClick && placedLabel.LabelRect.Contains(mousePosition))
            {
                WorldObject worldObject = placedLabel.WorldObject;
                pendingSelection = worldObject;
                Event.current.Use();
                return true;
            }

            return false;
        }

        private static void DrawConnector(PlacedLabel placedLabel)
        {
            if (Mathf.Abs(
                placedLabel.LabelRect.y -
                placedLabel.NaturalLabelRect.y) <= 0.5f)
            {
                return;
            }

            Color connectorColor = Color.white;
            connectorColor.a = 0.72f *
                Mathf.Clamp01(placedLabel.Transition * 2f);
            Widgets.DrawLine(
                new Vector2(
                    placedLabel.IconRect.center.x,
                    placedLabel.IconRect.yMax),
                new Vector2(
                    placedLabel.LabelRect.center.x,
                    placedLabel.LabelRect.yMin),
                connectorColor,
                1f);
        }

        private static void DrawPlacedLabel(
            PlacedLabel placedLabel,
            FactionLensSettings settings,
            Vector2 mousePosition)
        {
            Color color = settings.ColorFor(placedLabel.Category);
            color.a *= Mathf.Clamp01(placedLabel.Transition * 2f);
            LabelDrawer.Draw(
                placedLabel.LabelRect,
                placedLabel.Label,
                color,
                settings,
                placedLabel.LabelRect.Contains(mousePosition));
        }

        private static void LogSkippedObject(
            WorldObject worldObject,
            Exception exception)
        {
            int objectId = worldObject?.ID ?? -1;
            Log.ErrorOnce(
                "[Faction Lens] Skipped a world object whose " +
                "label could not be resolved: " + exception,
                201607400 ^ objectId);
        }

        private static void ApplyPendingSelection()
        {
            WorldObject worldObject = pendingSelection;
            pendingSelection = null;
            if (worldObject == null || worldObject.Destroyed)
            {
                return;
            }

            Find.WorldSelector.ClearSelection();
            Find.WorldSelector.Select(
                worldObject,
                playSound: true);
        }

        private static void DrawLegend(FactionLensSettings settings)
        {
            const float rowHeight = 20f;
            Rect panel = LegendPanelRect();
            Widgets.DrawBoxSolid(
                panel,
                new Color(0.035f, 0.035f, 0.035f, 0.86f));
            Widgets.DrawBox(panel, 1);

            TextAnchor previousAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
            Widgets.Label(
                new Rect(
                    panel.x + 8f,
                    panel.y + 3f,
                    panel.width - 16f,
                    22f),
                "FactionLens_Legend_Title".Translate());

            for (int index = 0; index < LegendCategories.Length; index++)
            {
                RelationshipCategory category = LegendCategories[index];
                float y = panel.y + 27f + index * rowHeight;
                Rect row = new Rect(
                    panel.x + 5f,
                    y,
                    panel.width - 10f,
                    rowHeight);
                Bootstrap.FactionLensMod.ContextualSettings?.Bind(
                    row,
                    ContextualSettingsTarget.Exact(
                        ColorSettingId(category),
                        "colors.header"),
                    new ContextualSettingsBindingOptions(priority: 10));
                Widgets.DrawBoxSolid(
                    new Rect(panel.x + 9f, y + 4f, 12f, 12f),
                    settings.ColorFor(category));
                GUI.color = Color.white;
                Widgets.Label(
                    new Rect(
                        panel.x + 27f,
                        y,
                        panel.width - 34f,
                        rowHeight),
                    FactionLensSettingsRegistry.CategoryLabel(category));
            }

            Text.Anchor = previousAnchor;
        }

        private static Rect LegendPanelRect()
        {
            const float width = 154f;
            const float rowHeight = 20f;
            const int categoryCount = 6;
            return new Rect(
                12f,
                72f,
                width,
                26f + categoryCount * rowHeight + 6f);
        }

        private static string ColorSettingId(
            RelationshipCategory category)
        {
            switch (category)
            {
                case RelationshipCategory.Hostile:
                    return "colors.hostile";
                case RelationshipCategory.Neutral:
                    return "colors.neutral";
                case RelationshipCategory.Allied:
                    return "colors.allied";
                case RelationshipCategory.Player:
                    return "colors.player";
                case RelationshipCategory.Factionless:
                    return "colors.factionless";
                default:
                    return "colors.unknown";
            }
        }

        private readonly struct PlacedLabel
        {
            internal PlacedLabel(
                WorldObject worldObject,
                RelationshipCategory category,
                WorldObjectKind kind,
                string label,
                float transition,
                Rect iconRect,
                Rect naturalLabelRect,
                Rect labelRect)
            {
                WorldObject = worldObject;
                Category = category;
                Kind = kind;
                Label = label;
                Transition = transition;
                IconRect = iconRect;
                NaturalLabelRect = naturalLabelRect;
                LabelRect = labelRect;
            }

            internal WorldObject WorldObject { get; }
            internal RelationshipCategory Category { get; }
            internal WorldObjectKind Kind { get; }
            internal string Label { get; }
            internal float Transition { get; }
            internal Rect IconRect { get; }
            internal Rect NaturalLabelRect { get; }
            internal Rect LabelRect { get; }
        }
    }
}
