using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CorpseSpawner : Singleton<CorpseSpawner>
    {
        [SerializeField] private Corpse defaultCorpsePrefab;

        private readonly Dictionary<Episode, List<Corpse>> corpses = new Dictionary<Episode, List<Corpse>>();

        private Episode currentEpisode;

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
            if (this.currentEpisode == null)
            {
                Debug.LogError("Can't spawn corpse outside of episode.");
            }

            SpawnCorpse(prefab == null ? this.defaultCorpsePrefab : prefab, this.currentEpisode, position);
        }

        private void SpawnCorpse(Corpse prefab, Episode episode, Vector3 position)
        {
            Timer.Instance.Wait(1.5f, () =>
            {
                this.corpses[episode].Add(Instantiate(prefab, position, Quaternion.identity));
            });
        }

        private void ShowCorpsesFromEpisode(Episode episode)
        {
            if (!this.corpses.ContainsKey(episode))
            {
                this.corpses.Add(episode, new List<Corpse>());
            }

            foreach (var corpse in this.corpses[episode])
            {
                corpse.gameObject.SetActive(true);
            }
        }

        private void HideCorpsesFromEpisode(Episode episode)
        {
            if (!this.corpses.ContainsKey(episode))
            {
                return;
            }

            foreach (var corpse in this.corpses[episode])
            {
                corpse.gameObject.SetActive(false);
            }
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            this.currentEpisode = episode;

            ShowCorpsesFromEpisode(this.currentEpisode);
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            var unit = data.Victim.GetComponent<UnitComponent>();

            if (unit.Flags.HasFlag(UnitFlags.Dummy) || unit.Flags.HasFlag(UnitFlags.Corpseless))
            {
                return;
            }

            SpawnCorpse(GetCorpse(data.Victim), this.currentEpisode, data.Victim.transform.position);
        }

        private void OnAnyEpisodeStopped(Episode episode)
        {
            HideCorpsesFromEpisode(episode);
        }

        private Corpse GetCorpse(GameObject entity)
        {
            return Resources.Load<Corpse>(entity.GetComponent<UnitComponent>().Corpse) ?? this.defaultCorpsePrefab;
        }

        private void OnAnyEpisodeTerminated(Episode episode)
        {
            if (!this.corpses.ContainsKey(episode))
            {
                return;
            }

            foreach (var corpse in this.corpses[episode])
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
            this.corpses[this.currentEpisode].Remove(corpse);
        }
    }
}
