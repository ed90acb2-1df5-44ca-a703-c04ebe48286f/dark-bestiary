using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class SettingsViewController : ViewController<ISettingsView>
    {
        private readonly SettingsManager settings;

        public SettingsViewController(ISettingsView view, SettingsManager settings) : base(view)
        {
            this.settings = settings;
        }

        protected override void OnInitialize()
        {
            View.DisplayModeChanging += OnDisplayModeChanging;
            View.ToggleVerticalSync += OnToggleVerticalSync;
            View.ToggleRunInBackground += OnToggleRunInBackground;
            View.ToggleAlwaysShowEnemyHealth += OnToggleAlwaysShowEnemyHealth;
            View.ToggleAlwaysShowEnemySkills += OnToggleAlwaysShowEnemySkills;
            View.ToggleDisplayFormulasInTooltips += OnToggleDisplayFormulasInTooltips;
            View.ToggleDisableErrorMessages += OnToggleDisableErrorMessages;
            View.ToggleDisableCameraShake += OnToggleDisableCameraShake;
            View.ToggleDisableCombatText += OnToggleDisableCombatText;
            View.ToggleSummonedUnitsControlledByAi += OnToggleSummonedUnitsControlledByAi;
            View.ToggleHideBuffs += OnToggleHideBuffs;
            View.ToggleHideSkills += OnToggleHideSkills;
            View.ToggleHideHealth += OnToggleHideHealth;
            View.ToggleHideHealthText += OnToggleHideHealthText;
            View.LanguageChanging += OnLanguageChanging;
            View.MasterVolumeChanging += OnMasterVolumeChanging;
            View.MusicVolumeChanging += OnMusicVolumeChanging;
            View.ResolutionChanging += OnResolutionChanging;
            View.SoundVolumeChanging += OnSoundVolumeChanging;
            View.Resetting += OnResetting;
            View.Showing += OnShowing;
            View.Refresh(this.settings);
        }

        protected override void OnTerminate()
        {
            View.DisplayModeChanging -= OnDisplayModeChanging;
            View.ToggleVerticalSync -= OnToggleVerticalSync;
            View.ToggleRunInBackground -= OnToggleRunInBackground;
            View.ToggleAlwaysShowEnemyHealth -= OnToggleAlwaysShowEnemyHealth;
            View.ToggleAlwaysShowEnemySkills -= OnToggleAlwaysShowEnemySkills;
            View.ToggleDisplayFormulasInTooltips -= OnToggleDisplayFormulasInTooltips;
            View.ToggleDisableErrorMessages -= OnToggleDisableErrorMessages;
            View.ToggleDisableCameraShake -= OnToggleDisableCameraShake;
            View.ToggleSummonedUnitsControlledByAi -= OnToggleSummonedUnitsControlledByAi;
            View.ToggleHideBuffs -= OnToggleHideBuffs;
            View.ToggleHideSkills -= OnToggleHideSkills;
            View.ToggleHideHealth -= OnToggleHideHealth;
            View.ToggleHideHealthText -= OnToggleHideHealthText;
            View.LanguageChanging -= OnLanguageChanging;
            View.MasterVolumeChanging -= OnMasterVolumeChanging;
            View.MusicVolumeChanging -= OnMusicVolumeChanging;
            View.ResolutionChanging -= OnResolutionChanging;
            View.SoundVolumeChanging -= OnSoundVolumeChanging;
            View.Resetting -= OnResetting;
            View.Showing -= OnShowing;
            View.Hidding -= OnHidding;
        }

        private void OnToggleHideHealthText(bool value)
        {
            this.settings.SetHideHealthText(value);
        }

        private void OnToggleHideHealth(bool value)
        {
            this.settings.SetHideHealth(value);
        }

        private void OnToggleHideSkills(bool value)
        {
            this.settings.SetHideSkills(value);
        }

        private void OnToggleHideBuffs(bool value)
        {
            this.settings.SetHideBuffs(value);
        }

        private void OnToggleDisableErrorMessages(bool value)
        {
            this.settings.SetDisableErrorMessages(value);
        }

        private void OnToggleSummonedUnitsControlledByAi(bool value)
        {
            this.settings.SetToggleSummonedUnitsControlledByAi(value);
        }

        private void OnToggleDisableCombatText(bool value)
        {
            this.settings.SetDisableCombatText(value);
        }

        private void OnToggleDisableCameraShake(bool value)
        {
            this.settings.SetDisableCameraShake(value);
        }

        private void OnToggleDisplayFormulasInTooltips(bool value)
        {
            this.settings.SetDisplayFormulasInTooltips(value);
        }

        private void OnToggleAlwaysShowEnemyHealth(bool value)
        {
            this.settings.SetAlwaysShowHealth(value);
        }

        private void OnToggleAlwaysShowEnemySkills(bool value)
        {
            this.settings.SetAlwaysShowSkills(value);
        }

        private void OnToggleVerticalSync(bool value)
        {
            this.settings.SetVerticalSync(value ? 1 : 0);
        }

        private void OnToggleRunInBackground(bool value)
        {
            this.settings.SetRunInBackground(value);
        }

        private void OnResolutionChanging(int index)
        {
            this.settings.SetResolution(index);
        }

        private void OnDisplayModeChanging(int index)
        {
            this.settings.SetDisplayMode(index);
        }

        private void OnLanguageChanging(int index)
        {
            this.settings.SetLocale(I18N.Instance.Dictionaries.Keys.ToArray()[index]);
        }

        private void OnMasterVolumeChanging(float volume)
        {
            this.settings.SetMasterVolume(volume);
        }

        private void OnMusicVolumeChanging(float volume)
        {
            this.settings.SetMusicVolume(volume);
        }

        private void OnSoundVolumeChanging(float volume)
        {
            this.settings.SetSoundVolume(volume);
        }

        private void OnResetting()
        {
            this.settings.Reset();
            View.Refresh(this.settings);
        }

        private void OnShowing()
        {
            View.Hidding += OnHidding;
        }

        private void OnHidding()
        {
            View.Hidding -= OnHidding;
            View.Hide();
        }
    }
}