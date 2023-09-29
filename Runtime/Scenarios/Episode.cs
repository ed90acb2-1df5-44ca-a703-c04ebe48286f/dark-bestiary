using System;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.Scenarios
{
    public class Episode
    {
        public static event Action<Episode> AnyEpisodeStarted;
        public static event Action<Episode> AnyEpisodeStopped;
        public static event Action<Episode> AnyEpisodeCompleted;
        public static event Action<Episode> AnyEpisodeFailed;
        public static event Action<Episode> AnyEpisodeInitialized;
        public static event Action<Episode> AnyEpisodeTerminated;

        public event Action<Episode> Stopped;
        public event Action<Episode> Started;
        public event Action<Episode> Completed;
        public event Action<Episode> Failed;

        public Scene Scene { get; }
        public Encounter Encounter { get; }
        public Scenario Scenario { get; private set; }

        private readonly IPathfinder m_Pathfinder;

        public Episode(Scene scene, Encounter encounter)
        {
            Scene = scene;
            Encounter = encounter;
        }

        public void Initialize(Scenario scenario)
        {
            Scenario = scenario;

            Scene.Initialize();

            AnyEpisodeInitialized?.Invoke(this);

            Encounter.Completed += OnEncounterCompleted;
            Encounter.Failed += OnEncounterFailed;
        }

        public void Terminate()
        {
            Scene.Terminate();
            AnyEpisodeTerminated?.Invoke(this);

            Encounter.Completed -= OnEncounterCompleted;
            Encounter.Failed -= OnEncounterFailed;
            Encounter.Stop();
        }

        public void Start()
        {
            Scene.Show();

            MoveEntitiesToSpawnPoints();

            Encounter.Start();

            Started?.Invoke(this);
            AnyEpisodeStarted?.Invoke(this);
        }

        public void Stop()
        {
            Scene.Hide();

            Stopped?.Invoke(this);
            AnyEpisodeStopped?.Invoke(this);
        }

        public void Tick(float delta)
        {
            Encounter.Tick(delta);
        }

        private void MoveEntitiesToSpawnPoints()
        {
            Pathfinder.Instance.Scan();

            Scene.MoveToSpawnPointsFormation(1);
            Scene.MoveToSpawnPointsRandom(2);
        }

        private void OnEncounterCompleted(Encounter encounter)
        {
            Completed?.Invoke(this);
            AnyEpisodeCompleted?.Invoke(this);
        }

        private void OnEncounterFailed(Encounter encounter)
        {
            Failed?.Invoke(this);
            AnyEpisodeFailed?.Invoke(this);
        }
    }
}