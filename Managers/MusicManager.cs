using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

namespace DarkBestiary.Managers
{
    public class MusicManager : Singleton<MusicManager>
    {
        private EventInstance music;
        private string path;

        public void Play(string path)
        {
            if (this.path == path)
            {
                return;
            }

            this.music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            try
            {
                this.music = RuntimeManager.CreateInstance(path);
                this.music.start();
                this.path = path;
            }
            catch (EventNotFoundException exception)
            {
                Debug.LogError("Event not found at path: " + exception.Message);
            }
        }

        public void Stop()
        {
            this.music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            this.path = "";
        }
    }
}