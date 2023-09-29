using UnityEngine;

namespace DarkBestiary.Audio
{
    public interface IAudioEngine
    {
        void SetMasterVolume(float fraction);

        void SetMusicVolume(float fraction);

        void SetSoundVolume(float fraction);

        void PlayOneShot(string path);

        void PlayOneShot(string path, Vector3 position);
    }
}