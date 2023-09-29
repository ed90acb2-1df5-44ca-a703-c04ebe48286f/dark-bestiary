using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
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
        public event Action<int> ResolutionChanging;
        public event Action<int> DisplayModeChanging;
        public event Action<int> LanguageChanging;
        public event Action<bool> ToggleHideActingUnitHealth;
        public event Action<bool> ToggleLoopMusic;
        public event Action<bool> ToggleDisableUiSounds;
        public event Action<bool> ToggleVerticalSync;
        public event Action<bool> ToggleRunInBackground;
        public event Action<bool> ToggleAlwaysShowEnemyHealth;
        public event Action<bool> ToggleAlwaysShowEnemySkills;
        public event Action<bool> ToggleDisplayFormulasInTooltips;
        public event Action<bool> ToggleDisableErrorMessages;
        public event Action<bool> ToggleDisableCombatText;
        public event Action<bool> ToggleHideHealthText;
        public event Action<bool> ToggleHideHealth;
        public event Action<bool> ToggleHideBuffs;
        public event Action<bool> ToggleHideSkills;
        public event Action<bool> ToggleHighContrastMode;
        public event Action<bool> ToggleSummonedUnitsControlledByAi;
        public event Action<float> MasterVolumeChanging;
        public event Action<float> MusicVolumeChanging;
        public event Action<float> SoundVolumeChanging;
        public event Action Resetting;

        [SerializeField] private TMP_Dropdown m_ResolutionDropdown;
        [SerializeField] private TMP_Dropdown m_FullscreenModeDropdown;
        [SerializeField] private TMP_Dropdown m_LanguageDropdown;
        [SerializeField] private Toggle m_LoopMusicToggle;
        [SerializeField] private Toggle m_HideActingUnitHealthToggle;
        [SerializeField] private Toggle m_DisableUiSoundsToggle;
        [SerializeField] private Toggle m_VerticalSyncToggle;
        [SerializeField] private Toggle m_RunInBackgroundToggle;
        [SerializeField] private Toggle m_AlwaysShowEnemyHealthToggle;
        [SerializeField] private Toggle m_AlwaysShowEnemySkillsToggle;
        [SerializeField] private Toggle m_DisplayFormulasInTooltipsToggle;
        [SerializeField] private Toggle m_DisableErrorMessagesToggle;
        [SerializeField] private Toggle m_DisableCombatTextToggle;
        [SerializeField] private Toggle m_SummonedUnitsControlledByAiToggle;
        [SerializeField] private Toggle m_HideHealthTextToggle;
        [SerializeField] private Toggle m_HideHealthToggle;
        [SerializeField] private Toggle m_HideBuffsToggle;
        [SerializeField] private Toggle m_HideSkillsToggle;
        [SerializeField] private Toggle m_HighContrastModeToggle;
        [SerializeField] private Slider m_MasterVolumeSlider;
        [SerializeField] private Slider m_MusicVolumeSlider;
        [SerializeField] private Slider m_SoundVolumeSlider;
        [SerializeField] private Interactable m_OkayButton;
        [SerializeField] private Interactable m_ResetButton;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Transform m_FullscreenModeDropdownContainer;
        [SerializeField] private Transform m_ResolutionDropdownContainer;

        public void Construct(SettingsManager settings)
        {
            m_MasterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            m_MusicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            m_SoundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            m_LoopMusicToggle.onValueChanged.AddListener(OnLoopMusicChanged);
            m_HideActingUnitHealthToggle.onValueChanged.AddListener(OnHideActingUnitHealthChanged);
            m_DisableUiSoundsToggle.onValueChanged.AddListener(OnUiSoundsChanged);
            m_VerticalSyncToggle.onValueChanged.AddListener(OnVerticalSyncChanged);
            m_RunInBackgroundToggle.onValueChanged.AddListener(OnRunInBackgroundChanged);
            m_AlwaysShowEnemyHealthToggle.onValueChanged.AddListener(OnAlwaysShowEnemyHealthChanged);
            m_AlwaysShowEnemySkillsToggle.onValueChanged.AddListener(OnAlwaysShowEnemySkillsChanged);
            m_DisplayFormulasInTooltipsToggle.onValueChanged.AddListener(OnDisplayFormulasInTooltipsChanged);
            m_DisableErrorMessagesToggle.onValueChanged.AddListener(OnDisableErrorMessagesChanged);
            m_DisableCombatTextToggle.onValueChanged.AddListener(OnDisableCombatTextChanged);
            m_HideBuffsToggle.onValueChanged.AddListener(OnHideBuffsChanged);
            m_HideSkillsToggle.onValueChanged.AddListener(OnHideSkillsChanged);
            m_HighContrastModeToggle.onValueChanged.AddListener(OnHighContrastModeChanged);
            m_HideHealthToggle.onValueChanged.AddListener(OnHideHealthChanged);
            m_HideHealthTextToggle.onValueChanged.AddListener(OnHideHealthTextChanged);
            m_SummonedUnitsControlledByAiToggle.onValueChanged.AddListener(OnSummonedUnitsControlledByAiChanged);

            m_ResetButton.PointerClick += OnResetButtonPointerClick;
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
            m_OkayButton.PointerClick += OnCloseButtonPointerClick;

            Refresh(settings);
        }

        public void Refresh(SettingsManager settings)
        {
            if (settings.GetResolutions().Count > 0)
            {
                InitializeResolutionsDropdown(settings.GetResolutions(), settings.ResolutionIndex);
                InitializeFullscreenModeDropdown(settings.GetDisplayModes(), settings.DisplayModeIndex);
            }
            else
            {
                m_ResolutionDropdownContainer.gameObject.SetActive(false);
                m_FullscreenModeDropdownContainer.gameObject.SetActive(false);
            }

            InitializeLanguagesDropdown(I18N.Instance.Dictionaries, settings.Locale);

            m_MasterVolumeSlider.value = settings.MasterVolume;
            m_MusicVolumeSlider.value = settings.MusicVolume;
            m_SoundVolumeSlider.value = settings.SoundVolume;
            m_VerticalSyncToggle.isOn = settings.VerticalSync > 0;
            m_LoopMusicToggle.isOn = settings.LoopMusic;
            m_DisableUiSoundsToggle.isOn = settings.DisableUiSounds;
            m_RunInBackgroundToggle.isOn = settings.RunInBackground;
            m_AlwaysShowEnemyHealthToggle.isOn = settings.AlwaysShowHealth;
            m_AlwaysShowEnemySkillsToggle.isOn = settings.AlwaysShowSkills;
            m_DisplayFormulasInTooltipsToggle.isOn = settings.DisplayFormulasInTooltips;
            m_DisableErrorMessagesToggle.isOn = settings.DisableErrorMessages;
            m_DisableCombatTextToggle.isOn = settings.DisableCombatText;
            m_HideBuffsToggle.isOn = settings.HideBuffs;
            m_HideSkillsToggle.isOn = settings.HideSkills;
            m_HighContrastModeToggle.isOn = settings.HighContrastMode;
            m_HideHealthToggle.isOn = settings.HideHealth;
            m_HideHealthTextToggle.isOn = settings.HideHealthText;
            m_SummonedUnitsControlledByAiToggle.isOn = settings.SummonedUnitsControlledByAi;
        }

        private void InitializeFullscreenModeDropdown(IEnumerable<FullScreenMode> modes, int value)
        {
            InitializeDropdown(
                m_FullscreenModeDropdown,
                modes.Select(mode => (string) EnumTranslator.Translate(mode)).ToList(), value, OnFullscreenModeChanged
            );
        }

        private void InitializeResolutionsDropdown(IEnumerable<Resolution> resolutions, int value)
        {
            InitializeDropdown(
                m_ResolutionDropdown,
                resolutions.Select(resolution => resolution.ToString()).ToList(), value, OnResolutionChanged
            );
        }

        private void InitializeLanguagesDropdown(Dictionary<string, I18NDictionaryInfo> dictionaries, string locale)
        {
            InitializeDropdown(
                m_LanguageDropdown,
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

        private void OnHighContrastModeChanged(bool value)
        {
            ToggleHighContrastMode?.Invoke(value);
        }

        private void OnHideBuffsChanged(bool value)
        {
            ToggleHideBuffs?.Invoke(value);
        }

        private void OnDisableCombatTextChanged(bool value)
        {
            ToggleDisableCombatText?.Invoke(value);
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

        private void OnHideActingUnitHealthChanged(bool value)
        {
            ToggleHideActingUnitHealth?.Invoke(value);
        }

        private void OnUiSoundsChanged(bool value)
        {
            ToggleDisableUiSounds?.Invoke(value);
        }

        private void OnLoopMusicChanged(bool value)
        {
            ToggleLoopMusic?.Invoke(value);
        }

        private void OnVerticalSyncChanged(bool value)
        {
            ToggleVerticalSync?.Invoke(value);
        }

        private void OnRunInBackgroundChanged(bool value)
        {
            ToggleRunInBackground?.Invoke(value);
        }

        private void OnResetButtonPointerClick()
        {
            Resetting?.Invoke();
        }

        private void OnCloseButtonPointerClick()
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