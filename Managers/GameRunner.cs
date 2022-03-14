using DarkBestiary.Analysis;
using DarkBestiary.Data;
using TMPro;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class GameRunner : Singleton<GameRunner>
    {
        [SerializeField] private TextMeshProUGUI versionText;

        private Game game;

        private void Awake()
        {
            Game.IsForgottenDepthsEnabled = Application.isEditor;

            #if !DISABLESTEAMWORKS
            using (new StopwatchSection("Collect data"))
            {
                DataCollector.Collect();
            }
            #endif
        }

        private void Start()
        {
            var version = "1.1.1";

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

            this.versionText.text = version;

            // Note: wait for unity "Start" callback
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                this.game = Container.Instance.Instantiate<Game>(new object[] {version});
                this.game.Start();
            });
        }

        private void Update()
        {
            this.game?.Tick(Time.deltaTime);
        }
    }
}