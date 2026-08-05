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

        // Nameplates are roughly 18px tall, so four pixels reads as rounded
        // without turning a short label into a lozenge.
        private const float CornerRadius = 4f;
        private const int CornerTextureSize = 16;

        // One quarter disc, drawn four times with flipped texture coordinates.
        // Authored as the top-left corner: opaque toward the bottom right.
        private static readonly Rect TopLeftCoords = new Rect(0f, 0f, 1f, 1f);
        private static readonly Rect TopRightCoords = new Rect(1f, 0f, -1f, 1f);
        private static readonly Rect BottomLeftCoords = new Rect(0f, 1f, 1f, -1f);
        private static readonly Rect BottomRightCoords = new Rect(1f, 1f, -1f, -1f);

        private static Texture2D cornerTexture;

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
            FactionLensSettings settings,
            bool hovered = false)
        {
            Color previousColor = GUI.color;

            if (settings.ShowBackground)
            {
                Color background = BackgroundColor;
                background.a *= color.a;
                DrawNameplate(rect, background, settings.RoundedNameplates);
            }
            if (hovered)
            {
                Widgets.DrawHighlight(rect);
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

        /// <summary>
        /// Fills <paramref name="rect"/> with <paramref name="color"/>, either as
        /// a plain box or as three solid bands plus four anti-aliased corner
        /// quads. The banded form is exact everywhere except the corners, so only
        /// the corners pay for texture sampling.
        /// </summary>
        private static void DrawNameplate(Rect rect, Color color, bool rounded)
        {
            float radius = Mathf.Min(
                CornerRadius,
                Mathf.Min(rect.width, rect.height) * 0.5f);

            if (!rounded || radius < 1f)
            {
                Widgets.DrawBoxSolid(rect, color);
                return;
            }

            Widgets.DrawBoxSolid(
                new Rect(rect.x, rect.y + radius, rect.width, rect.height - radius * 2f),
                color);
            Widgets.DrawBoxSolid(
                new Rect(rect.x + radius, rect.y, rect.width - radius * 2f, radius),
                color);
            Widgets.DrawBoxSolid(
                new Rect(rect.x + radius, rect.yMax - radius, rect.width - radius * 2f, radius),
                color);

            Texture2D corner = CornerTexture();
            if (corner == null)
            {
                return;
            }

            Color previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTextureWithTexCoords(
                new Rect(rect.x, rect.y, radius, radius), corner, TopLeftCoords);
            GUI.DrawTextureWithTexCoords(
                new Rect(rect.xMax - radius, rect.y, radius, radius), corner, TopRightCoords);
            GUI.DrawTextureWithTexCoords(
                new Rect(rect.x, rect.yMax - radius, radius, radius), corner, BottomLeftCoords);
            GUI.DrawTextureWithTexCoords(
                new Rect(rect.xMax - radius, rect.yMax - radius, radius, radius), corner, BottomRightCoords);
            GUI.color = previousColor;
        }

        /// <summary>
        /// Builds the quarter-disc mask once, on first draw, so no texture is
        /// allocated for players who leave rounded corners off.
        /// </summary>
        private static Texture2D CornerTexture()
        {
            if (cornerTexture != null)
            {
                return cornerTexture;
            }

            const int size = CornerTextureSize;
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false)
            {
                name = "FactionLens_NameplateCorner",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color[size * size];
            for (int screenY = 0; screenY < size; screenY++)
            {
                for (int x = 0; x < size; x++)
                {
                    // The arc centre sits at the cell's bottom-right in screen
                    // space, so the transparent wedge lands at the top left.
                    float dx = size - (x + 0.5f);
                    float dy = size - (screenY + 0.5f);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(size - distance + 0.5f);

                    // Texture row 0 is the bottom of the image; screen row 0 is
                    // the top.
                    int textureY = size - 1 - screenY;
                    pixels[textureY * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            cornerTexture = texture;
            return cornerTexture;
        }

        private static Rect Offset(Rect rect, float x, float y)
        {
            rect.x += x;
            rect.y += y;
            return rect;
        }
    }
}
