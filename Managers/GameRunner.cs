using UnityEngine;
using Version = System.Version;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DarkBestiary.Managers
{
    public class GameRunner : Singleton<GameRunner>
    {
        [SerializeField] private string version = "0.0.0.0";

        private Game game;

        private void Start()
        {
            // Note: wait for unity "Start" callback
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                this.game = Container.Instance.Instantiate<Game>(new object[] {this.version});
                this.game.Start();
            });
        }

        private void Update()
        {
            this.game?.Tick(Time.deltaTime);
        }

        private void IncrementVersion(int? major = null, int? minor = null, int? build = null, int? revision = null)
        {
            var currentVersion = new Version(this.version);

            currentVersion = new Version(
                (currentVersion.Major < 0 ? 0 : currentVersion.Major) + (major ?? 0),
                (currentVersion.Minor < 0 ? 0 : currentVersion.Minor) + (minor ?? 0),
                (currentVersion.Build < 0 ? 0 : currentVersion.Build) + (build ?? 0),
                (currentVersion.Revision < 0 ? 0 : currentVersion.Revision) + (revision ?? 0)
            );

            this.version = currentVersion.ToString();

            Debug.Log("Build version: " + this.version);
        }

#if UNITY_EDITOR
        static GameRunner()
        {
            EditorApplication.update += RunOnce;
        }

        private static void RunOnce()
        {
            EditorApplication.update -= RunOnce;
            Instance.IncrementVersion(revision: 1);
        }
#endif
    }
}