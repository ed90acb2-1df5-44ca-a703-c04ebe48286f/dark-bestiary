using System;
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
        public float MusicVolume = 0.5f;
        public string Locale = I18N.GetDefaultLocale();
        public bool RunInBackground;
        public bool AlwaysShowHealth = true;
        public bool AlwaysShowSkills = true;
        public bool SummonedUnitsControlledByAi = true;
        public bool DisableErrorMessages;
        public bool DisplayFormulasInTooltips;
        public bool DisableCameraShake;
        public bool DisableCombatText;
        public bool HideBuffs;
        public bool HideSkills;
        public bool HideHealth;
        public bool HideHealthText;
    }
}