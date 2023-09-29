using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SettingsData
    {
        public int ResolutionIndex = Screen.resolutions.ToList().Count - 1;
        public int DisplayModeIndex;
        public int VerticalSync = 1;
        public float MasterVolume = 0.5f;
        public float SoundVolume = 1.0f;
        public float MusicVolume = 0.15f;
        public string Locale = I18N.GetDefaultLocale();
        public bool LoopMusic = true;
        public bool DisableUiSounds;
        public bool RunInBackground;
        public bool HideActingUnitHealth = true;
        public bool AlwaysShowHealth = true;
        public bool AlwaysShowSkills = true;
        public bool SummonedUnitsControlledByAi = true;
        public bool DisableErrorMessages;
        public bool DisplayFormulasInTooltips;
        public bool DisableCombatText;
        public bool HideBuffs;
        public bool HideSkills;
        public bool HideHealth;
        public bool HideHealthText;
        public bool HighContrastMode;
        public bool DoNotShowTowerConfirmation;
        public Dictionary<KeyType, KeyCode> KeyBindings = Managers.KeyBindings.s_Default;
    }
}