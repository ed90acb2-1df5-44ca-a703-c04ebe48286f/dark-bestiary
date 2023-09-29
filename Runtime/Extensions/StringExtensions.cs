using System;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string query)
        {
            return source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static float CalculateReadTime(this string text)
        {
            return Mathf.Max(3, text.Length * 0.1f);
        }

        public static Color ToColor(this string text)
        {
            if (!ColorUtility.TryParseHtmlString(text, out var color))
            {
                color = Color.white;
            }

            return color;
        }
    }
}