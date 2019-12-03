using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.Scenarios
{
    public class Episode
    {
        public static event Payload<Episode> AnyEpisodeStarted;
        public static event Payload<Episode> AnyEpisodeStopped;
        public static event Payload<Episode> AnyEpisodeCompleted;
        public static event Payload<Episode> AnyEpisodeFailed;
        public static event Payload<Episode> AnyEpisodeInitialized;
        public static event Payload<Episode> AnyEpisodeTerminated;

        public event Payload<Episode> Stopped;
        public event Payload<Episode> Started;
        public event Payload<Episode> Completed;
        public event Payload<Episode> Failed;

        public Scene Scene { get; }
        public Encounter Encounter { get; }
        public Scenario Scenario { get; private set; }
        public bool IsCompleted { get; private set; }

        private readonly IPathfinder pathfinder;

        public Episode(Scene scene, Encounter encounter)
        {
            Scene = scene;
            Encounter = encounter;
        }

        public void Initialize(Scenario scenario)
        {
            Scenario = scenario;

            Scene.Initialize();
            Scene.CloseEnterDoor();
            Scene.CloseExitDoor();
            AnyEpisodeInitialized?.Invoke(this);

            Timer.Instance.Wait(1.0f, () =>
            {
                Encounter.Completed += OnEncounterCompleted;
                Encounter.Failed += OnEncounterFailed;
                Encounter.Start();
            });
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
            Scene.EnterDoor += OnEnterDoor;
            Scene.ExitDoor += OnExitDoor;
            Scene.Show();

            if (IsCompleted)
            {
                if (!Scenario.IsActiveEpisodeLast)
                {
                    Scene.OpenExitDoor();
                }
            }

            MoveEntitiesToSpawnPoints();

            Started?.Invoke(this);
            AnyEpisodeStarted?.Invoke(this);
        }

        public void Stop()
        {
            Scene.EnterDoor -= OnEnterDoor;
            Scene.ExitDoor -= OnExitDoor;
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

            if (!IsCompleted)
            {
                Scene.MoveEnemiesToSpawnPoints();
            }

            if (Scenario.NextEpisode == null ||
                Scenario.Episodes.IndexOf(this) > Scenario.Episodes.IndexOf(Scenario.PreviousEpisode))
            {
                Scene.MoveAlliesToEnter();
            }
            else
            {
                Scene.MoveAlliesToExit();
            }
        }

        private void OnEncounterCompleted(Encounter encounter)
        {
            IsCompleted = true;

            Scene.OpenExitDoor();

            Completed?.Invoke(this);
            AnyEpisodeCompleted?.Invoke(this);
        }

        private void OnEncounterFailed(Encounter encounter)
        {
            Failed?.Invoke(this);
            AnyEpisodeFailed?.Invoke(this);
        }

        private void OnExitDoor(GameObject entity)
        {
            if (!entity.IsCharacter())
            {
                return;
            }

            Scenario.StartNextEpisode();
        }

        private void OnEnterDoor(GameObject entity)
        {
            if (!entity.IsCharacter())
            {
                return;
            }

            Scenario.StartPreviousEpisode();
        }
    }
}