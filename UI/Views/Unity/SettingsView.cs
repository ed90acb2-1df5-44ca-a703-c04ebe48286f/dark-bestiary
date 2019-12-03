using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class SettingsView : View, ISettingsView
    {
        public event Payload<int> ResolutionChanging;
        public event Payload<int> DisplayModeChanging;
        public event Payload<int> LanguageChanging;
        public event Payload<bool> ToggleVerticalSync;
        public event Payload<bool> ToggleRunInBackground;
        public event Payload<bool> ToggleAlwaysShowEnemyHealth;
        public event Payload<bool> ToggleAlwaysShowEnemySkills;
        public event Payload<bool> ToggleDisplayFormulasInTooltips;
        public event Payload<bool> ToggleDisableErrorMessages;
        public event Payload<bool> ToggleDisableCameraShake;
        public event Payload<bool> ToggleDisableCombatText;
        public event Payload<bool> ToggleHideHealthText;
        public event Payload<bool> ToggleHideHealth;
        public event Payload<bool> ToggleHideBuffs;
        public event Payload<bool> ToggleHideSkills;
        public event Payload<bool> ToggleSummonedUnitsControlledByAi;
        public event Payload<float> MasterVolumeChanging;
        public event Payload<float> MusicVolumeChanging;
        public event Payload<float> SoundVolumeChanging;
        public event Payload Resetting;

        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fullscreenModeDropdown;
        [SerializeField] private TMP_Dropdown languageDropdown;
        [SerializeField] private Toggle verticalSyncToggle;
        [SerializeField] private Toggle runInBackgroundToggle;
        [SerializeField] private Toggle alwaysShowEnemyHealthToggle;
        [SerializeField] private Toggle alwaysShowEnemySkillsToggle;
        [SerializeField] private Toggle displayFormulasInTooltipsToggle;
        [SerializeField] private Toggle disableErrorMessagesToggle;
        [SerializeField] private Toggle disableCameraShakeToggle;
        [SerializeField] private Toggle disableCombatTextToggle;
        [SerializeField] private Toggle summonedUnitsControlledByAiToggle;
        [SerializeField] private Toggle hideHealthTextToggle;
        [SerializeField] private Toggle hideHealthToggle;
        [SerializeField] private Toggle hideBuffsToggle;
        [SerializeField] private Toggle hideSkillsToggle;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider soundVolumeSlider;
        [SerializeField] private Interactable okayButton;
        [SerializeField] private Interactable resetButton;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Transform fullscreenModeDropdownContainer;
        [SerializeField] private Transform resolutionDropdownContainer;

        public void Refresh(SettingsManager settings)
        {
            if (settings.GetResolutions().Count > 0)
            {
                InitializeResolutionsDropdown(settings.GetResolutions(), settings.ResolutionIndex);
                InitializeFullscreenModeDropdown(settings.GetDisplayModes(), settings.DisplayModeIndex);
            }
            else
            {
                this.resolutionDropdownContainer.gameObject.SetActive(false);
                this.fullscreenModeDropdownContainer.gameObject.SetActive(false);
            }

            InitializeLanguagesDropdown(I18N.Instance.Dictionaries, settings.Locale);

            this.masterVolumeSlider.value = settings.MasterVolume;
            this.masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

            this.musicVolumeSlider.value = settings.MusicVolume;
            this.musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            this.soundVolumeSlider.value = settings.SoundVolume;
            this.soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);

            this.verticalSyncToggle.isOn = settings.VerticalSync > 0;
            this.verticalSyncToggle.onValueChanged.AddListener(OnVerticalSyncChanged);

            this.runInBackgroundToggle.isOn = settings.RunInBackground;
            this.runInBackgroundToggle.onValueChanged.AddListener(OnRunInBackgroundChanged);

            this.alwaysShowEnemyHealthToggle.isOn = settings.AlwaysShowHealth;
            this.alwaysShowEnemyHealthToggle.onValueChanged.AddListener(OnAlwaysShowEnemyHealthChanged);

            this.alwaysShowEnemySkillsToggle.isOn = settings.AlwaysShowSkills;
            this.alwaysShowEnemySkillsToggle.onValueChanged.AddListener(OnAlwaysShowEnemySkillsChanged);

            this.displayFormulasInTooltipsToggle.isOn = settings.DisplayFormulasInTooltips;
            this.displayFormulasInTooltipsToggle.onValueChanged.AddListener(OnDisplayFormulasInTooltipsChanged);

            this.disableErrorMessagesToggle.isOn = settings.DisableErrorMessages;
            this.disableErrorMessagesToggle.onValueChanged.AddListener(OnDisableErrorMessagesChanged);

            this.disableCameraShakeToggle.isOn = settings.DisableCameraShake;
            this.disableCameraShakeToggle.onValueChanged.AddListener(OnDisableCameraShakeChanged);

            this.disableCombatTextToggle.isOn = settings.DisableCombatText;
            this.disableCombatTextToggle.onValueChanged.AddListener(OnDisableCombatTextChanged);

            this.hideBuffsToggle.isOn = settings.HideBuffs;
            this.hideBuffsToggle.onValueChanged.AddListener(OnHideBuffsChanged);

            this.hideSkillsToggle.isOn = settings.HideSkills;
            this.hideSkillsToggle.onValueChanged.AddListener(OnHideSkillsChanged);

            this.hideHealthToggle.isOn = settings.HideHealth;
            this.hideHealthToggle.onValueChanged.AddListener(OnHideHealthChanged);

            this.hideHealthTextToggle.isOn = settings.HideHealthText;
            this.hideHealthTextToggle.onValueChanged.AddListener(OnHideHealthTextChanged);

            this.summonedUnitsControlledByAiToggle.isOn = settings.SummonedUnitsControlledByAi;
            this.summonedUnitsControlledByAiToggle.onValueChanged.AddListener(OnSummonedUnitsControlledByAiChanged);
        }

        protected override void OnInitialize()
        {
            this.resetButton.PointerUp += OnResetButtonPointerUp;
            this.closeButton.PointerUp += OnCloseButtonPointerUp;
            this.okayButton.PointerUp += OnCloseButtonPointerUp;
        }

        private void InitializeFullscreenModeDropdown(IEnumerable<FullScreenMode> modes, int value)
        {
            InitializeDropdown(
                this.fullscreenModeDropdown,
                modes.Select(mode => (string) EnumTranslator.Translate(mode)).ToList(), value, OnFullscreenModeChanged
            );
        }

        private void InitializeResolutionsDropdown(IEnumerable<Resolution> resolutions, int value)
        {
            InitializeDropdown(
                this.resolutionDropdown,
                resolutions.Select(resolution => resolution.ToString()).ToList(), value, OnResolutionChanged
            );
        }

        private void InitializeLanguagesDropdown(Dictionary<string, I18NDictionaryInfo> dictionaries, string locale)
        {
            InitializeDropdown(
                this.languageDropdown,
                dictionaries.Values.Select(d => d.DisplayName).ToList(),
                dictionaries.Keys.ToList().IndexOf(locale),
                OnLanguageChanged
            );
        }

        private static void InitializeDropdown(TMP_Dropdown dropdown, List<string> options,
            int value, UnityAction<int> onChange)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.RefreshShownValue();
            dropdown.value = value;
            dropdown.onValueChanged.AddListener(onChange);
        }

        private void OnHideHealthTextChanged(bool value)
        {
            ToggleHideHealthText?.Invoke(value);
        }

        private void OnHideHealthChanged(bool value)
        {
            ToggleHideHealth?.Invoke(value);
        }

        private void OnHideSkillsChanged(bool value)
        {
            ToggleHideSkills?.Invoke(value);
        }

        private void OnHideBuffsChanged(bool value)
        {
            ToggleHideBuffs?.Invoke(value);
        }

        private void OnDisableCombatTextChanged(bool value)
        {
            ToggleDisableCombatText?.Invoke(value);
        }

        private void OnDisableCameraShakeChanged(bool value)
        {
            ToggleDisableCameraShake?.Invoke(value);
        }

        private void OnSummonedUnitsControlledByAiChanged(bool value)
        {
            ToggleSummonedUnitsControlledByAi?.Invoke(value);
        }

        private void OnDisableErrorMessagesChanged(bool value)
        {
            ToggleDisableErrorMessages?.Invoke(value);
        }

        private void OnDisplayFormulasInTooltipsChanged(bool value)
        {
            ToggleDisplayFormulasInTooltips?.Invoke(value);
        }

        private void OnAlwaysShowEnemySkillsChanged(bool value)
        {
            ToggleAlwaysShowEnemySkills?.Invoke(value);
        }

        private void OnAlwaysShowEnemyHealthChanged(bool value)
        {
            ToggleAlwaysShowEnemyHealth?.Invoke(value);
        }

        private void OnVerticalSyncChanged(bool value)
        {
            ToggleVerticalSync?.Invoke(value);
        }

        private void OnRunInBackgroundChanged(bool value)
        {
            ToggleRunInBackground?.Invoke(value);
        }

        private void OnResetButtonPointerUp()
        {
            Resetting?.Invoke();
        }

        private void OnCloseButtonPointerUp()
        {
            Hide();
        }

        public void OnFullscreenModeChanged(int index)
        {
            DisplayModeChanging?.Invoke(index);
        }

        public void OnResolutionChanged(int index)
        {
            ResolutionChanging?.Invoke(index);
        }

        public void OnLanguageChanged(int index)
        {
            LanguageChanging?.Invoke(index);
        }

        public void OnMasterVolumeChanged(float volume)
        {
            MasterVolumeChanging?.Invoke(volume);
        }

        public void OnMusicVolumeChanged(float volume)
        {
            MusicVolumeChanging?.Invoke(volume);
        }

        public void OnSoundVolumeChanged(float volume)
        {
            SoundVolumeChanging?.Invoke(volume);
        }
    }
}