using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace DarkBestiary.Audio
{
    public class FmodAudioEngine : IAudioEngine
    {
        private Bus m_MasterBus;
        private Bus m_MusicBus;
        private Bus m_SoundBus;

        public FmodAudioEngine()
        {
            m_MasterBus = RuntimeManager.GetBus("bus:/Master");
            m_MusicBus = RuntimeManager.GetBus("bus:/Master/Music");
            m_SoundBus = RuntimeManager.GetBus("bus:/Master/Sound");
        }

        public void SetMasterVolume(float fraction)
        {
            m_MasterBus.setVolume(fraction);
        }

        public void SetMusicVolume(float fraction)
        {
            m_MusicBus.setVolume(fraction);
        }

        public void SetSoundVolume(float fraction)
        {
            m_SoundBus.setVolume(fraction);
        }

        public void PlayOneShot(string path)
        {
            PlayOneShot(path, Vector3.zero);
        }

        public void PlayOneShot(string path, Vector3 position)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            RuntimeManager.PlayOneShot(path, position);
        }
    }
}