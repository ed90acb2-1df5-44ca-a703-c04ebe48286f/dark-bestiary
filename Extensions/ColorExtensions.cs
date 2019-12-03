using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class ColorExtensions
    {
        public static Color With(
            this Color original,
            float? r = null,
            float? g = null,
            float? b = null,
            float? a = null)
        {
            return new Color(
                r ?? original.r,
                g ?? original.g,
                b ?? original.b,
                a ?? original.a
            );
        }

        public static Color Randomize(this Color original)
        {
            return new Color(RNG.Float(), RNG.Float(), RNG.Float());
        }
    }
}