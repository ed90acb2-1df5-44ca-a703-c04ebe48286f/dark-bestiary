using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioProgress : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_Connection;

        [SerializeField]
        private ScenarioProgressEpisode m_EpisodePrefab;

        [SerializeField]
        private Transform m_EpisodeContainer;

        private readonly Dictionary<Episode, ScenarioProgressEpisode> m_EpisodeViews = new();

        private Scenario m_Scenario;

        public void Initialize(Scenario scenario)
        {
            m_Scenario = scenario;

            foreach (var episode in m_Scenario.Episodes)
            {
                episode.Started += OnEpisodeStarted;
                episode.Completed += OnEpisodeCompleted;

                var episodeView = Instantiate(m_EpisodePrefab, m_EpisodeContainer);
                episodeView.SetInactive();

                m_EpisodeViews.Add(episode, episodeView);
            }

            m_EpisodeViews.Values.First().SetCurrent();
        }

        public void Terminate()
        {
            foreach (var episode in m_EpisodeViews.Keys)
            {
                episode.Started -= OnEpisodeStarted;
                episode.Completed -= OnEpisodeCompleted;
            }

            Destroy(gameObject);
        }

        private void OnEpisodeStarted(Episode episode)
        {
            m_EpisodeViews[episode].SetCurrent();
        }

        private void OnEpisodeCompleted(Episode episode)
        {
            m_EpisodeViews[episode].SetCompleted();

            if (m_Scenario.IsActiveEpisodeLast)
            {
                return;
            }

            m_Connection.sizeDelta = new Vector2(96 * m_Scenario.ActiveEpisodeIndex, m_Connection.rect.height);
        }
    }
}