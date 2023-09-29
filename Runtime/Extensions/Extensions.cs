using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class Extensions
    {
        public static bool IsDefault<T>(this T value) where T : struct
        {
            return value.Equals(default(T));
        }

        public static RectOffset With(this RectOffset original, int? left = null, int? right = null, int? top = null, int? bottom = null)
        {
            return new RectOffset(left ?? original.left, right ?? original.right, top ?? original.top, bottom ?? original.bottom);
        }
    }
}