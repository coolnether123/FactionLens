using FactionLens.Settings;
using UnityEngine;
using Verse;

namespace FactionLens.Presentation
{
    internal static class LabelDrawer
    {
        private static readonly Color BackgroundColor =
            new Color(0.04f, 0.04f, 0.04f, 0.78f);
        private static readonly Color OutlineColor =
            new Color(0.02f, 0.02f, 0.02f, 0.95f);

        internal static Vector2 Measure(string label)
        {
            Vector2 textSize = Text.CalcSize(label);
            return new Vector2(
                Mathf.Ceil(textSize.x) + 8f,
                Mathf.Max(18f, Mathf.Ceil(textSize.y) + 4f));
        }

        internal static void Draw(
            Rect rect,
            string label,
            Color color,
            FactionLensSettings settings)
        {
            Color previousColor = GUI.color;

            if (settings.ShowBackground)
            {
                Color background = BackgroundColor;
                background.a *= color.a;
                Widgets.DrawBoxSolid(rect, background);
            }

            Rect textRect = rect.ContractedBy(4f, 1f);
            if (settings.ShowOutline)
            {
                Color outline = OutlineColor;
                outline.a *= color.a;
                GUI.color = outline;
                Widgets.Label(Offset(textRect, -1f, 0f), label);
                Widgets.Label(Offset(textRect, 1f, 0f), label);
                Widgets.Label(Offset(textRect, 0f, -1f), label);
                Widgets.Label(Offset(textRect, 0f, 1f), label);
            }

            GUI.color = color;
            Widgets.Label(textRect, label);
            GUI.color = previousColor;
        }

        private static Rect Offset(Rect rect, float x, float y)
        {
            rect.x += x;
            rect.y += y;
            return rect;
        }
    }
}
