using System.Collections.Generic;
using UnityEngine;

namespace DarkBestiary
{
    public static class KeyCodes
    {
        public static readonly List<KeyCode> Hotkeys = new List<KeyCode>
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Q,
            KeyCode.W,
            KeyCode.E,
            KeyCode.R,
            KeyCode.T,
        };

        private static readonly Dictionary<KeyCode, string> KeyCodeLabels = new Dictionary<KeyCode, string>
        {
            {KeyCode.Alpha0, "0"},
            {KeyCode.Alpha1, "1"},
            {KeyCode.Alpha2, "2"},
            {KeyCode.Alpha3, "3"},
            {KeyCode.Alpha4, "4"},
            {KeyCode.Alpha5, "5"},
            {KeyCode.Alpha6, "6"},
            {KeyCode.Alpha7, "7"},
            {KeyCode.Alpha8, "8"},
            {KeyCode.Alpha9, "9"},
            {KeyCode.Escape, "Esc"},
        };

        public static string GetLabel(KeyCode code)
        {
            return KeyCodeLabels.ContainsKey(code) ? KeyCodeLabels[code] : code.ToString();
        }
    }
}