using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public static class KeyBindings
    {
        public static readonly Dictionary<KeyType, KeyCode> s_Default = new()
        {
            {KeyType.Move, KeyCode.F1},
            {KeyType.Stop, KeyCode.S},

            {KeyType.Skill1, KeyCode.Alpha1},
            {KeyType.Skill2, KeyCode.Alpha2},
            {KeyType.Skill3, KeyCode.Alpha3},
            {KeyType.Skill4, KeyCode.Alpha4},
            {KeyType.Skill5, KeyCode.Alpha5},
            {KeyType.Skill6, KeyCode.Q},
            {KeyType.Skill7, KeyCode.W},
            {KeyType.Skill8, KeyCode.E},
            {KeyType.Skill9, KeyCode.R},
            {KeyType.Skill10, KeyCode.T},
            {KeyType.SwapWeapon, KeyCode.Tab},
            {KeyType.ConsumablesBag, KeyCode.B},
            {KeyType.EndTurn, KeyCode.Space},

            {KeyType.Equipment, KeyCode.C},
            {KeyType.Reliquary, KeyCode.Y},
            {KeyType.Masteries, KeyCode.M},
            {KeyType.Attributes, KeyCode.A},
            {KeyType.Talents, KeyCode.N},
            {KeyType.Skills, KeyCode.P},
            {KeyType.Achievements, KeyCode.H},
            {KeyType.CombatLog, KeyCode.L},
            {KeyType.DamageMeter, KeyCode.D},
            {KeyType.Mailbox, KeyCode.X},
            {KeyType.Menu, KeyCode.O},
            {KeyType.DecreaseGameSpeed, KeyCode.Minus},
            {KeyType.IncreaseGameSpeed, KeyCode.Equals},
        };

        public static event Action<KeyType, KeyCode> KeyBindingChanged;
        public static event Action<Dictionary<KeyType, KeyCode>> KeyBindingsChanged;

        public static Dictionary<KeyType, KeyCode> All()
        {
            return SettingsManager.Instance.Data.KeyBindings;
        }

        public static void Reset()
        {
            SettingsManager.Instance.Data.KeyBindings = s_Default;
            KeyBindingsChanged?.Invoke(SettingsManager.Instance.Data.KeyBindings);
        }

        public static void Change(KeyType type, KeyCode code)
        {
            SettingsManager.Instance.Data.KeyBindings[type] = code;
            KeyBindingChanged?.Invoke(type, code);
        }

        public static void Change(Dictionary<KeyType, KeyCode> keyBindings)
        {
            foreach (var keyBinding in keyBindings)
            {
                SettingsManager.Instance.Data.KeyBindings[keyBinding.Key] = keyBinding.Value;
            }

            KeyBindingsChanged?.Invoke(keyBindings);
        }

        public static KeyCode Get(KeyType type)
        {
            if (!SettingsManager.Instance.Data.KeyBindings.ContainsKey(type))
            {
                SettingsManager.Instance.Data.KeyBindings.Add(type, s_Default[type]);
                KeyBindingChanged?.Invoke(type, s_Default[type]);
            }

            return SettingsManager.Instance.Data.KeyBindings[type];
        }

        public static List<KeyCode> Skills()
        {
            var settings = SettingsManager.Instance;

            return new List<KeyCode>
            {
                settings.Data.KeyBindings[KeyType.Skill1],
                settings.Data.KeyBindings[KeyType.Skill2],
                settings.Data.KeyBindings[KeyType.Skill3],
                settings.Data.KeyBindings[KeyType.Skill4],
                settings.Data.KeyBindings[KeyType.Skill5],
                settings.Data.KeyBindings[KeyType.Skill6],
                settings.Data.KeyBindings[KeyType.Skill7],
                settings.Data.KeyBindings[KeyType.Skill8],
                settings.Data.KeyBindings[KeyType.Skill9],
                settings.Data.KeyBindings[KeyType.Skill10],
            };
        }
    }
}