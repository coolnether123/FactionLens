#if RWT_LEGACY_BOOTSTRAP
using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace FactionLens.Legacy
{
    internal static class FactionLensLegacyOverlay
    {
#if RWT_LEGACY_FACTION_MAP
        internal static void DrawFactionLabels()
        {
            if (Event.current == null ||
                Event.current.type != EventType.Repaint ||
                Find.FactionManager == null)
            {
                return;
            }

#if RWT_LEGACY_FACTION_MAP_013
            Faction colony = Faction.OfColony;
#else
            Faction colony = Faction.OfPlayer;
#endif
            foreach (Faction faction in
                Find.FactionManager.AllFactionsInViewOrder)
            {
#if RWT_LEGACY_FACTION_MAP_013
                if (faction == null ||
                    faction.def == null ||
                    faction.def.hidden ||
                    faction == colony ||
                    faction.homeSquare.IsInvalid ||
                    String.IsNullOrEmpty(faction.name))
#else
                if (faction == null ||
                    faction.def == null ||
                    faction.def.hidden ||
                    faction.IsPlayer ||
                    faction.homeSquare.IsInvalid ||
                    String.IsNullOrEmpty(faction.Name))
#endif
                {
                    continue;
                }

#if RWT_LEGACY_FACTION_MAP_013
                DrawText(
                    new Vector2(faction.homeSquare.x, faction.homeSquare.z),
                    faction.name,
                    ColorFor(faction, colony));
#else
                DrawText(
                    new Vector2(faction.homeSquare.x, faction.homeSquare.z),
                    faction.Name,
                    ColorFor(faction, colony));
#endif
            }
        }

        private static void DrawText(
            Vector2 worldPosition,
            string label,
            Color color)
        {
            Color previousColor = GUI.color;
            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            try
            {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperCenter;
                GenWorldUI.DrawText(worldPosition, label, color);
            }
            finally
            {
                GUI.color = previousColor;
                Text.Font = previousFont;
                Text.Anchor = previousAnchor;
            }
        }
#endif

#if RWT_LEGACY_WORLD_OBJECTS
        internal static void DrawWorldObjectLabels()
        {
            if (Event.current == null ||
                Event.current.type != EventType.Repaint ||
                Find.WorldObjects == null)
            {
                return;
            }

            foreach (WorldObject worldObject in
                Find.WorldObjects.AllWorldObjects)
            {
                if (worldObject == null ||
                    worldObject.def == null ||
                    !worldObject.def.expandingIcon ||
                    worldObject.HiddenBehindTerrainNow() ||
                    String.IsNullOrEmpty(worldObject.LabelCap))
                {
                    continue;
                }

                Rect iconRect =
                    ExpandableWorldObjectsUtility.ExpandedIconScreenRect(
                        worldObject);
                string label = worldObject.LabelCap;
                Text.Font = GameFont.Tiny;
                Vector2 size = Text.CalcSize(label);
                Rect labelRect = new Rect(
                    iconRect.center.x - size.x / 2f - 4f,
                    iconRect.yMax + 2f,
                    size.x + 8f,
                    12f);
                if (labelRect.xMax < 0f ||
                    labelRect.x > UI.screenWidth ||
                    labelRect.yMax < 0f ||
                    labelRect.y > UI.screenHeight)
                {
                    continue;
                }

                Color previousColor = GUI.color;
                TextAnchor previousAnchor = Text.Anchor;
                try
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(labelRect, TexUI.GrayTextBG);
                    GUI.color = ColorFor(worldObject.Faction, Faction.OfPlayer);
                    Text.Anchor = TextAnchor.UpperCenter;
                    Widgets.Label(
                        new Rect(
                            iconRect.center.x - size.x / 2f,
                            iconRect.yMax - 1f,
                            size.x,
                            999f),
                        label);
                }
                finally
                {
                    GUI.color = previousColor;
                    Text.Anchor = previousAnchor;
                }
            }
        }
#endif

        private static Color ColorFor(Faction faction, Faction colony)
        {
            if (faction == null)
            {
                return new Color(0.62f, 0.34f, 0.90f, 0.85f);
            }

#if RWT_LEGACY_FACTION_MAP_013
            if (faction == colony)
#else
            if (faction.IsPlayer)
#endif
            {
                return new Color(0.18f, 0.86f, 0.78f, 0.85f);
            }

            if (colony != null && colony.HostileTo(faction))
            {
                return new Color(0.93f, 0.22f, 0.22f, 0.85f);
            }

            if (colony != null && faction.GoodwillWith(colony) >= 75f)
            {
                return new Color(0.36f, 0.86f, 0.36f, 0.85f);
            }

            return new Color(0.96f, 0.82f, 0.24f, 0.85f);
        }
    }

#if RWT_LEGACY_FACTION_MAP
    [HarmonyPatch(typeof(WorldFactionsRenderer),
        nameof(WorldFactionsRenderer.DrawWorldFactions))]
    internal static class FactionLensLegacyFactionMapPatch
    {
        private static void Postfix()
        {
            FactionLensLegacyOverlay.DrawFactionLabels();
        }
    }
#endif

#if RWT_LEGACY_WORLD_OBJECTS
    [HarmonyPatch(typeof(ExpandableWorldObjectsUtility),
        nameof(ExpandableWorldObjectsUtility.ExpandableWorldObjectsOnGUI))]
    internal static class FactionLensLegacyWorldObjectPatch
    {
        private static void Postfix()
        {
            FactionLensLegacyOverlay.DrawWorldObjectLabels();
        }
    }
#endif
}
#endif
