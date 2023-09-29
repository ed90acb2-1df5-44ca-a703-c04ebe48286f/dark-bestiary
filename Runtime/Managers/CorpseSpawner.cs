using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CorpseSpawner : Singleton<CorpseSpawner>
    {
        [SerializeField] private Corpse m_DefaultCorpsePrefab;

        private readonly Dictionary<Episode, List<Corpse>> m_Corpses = new();

        private Episode m_CurrentEpisode;

        private void Start()
        {
            Corpse.AnyCorpseConsumed += OnAnyCorpseConsumed;

            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;
            Episode.AnyEpisodeStopped += OnAnyEpisodeStopped;
            Episode.AnyEpisodeTerminated += OnAnyEpisodeTerminated;

            HealthComponent.AnyEntityDied += OnAnyEntityDied;
        }

        public void SpawnCorpse(Vector3 position, Corpse prefab = null)
        {
            if (m_CurrentEpisode == null)
            {
                Debug.LogError("Can't spawn corpse outside of episode.");
            }

            SpawnCorpse(prefab == null ? m_DefaultCorpsePrefab : prefab, m_CurrentEpisode, position);
        }

        private void SpawnCorpse(Corpse prefab, Episode episode, Vector3 position)
        {
            Timer.Instance.Wait(1.5f, () =>
            {
                if (Scenario.Active == null)
                {
                    return;
                }

                m_Corpses[episode].Add(Instantiate(prefab, position, Quaternion.identity));
            });
        }

        private void ShowCorpsesFromEpisode(Episode episode)
        {
            if (!m_Corpses.ContainsKey(episode))
            {
                m_Corpses.Add(episode, new List<Corpse>());
            }

            foreach (var corpse in m_Corpses[episode])
            {
                corpse.gameObject.SetActive(true);
            }
        }

        private void HideCorpsesFromEpisode(Episode episode)
        {
            if (!m_Corpses.ContainsKey(episode))
            {
                return;
            }

            foreach (var corpse in m_Corpses[episode])
            {
                corpse.gameObject.SetActive(false);
            }
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            m_CurrentEpisode = episode;

            ShowCorpsesFromEpisode(m_CurrentEpisode);
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            var unit = data.Victim.GetComponent<UnitComponent>();

            if (unit.Flags.HasFlag(UnitFlags.Dummy) || unit.Flags.HasFlag(UnitFlags.Corpseless))
            {
                return;
            }

            SpawnCorpse(GetCorpse(data.Victim), m_CurrentEpisode, data.Victim.transform.position.Snapped());
        }

        private void OnAnyEpisodeStopped(Episode episode)
        {
            HideCorpsesFromEpisode(episode);
        }

        private Corpse GetCorpse(GameObject entity)
        {
            return Resources.Load<Corpse>(entity.GetComponent<UnitComponent>().Corpse) ?? m_DefaultCorpsePrefab;
        }

        private void OnAnyEpisodeTerminated(Episode episode)
        {
            if (!m_Corpses.ContainsKey(episode))
            {
                return;
            }

            foreach (var corpse in m_Corpses[episode])
            {
                if (corpse == null)
                {
                    continue;
                }

                Destroy(corpse.gameObject);
            }
        }

        private void OnAnyCorpseConsumed(Corpse corpse)
        {
            m_Corpses[m_CurrentEpisode].Remove(corpse);
        }
    }
}