using System;
using DarkBestiary.Managers;

namespace DarkBestiary.UI.Views
{
    public interface ISettingsView : IView, IHideOnEscape
    {
        event Action<int> ResolutionChanging;
        event Action<int> DisplayModeChanging;
        event Action<int> LanguageChanging;
        event Action<bool> ToggleLoopMusic;
        event Action<bool> ToggleHideActingUnitHealth;
        event Action<bool> ToggleDisableUiSounds;
        event Action<bool> ToggleVerticalSync;
        event Action<bool> ToggleRunInBackground;
        event Action<bool> ToggleAlwaysShowEnemyHealth;
        event Action<bool> ToggleAlwaysShowEnemySkills;
        event Action<bool> ToggleDisplayFormulasInTooltips;
        event Action<bool> ToggleDisableErrorMessages;
        event Action<bool> ToggleDisableCombatText;
        event Action<bool> ToggleSummonedUnitsControlledByAi;
        event Action<bool> ToggleHideHealthText;
        event Action<bool> ToggleHideHealth;
        event Action<bool> ToggleHideBuffs;
        event Action<bool> ToggleHideSkills;
        event Action<bool> ToggleHighContrastMode;
        event Action<float> MasterVolumeChanging;
        event Action<float> MusicVolumeChanging;
        event Action<float> SoundVolumeChanging;
        event Action Resetting;

        void Construct(SettingsManager settings);
        void Refresh(SettingsManager settings);
    }
}