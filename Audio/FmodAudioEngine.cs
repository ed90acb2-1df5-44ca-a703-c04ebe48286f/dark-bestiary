using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace DarkBestiary.Audio
{
    public class FmodAudioEngine : IAudioEngine
    {
        private Bus masterBus;
        private Bus musicBus;
        private Bus soundBus;

        public FmodAudioEngine()
        {
            this.masterBus = RuntimeManager.GetBus("bus:/Master");
            this.musicBus = RuntimeManager.GetBus("bus:/Master/Music");
            this.soundBus = RuntimeManager.GetBus("bus:/Master/Sound");
        }

        public void SetMasterVolume(float fraction)
        {
            this.masterBus.setVolume(fraction);
        }

        public void SetMusicVolume(float fraction)
        {
            this.musicBus.setVolume(fraction);
        }

        public void SetSoundVolume(float fraction)
        {
            this.soundBus.setVolume(fraction);
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