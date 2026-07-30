using UnityEngine;

namespace FactionLens.Settings
{
    internal readonly struct FactionLensPalette
    {
        internal FactionLensPalette(
            Color hostile,
            Color neutral,
            Color allied,
            Color player,
            Color factionless,
            Color unknown)
        {
            Hostile = hostile;
            Neutral = neutral;
            Allied = allied;
            Player = player;
            Factionless = factionless;
            Unknown = unknown;
        }

        internal Color Hostile { get; }
        internal Color Neutral { get; }
        internal Color Allied { get; }
        internal Color Player { get; }
        internal Color Factionless { get; }
        internal Color Unknown { get; }

        internal static FactionLensPalette Default =>
            new FactionLensPalette(
                new Color(0.95f, 0.28f, 0.24f),
                new Color(0.95f, 0.78f, 0.24f),
                new Color(0.30f, 0.88f, 0.48f),
                new Color(0.30f, 0.72f, 1.00f),
                new Color(0.68f, 0.68f, 0.68f),
                new Color(0.78f, 0.58f, 0.92f));

        // Okabe-Ito-derived hues, adjusted slightly for dark map contrast.
        internal static FactionLensPalette Colorblind =>
            new FactionLensPalette(
                new Color(0.84f, 0.37f, 0.00f),
                new Color(0.94f, 0.89f, 0.26f),
                new Color(0.00f, 0.62f, 0.45f),
                new Color(0.00f, 0.45f, 0.70f),
                new Color(0.65f, 0.65f, 0.65f),
                new Color(0.80f, 0.47f, 0.65f));
    }
}
