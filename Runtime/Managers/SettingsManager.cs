using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Audio;
using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Managers
{
    public class SettingsManager : IInitializable
    {
        public static SettingsManager Instance { get; private set; }

        public SettingsData Data { get; private set; }

        public int ResolutionIndex => Data.ResolutionIndex;
        public int DisplayModeIndex => Data.DisplayModeIndex;
        public string Locale => Data.Locale;
        public float MasterVolume => Data.MasterVolume;
        public float MusicVolume => Data.MusicVolume;
        public float SoundVolume => Data.SoundVolume;
        public int VerticalSync => Data.VerticalSync;
        public bool DisableUiSounds => Data.DisableUiSounds;
        public bool LoopMusic => Data.LoopMusic;
        public bool RunInBackground => Data.RunInBackground;

        public bool HighContrastMode => Data.HighContrastMode;
        public bool HideBuffs => Data.HideBuffs;
        public bool HideSkills => Data.HideSkills;
        public bool HideHealth => Data.HideHealth;
        public bool HideHealthText => Data.HideHealthText;
        public bool DisableErrorMessages => Data.DisableErrorMessages;
        public bool SummonedUnitsControlledByAi => Data.SummonedUnitsControlledByAi;
        public bool DisableCombatText => Data.DisableCombatText;
        public bool AlwaysShowHealth => Data.AlwaysShowHealth;
        public bool HideActingUnitHealth => Data.HideActingUnitHealth;
        public bool AlwaysShowSkills => Data.AlwaysShowSkills;
        public bool DisplayFormulasInTooltips => Data.DisplayFormulasInTooltips;

        private readonly IFileReader m_Reader;
        private readonly IAudioEngine m_AudioEngine;

        public SettingsManager(IFileReader reader, IAudioEngine audioEngine)
        {
            m_Reader = reader;
            m_AudioEngine = audioEngine;

            Instance = this;
        }

        public void Initialize()
        {
            try
            {
                Data = m_Reader.Read<SettingsData>(GetDataPath()) ?? new SettingsData();
            }
            catch (Exception exception)
            {
                Data = new SettingsData();
            }

            Setup();

            Application.quitting += OnApplicationQuitting;
        }

        private void Setup()
        {
            SetLocale(Data.Locale);
            SetMasterVolume(Data.MasterVolume);
            SetSoundVolume(Data.SoundVolume);
            SetMusicVolume(Data.MusicVolume);
            SetVerticalSync(Data.VerticalSync);
            SetRunInBackground(Data.RunInBackground);
            SetDisableErrorMessages(Data.DisableErrorMessages);
            SetHideActingUnitHealth(Data.HideActingUnitHealth);
            SetAlwaysShowHealth(Data.AlwaysShowHealth);
            SetAlwaysShowSkills(Data.AlwaysShowSkills);
            SetHideHealthText(Data.HideHealthText);
            SetHideHealth(Data.HideHealth);
            SetHideSkills(Data.HideSkills);
            SetHideBuffs(Data.HideBuffs);
        }

        public void Apply(SettingsData data)
        {
            Data = data;

            Setup();

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                SetResolution(data.ResolutionIndex);
                SetDisplayMode(data.DisplayModeIndex);
            });
        }

        public void Reset()
        {
            Apply(new SettingsData());
        }

        public Resolution? GetResolution(int index)
        {
            var resolutions = GetResolutions();
            return resolutions.IndexInBounds(index) ? resolutions[index] : resolutions.LastOrDefault();
        }

        public FullScreenMode? GetDisplayMode(int index)
        {
            var modes = GetDisplayModes();
            return modes.IndexInBounds(index) ? modes[index] : modes.LastOrDefault();
        }

        public List<Resolution> GetResolutions()
        {
            return Screen.resolutions.ToList();
        }

        public List<FullScreenMode> GetDisplayModes()
        {
            return new List<FullScreenMode>
            {
                FullScreenMode.ExclusiveFullScreen,
                FullScreenMode.FullScreenWindow,
                FullScreenMode.MaximizedWindow,
                FullScreenMode.Windowed
            };
        }

        public void SetLocale(string locale)
        {
            I18N.Instance.ChangeLocale(locale);
            Data.Locale = locale;
        }

        public void SetDisplayFormulasInTooltips(bool value)
        {
            Data.DisplayFormulasInTooltips = value;
        }

        public void SetDisableErrorMessages(bool value)
        {
            Data.DisableErrorMessages = value;
        }

        public void SetDisableCombatText(bool value)
        {
            Data.DisableCombatText = value;
        }

        public void SetToggleSummonedUnitsControlledByAi(bool value)
        {
            Data.SummonedUnitsControlledByAi = value;
        }

        public void SetLoopMusic(bool value)
        {
            Data.LoopMusic = value;
        }

        public void SetDisableUiSounds(bool value)
        {
            Data.DisableUiSounds = value;
        }

        public void SetHideHealthText(bool value)
        {
            Data.HideHealthText = value;
            FloatingHealthBar.HideHealthText(value);
        }

        public void SetHideHealth(bool value)
        {
            Data.HideHealth = value;
            FloatingHealthBar.HideHealth(value);
        }

        public void SetHideBuffs(bool value)
        {
            Data.HideBuffs = value;
            FloatingHealthBar.HideBuffs(value);
        }

        public void SetHighContrastMode(bool value)
        {
            Data.HighContrastMode = value;
        }

        public void SetHideSkills(bool value)
        {
            Data.HideSkills = value;
            FloatingActionBar.HideSkills(value);
        }

        public void SetHideActingUnitHealth(bool value)
        {
            Data.HideActingUnitHealth = value;
        }

        public void SetAlwaysShowHealth(bool value)
        {
            Data.AlwaysShowHealth = value;
            FloatingHealthBar.AlwaysShow(value);
        }

        public void SetAlwaysShowSkills(bool value)
        {
            Data.AlwaysShowSkills = value;
            FloatingActionBar.AlwaysShow(value);
        }

        public void SetVerticalSync(int value)
        {
            Data.VerticalSync = value;
            QualitySettings.vSyncCount = value;
        }

        public void SetRunInBackground(bool value)
        {
            Data.RunInBackground = value;
            Application.runInBackground = value;
        }

        public void SetResolution(int index)
        {
            var resolution = GetResolution(index);

            if (!resolution.HasValue)
            {
                return;
            }

            SetResolution(resolution.Value);
        }

        public void SetDisplayMode(int index)
        {
            var mode = GetDisplayMode(index);

            if (!mode.HasValue)
            {
                return;
            }

            SetDisplayMode(mode.Value);
        }

        public void SetDisplayMode(FullScreenMode mode)
        {
            Screen.fullScreenMode = mode;
        }

        public void SetResolution(Resolution resolution)
        {
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
            Data.ResolutionIndex = GetResolutions().IndexOf(resolution);
        }

        public void SetMasterVolume(float volume)
        {
            m_AudioEngine.SetMasterVolume(volume);
            Data.MasterVolume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            m_AudioEngine.SetMusicVolume(volume);
            Data.MusicVolume = volume;
        }

        public void SetSoundVolume(float volume)
        {
            m_AudioEngine.SetSoundVolume(volume);
            Data.SoundVolume = volume;
        }

        private static string GetDataPath()
        {
            return Environment.s_PersistentDataPath + "/settings.json";
        }

        private void OnApplicationQuitting()
        {
            m_Reader.Write(Data, GetDataPath());
        }
    }
}