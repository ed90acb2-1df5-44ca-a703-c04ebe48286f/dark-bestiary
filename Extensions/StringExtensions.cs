using System.Text.RegularExpressions;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class StringExtensions
    {
        public static bool LikeIgnoreCase(this string toSearch, string toFind)
        {
            return toSearch.ToLower().Like(toFind.ToLower());
        }

        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(
                @"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
                        .Replace(toFind, ch => @"\" + ch)
                        .Replace('_', '.')
                        .Replace("%", ".*") + @"\z",
                RegexOptions.Singleline).IsMatch(toSearch);
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