using System;
using System.Collections.Generic;
using FactionLens.Domain;
using FactionLens.Ownership;
using FactionLens.Settings;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace FactionLens.Presentation
{
    internal static class WorldLabelOverlay
    {
        private static readonly ScreenCollisionIndex CollisionIndex =
            new ScreenCollisionIndex();
        private static readonly Dictionary<string, Vector2> LabelSizes =
            new Dictionary<string, Vector2>(
                StringComparer.Ordinal);

        internal static void Draw()
        {
            Event currentEvent = Event.current;
            bool repaint =
                currentEvent.type == EventType.Repaint;
            bool leftClick =
                currentEvent.type == EventType.MouseDown &&
                currentEvent.button == 0;
            FactionLensSettings settings =
                Bootstrap.FactionLensMod.Settings;
            if (settings == null ||
                !settings.FeatureEnabled ||
                (!repaint && !leftClick) ||
                !DebugViewSettings.drawWorldObjects ||
                Find.WorldObjects == null)
            {
                return;
            }

            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            Color previousColor = GUI.color;
            try
            {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleCenter;
                CollisionIndex.Clear();

                List<WorldObject> objects =
                    Find.WorldObjects.AllWorldObjects;
                for (int index = 0; index < objects.Count; index++)
                {
                    WorldObject worldObject = objects[index];
                    try
                    {
                        DrawObject(
                            worldObject,
                            settings,
                            repaint,
                            leftClick,
                            currentEvent.mousePosition);
                    }
                    catch (Exception exception)
                    {
                        int objectId = worldObject?.ID ?? -1;
                        Log.ErrorOnce(
                            "[Faction Lens] Skipped a world object whose " +
                            "label could not be resolved: " + exception,
                            201607400 ^ objectId);
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
                Text.Font = previousFont;
                Text.Anchor = previousAnchor;
                GUI.color = previousColor;
            }
        }

        private static void DrawObject(
            WorldObject worldObject,
            FactionLensSettings settings,
            bool repaint,
            bool leftClick,
            Vector2 mousePosition)
        {
            if (worldObject == null ||
                worldObject.Destroyed ||
                worldObject.def == null ||
                worldObject.HiddenBehindTerrainNow())
            {
                return;
            }

            float transition =
                ExpandableWorldObjectsUtility.TransitionPct(worldObject);
            if (transition <= 0.08f)
            {
                return;
            }

            if (!OwnershipService.TryClassify(
                worldObject,
                out RelationshipCategory category,
                out WorldObjectKind kind) ||
                !settings.IsKindEnabled(kind))
            {
                return;
            }

            string label = worldObject.LabelCap;
            if (label.NullOrEmpty())
            {
                return;
            }

            if (!LabelSizes.TryGetValue(label, out Vector2 size))
            {
                if (LabelSizes.Count >= 512)
                {
                    LabelSizes.Clear();
                }

                size = LabelDrawer.Measure(label);
                LabelSizes[label] = size;
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
                return;
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
                return;
            }

            labelRect.y = placed.Y;
            bool mouseOver = placed.Contains(
                mousePosition.x,
                mousePosition.y);
            if (repaint)
            {
                Color color = settings.ColorFor(category);
                color.a *= Mathf.Clamp01(transition * 2f);
                LabelDrawer.Draw(
                    labelRect,
                    label,
                    color,
                    settings,
                    mouseOver);
            }

            if (leftClick && mouseOver)
            {
                Find.WorldSelector.ClearSelection();
                Find.WorldSelector.Select(
                    worldObject,
                    playSound: true);
                Event.current.Use();
            }
        }

        private static void DrawLegend(FactionLensSettings settings)
        {
            RelationshipCategory[] categories =
            {
                RelationshipCategory.Player,
                RelationshipCategory.Allied,
                RelationshipCategory.Neutral,
                RelationshipCategory.Hostile,
                RelationshipCategory.Factionless,
                RelationshipCategory.Unknown
            };

            const float width = 154f;
            const float rowHeight = 20f;
            float height = 26f + categories.Length * rowHeight + 6f;
            Rect panel = new Rect(12f, 72f, width, height);
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

            for (int index = 0; index < categories.Length; index++)
            {
                RelationshipCategory category = categories[index];
                float y = panel.y + 27f + index * rowHeight;
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
                    FactionLensSettingsUi.CategoryLabel(category));
            }

            Text.Anchor = previousAnchor;
        }
    }
}
