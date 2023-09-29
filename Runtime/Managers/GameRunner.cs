using DarkBestiary.Analysis;
using DarkBestiary.Data;
using TMPro;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_VersionText = null!;

        private void Start()
        {
            #if !DISABLESTEAMWORKS
            using (new StopwatchSection("Collect data"))
            {
                DataCollector.Collect();
            }
            #endif

            var version = "2.0.0";

            #if !DISABLESTEAMWORKS
            if (SteamManager.Initialized)
            {
                version = $"{version}.{Steamworks.SteamApps.GetAppBuildId().ToString()}";
            }
            else
            {
                Application.Quit();
            }
            #endif

            m_VersionText.text = version;

            Container.Instance.Instantiate<Game>();
        }
    }
}