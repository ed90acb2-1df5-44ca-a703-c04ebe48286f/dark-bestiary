using System.Collections.Generic;

namespace DarkBestiary
{
    public static class StringUtils
    {
        private static readonly Dictionary<int, string> s_RomanNumerals = new()
        {
            {0, "0"},
            {1, "I"},
            {2, "II"},
            {3, "III"},
            {4, "IV"},
            {5, "V"},
            {6, "VI"},
            {7, "VII"},
            {8, "VIII"},
            {9, "IX"},
            {10, "X"},
            {11, "XI"},
            {12, "XII"},
        };

        public static string ToRomanNumeral(int number)
        {
            return s_RomanNumerals[number];
        }
    }
}