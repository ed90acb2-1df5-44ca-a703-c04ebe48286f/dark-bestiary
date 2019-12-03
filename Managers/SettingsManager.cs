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

        private readonly IFileReader reader;
        private readonly IAudioEngine audioEngine;

        public int ResolutionIndex => this.data.ResolutionIndex;
        public int DisplayModeIndex => this.data.DisplayModeIndex;
        public string Locale => this.data.Locale;
        public float MasterVolume => this.data.MasterVolume;
        public float MusicVolume => this.data.MusicVolume;
        public float SoundVolume => this.data.SoundVolume;
        public int VerticalSync => this.data.VerticalSync;
        public bool RunInBackground => this.data.RunInBackground;

        public bool HideBuffs => this.data.HideBuffs;
        public bool HideSkills => this.data.HideSkills;
        public bool HideHealth => this.data.HideHealth;
        public bool HideHealthText => this.data.HideHealthText;
        public bool DisableErrorMessages => this.data.DisableErrorMessages;
        public bool SummonedUnitsControlledByAi => this.data.SummonedUnitsControlledByAi;
        public bool DisableCombatText => this.data.DisableCombatText;
        public bool DisableCameraShake => this.data.DisableCameraShake;
        public bool AlwaysShowHealth => this.data.AlwaysShowHealth;
        public bool AlwaysShowSkills => this.data.AlwaysShowSkills;
        public bool DisplayFormulasInTooltips => this.data.DisplayFormulasInTooltips;

        private SettingsData data;

        public SettingsManager(IFileReader reader, IAudioEngine audioEngine)
        {
            this.reader = reader;
            this.audioEngine = audioEngine;

            Instance = this;
        }

        public void Initialize()
        {
            try
            {
                this.data = this.reader.Read<SettingsData>(GetDataPath()) ?? new SettingsData();
            }
            catch (Exception exception)
            {
                this.data = new SettingsData();
            }

            Setup();

            Application.quitting += OnApplicationQuitting;
        }

        private void Setup()
        {
            SetLocale(this.data.Locale);
            SetMasterVolume(this.data.MasterVolume);
            SetSoundVolume(this.data.SoundVolume);
            SetMusicVolume(this.data.MusicVolume);
            SetVerticalSync(this.data.VerticalSync);
            SetRunInBackground(this.data.RunInBackground);
            SetDisableErrorMessages(this.data.DisableErrorMessages);
            SetAlwaysShowHealth(this.data.AlwaysShowHealth);
            SetAlwaysShowSkills(this.data.AlwaysShowSkills);
            SetHideHealthText(this.data.HideHealthText);
            SetHideHealth(this.data.HideHealth);
            SetHideSkills(this.data.HideSkills);
            SetHideBuffs(this.data.HideBuffs);
        }

        public void Apply(SettingsData data)
        {
            this.data = data;

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
            this.data.Locale = locale;
        }

        public void SetDisplayFormulasInTooltips(bool value)
        {
            this.data.DisplayFormulasInTooltips = value;
        }

        public void SetDisableErrorMessages(bool value)
        {
            this.data.DisableErrorMessages = value;
        }

        public void SetDisableCombatText(bool value)
        {
            this.data.DisableCombatText = value;
        }

        public void SetDisableCameraShake(bool value)
        {
            this.data.DisableCameraShake = value;
        }

        public void SetToggleSummonedUnitsControlledByAi(bool value)
        {
            this.data.SummonedUnitsControlledByAi = value;
        }

        public void SetHideHealthText(bool value)
        {
            this.data.HideHealthText = value;
            FloatingHealthBar.HideHealthText(value);
        }

        public void SetHideHealth(bool value)
        {
            this.data.HideHealth = value;
            FloatingHealthBar.HideHealth(value);
        }

        public void SetHideBuffs(bool value)
        {
            this.data.HideBuffs = value;
            FloatingHealthBar.HideBuffs(value);
        }

        public void SetHideSkills(bool value)
        {
            this.data.HideSkills = value;
            FloatingActionBar.HideSkills(value);
        }

        public void SetAlwaysShowHealth(bool value)
        {
            this.data.AlwaysShowHealth = value;
            FloatingHealthBar.AlwaysShow(value);
        }

        public void SetAlwaysShowSkills(bool value)
        {
            this.data.AlwaysShowSkills = value;
            FloatingActionBar.AlwaysShow(value);
        }

        public void SetVerticalSync(int value)
        {
            this.data.VerticalSync = value;
            QualitySettings.vSyncCount = value;
        }

        public void SetRunInBackground(bool value)
        {
            this.data.RunInBackground = value;
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
            this.data.ResolutionIndex = GetResolutions().IndexOf(resolution);
        }

        public void SetMasterVolume(float volume)
        {
            this.audioEngine.SetMasterVolume(volume);
            this.data.MasterVolume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            this.audioEngine.SetMusicVolume(volume);
            this.data.MusicVolume = volume;
        }

        public void SetSoundVolume(float volume)
        {
            this.audioEngine.SetSoundVolume(volume);
            this.data.SoundVolume = volume;
        }

        private static string GetDataPath()
        {
            return Application.persistentDataPath + "/settings.json";
        }

        private void OnApplicationQuitting()
        {
            this.reader.Write(this.data, GetDataPath());
        }
    }
}