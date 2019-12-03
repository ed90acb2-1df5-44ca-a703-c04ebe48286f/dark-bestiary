using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioProgress : MonoBehaviour
    {
        [SerializeField] private RectTransform connection;
        [SerializeField] private ScenarioProgressEpisode episodePrefab;
        [SerializeField] private Transform episodeContainer;

        private readonly Dictionary<Episode, ScenarioProgressEpisode> episodeViews =
            new Dictionary<Episode, ScenarioProgressEpisode>();

        private Scenario scenario;

        public void Initialize(Scenario scenario)
        {
            this.scenario = scenario;

            foreach (var episode in this.scenario.Episodes)
            {
                episode.Started += OnEpisodeStarted;
                episode.Completed += OnEpisodeCompleted;

                var episodeView = Instantiate(this.episodePrefab, this.episodeContainer);
                episodeView.SetInactive();

                this.episodeViews.Add(episode, episodeView);
            }

            this.episodeViews.Values.First().SetCurrent();
        }

        public void Terminate()
        {
            foreach (var episode in this.episodeViews.Keys)
            {
                episode.Started -= OnEpisodeStarted;
                episode.Completed -= OnEpisodeCompleted;
            }

            Destroy(gameObject);
        }

        private void OnEpisodeStarted(Episode episode)
        {
            this.episodeViews[episode].SetCurrent();
        }

        private void OnEpisodeCompleted(Episode episode)
        {
            this.episodeViews[episode].SetCompleted();

            if (this.scenario.IsActiveEpisodeLast)
            {
                return;
            }

            this.connection.sizeDelta = new Vector2(
                96 * this.scenario.Episodes.Count(e => e.IsCompleted),
                this.connection.rect.height);
        }
    }
}