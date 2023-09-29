using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

namespace DarkBestiary.Managers
{
    public class MusicManager : Singleton<MusicManager>
    {
        private EventInstance m_Music;
        private string m_Path;

        public void Play(string path)
        {
            if (m_Path == path)
            {
                return;
            }

            m_Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            try
            {
                m_Music = RuntimeManager.CreateInstance(path);
                m_Music.setParameterByName("Loop", SettingsManager.Instance.LoopMusic ? 1 : 0);
                m_Music.start();
                m_Path = path;
            }
            catch (EventNotFoundException exception)
            {
                Debug.LogError("Event not found at path: " + exception.Message);
            }
        }

        public void Stop()
        {
            m_Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            m_Path = "";
        }
    }
}