using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SettingsViewController : ViewController<ISettingsView>
    {
        private readonly SettingsManager m_Settings;

        public SettingsViewController(ISettingsView view, SettingsManager settings) : base(view)
        {
            m_Settings = settings;
        }

        protected override void OnInitialize()
        {
            View.DisplayModeChanging += OnDisplayModeChanging;
            View.ToggleLoopMusic += OnToggleLoopMusic;
            View.ToggleHideActingUnitHealth += OnToggleHideActingUnitHealth;
            View.ToggleDisableUiSounds += OnToggleDisableUiSounds;
            View.ToggleVerticalSync += OnToggleVerticalSync;
            View.ToggleRunInBackground += OnToggleRunInBackground;
            View.ToggleAlwaysShowEnemyHealth += OnToggleAlwaysShowEnemyHealth;
            View.ToggleAlwaysShowEnemySkills += OnToggleAlwaysShowEnemySkills;
            View.ToggleDisplayFormulasInTooltips += OnToggleDisplayFormulasInTooltips;
            View.ToggleDisableErrorMessages += OnToggleDisableErrorMessages;
            View.ToggleSummonedUnitsControlledByAi += OnToggleSummonedUnitsControlledByAi;
            View.ToggleDisableCombatText += OnToggleDisableCombatText;
            View.ToggleHideBuffs += OnToggleHideBuffs;
            View.ToggleHideSkills += OnToggleHideSkills;
            View.ToggleHideHealth += OnToggleHideHealth;
            View.ToggleHideHealthText += OnToggleHideHealthText;
            View.ToggleHighContrastMode += OnToggleHighContrastMode;
            View.LanguageChanging += OnLanguageChanging;
            View.MasterVolumeChanging += OnMasterVolumeChanging;
            View.MusicVolumeChanging += OnMusicVolumeChanging;
            View.ResolutionChanging += OnResolutionChanging;
            View.SoundVolumeChanging += OnSoundVolumeChanging;
            View.Resetting += OnResetting;

            View.Construct(m_Settings);
        }

        protected override void OnTerminate()
        {
            View.DisplayModeChanging -= OnDisplayModeChanging;
            View.ToggleLoopMusic -= OnToggleLoopMusic;
            View.ToggleHideActingUnitHealth -= OnToggleHideActingUnitHealth;
            View.ToggleVerticalSync -= OnToggleVerticalSync;
            View.ToggleRunInBackground -= OnToggleRunInBackground;
            View.ToggleAlwaysShowEnemyHealth -= OnToggleAlwaysShowEnemyHealth;
            View.ToggleAlwaysShowEnemySkills -= OnToggleAlwaysShowEnemySkills;
            View.ToggleDisplayFormulasInTooltips -= OnToggleDisplayFormulasInTooltips;
            View.ToggleDisableErrorMessages -= OnToggleDisableErrorMessages;
            View.ToggleSummonedUnitsControlledByAi -= OnToggleSummonedUnitsControlledByAi;
            View.ToggleHideBuffs -= OnToggleHideBuffs;
            View.ToggleHideSkills -= OnToggleHideSkills;
            View.ToggleHideHealth -= OnToggleHideHealth;
            View.ToggleHideHealthText -= OnToggleHideHealthText;
            View.ToggleHighContrastMode -= OnToggleHighContrastMode;
            View.LanguageChanging -= OnLanguageChanging;
            View.MasterVolumeChanging -= OnMasterVolumeChanging;
            View.MusicVolumeChanging -= OnMusicVolumeChanging;
            View.ResolutionChanging -= OnResolutionChanging;
            View.SoundVolumeChanging -= OnSoundVolumeChanging;
            View.Resetting -= OnResetting;
        }

        private void OnToggleHideHealthText(bool value)
        {
            m_Settings.SetHideHealthText(value);
        }

        private void OnToggleHighContrastMode(bool value)
        {
            m_Settings.SetHighContrastMode(value);
        }

        private void OnToggleHideHealth(bool value)
        {
            m_Settings.SetHideHealth(value);
        }

        private void OnToggleHideSkills(bool value)
        {
            m_Settings.SetHideSkills(value);
        }

        private void OnToggleHideBuffs(bool value)
        {
            m_Settings.SetHideBuffs(value);
        }

        private void OnToggleDisableErrorMessages(bool value)
        {
            m_Settings.SetDisableErrorMessages(value);
        }

        private void OnToggleSummonedUnitsControlledByAi(bool value)
        {
            m_Settings.SetToggleSummonedUnitsControlledByAi(value);
        }

        private void OnToggleDisableCombatText(bool value)
        {
            m_Settings.SetDisableCombatText(value);
        }

        private void OnToggleDisplayFormulasInTooltips(bool value)
        {
            m_Settings.SetDisplayFormulasInTooltips(value);
        }

        private void OnToggleAlwaysShowEnemyHealth(bool value)
        {
            m_Settings.SetAlwaysShowHealth(value);
        }

        private void OnToggleAlwaysShowEnemySkills(bool value)
        {
            m_Settings.SetAlwaysShowSkills(value);
        }

        private void OnToggleDisableUiSounds(bool value)
        {
            m_Settings.SetDisableUiSounds(value);
        }

        private void OnToggleHideActingUnitHealth(bool value)
        {
            m_Settings.SetHideActingUnitHealth(value);
        }

        private void OnToggleLoopMusic(bool value)
        {
            m_Settings.SetLoopMusic(value);
        }

        private void OnToggleVerticalSync(bool value)
        {
            m_Settings.SetVerticalSync(value ? 1 : 0);
        }

        private void OnToggleRunInBackground(bool value)
        {
            m_Settings.SetRunInBackground(value);
        }

        private void OnResolutionChanging(int index)
        {
            m_Settings.SetResolution(index);
        }

        private void OnDisplayModeChanging(int index)
        {
            m_Settings.SetDisplayMode(index);
        }

        private void OnLanguageChanging(int index)
        {
            m_Settings.SetLocale(I18N.Instance.Dictionaries.Keys.ToArray()[index]);
        }

        private void OnMasterVolumeChanging(float volume)
        {
            m_Settings.SetMasterVolume(volume);
        }

        private void OnMusicVolumeChanging(float volume)
        {
            m_Settings.SetMusicVolume(volume);
        }

        private void OnSoundVolumeChanging(float volume)
        {
            m_Settings.SetSoundVolume(volume);
        }

        private void OnResetting()
        {
            m_Settings.Reset();
            View.Refresh(m_Settings);
        }
    }
}