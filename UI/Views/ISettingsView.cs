using DarkBestiary.Managers;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ISettingsView : IView, IHideOnEscape
    {
        event Payload<int> ResolutionChanging;
        event Payload<int> DisplayModeChanging;
        event Payload<int> LanguageChanging;
        event Payload<bool> ToggleLoopMusic;
        event Payload<bool> ToggleHideActingUnitHealth;
        event Payload<bool> ToggleDisableUiSounds;
        event Payload<bool> ToggleVerticalSync;
        event Payload<bool> ToggleRunInBackground;
        event Payload<bool> ToggleAlwaysShowEnemyHealth;
        event Payload<bool> ToggleAlwaysShowEnemySkills;
        event Payload<bool> ToggleDisplayFormulasInTooltips;
        event Payload<bool> ToggleDisableErrorMessages;
        event Payload<bool> ToggleDisableCameraShake;
        event Payload<bool> ToggleDisableCombatText;
        event Payload<bool> ToggleSummonedUnitsControlledByAi;
        event Payload<bool> ToggleHideHealthText;
        event Payload<bool> ToggleHideHealth;
        event Payload<bool> ToggleHideBuffs;
        event Payload<bool> ToggleHideSkills;
        event Payload<bool> ToggleHighContrastMode;
        event Payload<float> MasterVolumeChanging;
        event Payload<float> MusicVolumeChanging;
        event Payload<float> SoundVolumeChanging;
        event Payload Resetting;

        void Construct(SettingsManager settings);
        void Refresh(SettingsManager settings);
    }
}