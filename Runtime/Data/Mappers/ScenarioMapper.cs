using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class ScenarioMapper : Mapper<ScenarioData, Scenario>
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly EpisodeMapper m_EpisodeMapper;

        public ScenarioMapper(IItemRepository itemRepository, IBehaviourRepository behaviourRepository, EpisodeMapper episodeMapper)
        {
            m_ItemRepository = itemRepository;
            m_BehaviourRepository = behaviourRepository;
            m_EpisodeMapper = episodeMapper;
        }

        public override ScenarioData ToData(Scenario target)
        {
            throw new NotImplementedException();
        }

        public override Scenario ToEntity(ScenarioData data)
        {
            var guaranteedRewards = new List<Item>();
            var choosableRewards = new List<Item>();

            var episodes = data.Episodes.Select(m_EpisodeMapper.ToEntity).ToList();

            SetMonsterLevel(data, episodes, 1);
            AddMonsterAffixes(episodes);

            return new Scenario(data, episodes, guaranteedRewards, choosableRewards);
        }

        private void SetMonsterLevel(ScenarioData data, List<Episode> episodes, int characterLevel)
        {
            foreach (var episode in episodes)
            {
                foreach (var entity in episode.Scene.Entities.All())
                {
                    if (entity == Game.Instance.Character.Entity)
                    {
                        continue;
                    }

                    var monsterLevel = characterLevel;

                    if (data.MinMonsterLevel > 0)
                    {
                        monsterLevel = Math.Max(data.MinMonsterLevel, monsterLevel);
                    }

                    if (data.MaxMonsterLevel > 0)
                    {
                        monsterLevel = Math.Min(data.MaxMonsterLevel, monsterLevel);
                    }

                    if (monsterLevel > 20)
                    {
                        monsterLevel += Rng.Range(-2, 2);
                    }

                    if (data.IsAscension)
                    {
                        var episodeIndex = episodes.IndexOf(episode);
                        var episodeNumber = episodeIndex + 1;

                        monsterLevel += episodeIndex * TowerManager.c_MonsterLevelGrowthPerEpisode +
                                        episodeNumber / TowerManager.c_BossEpisodeNumber * TowerManager.c_MonsterLevelGrowthPerBoss;
                    }

                    entity.GetComponent<UnitComponent>().Level = monsterLevel;
                    entity.GetComponent<HealthComponent>().Restore();
                }
            }
        }

        private void AddMonsterAffixes(List<Episode> episodes)
        {
            foreach (var episode in episodes)
            {
                foreach (var entity in GetEntities(episode))
                {
                    var unit = entity.GetComponent<UnitComponent>();

                    if (unit.Level < 30)
                    {
                        continue;
                    }

                    var behaviours = entity.GetComponent<BehavioursComponent>();
                    var affixes = m_BehaviourRepository.Random(b => b.Flags.HasFlag(BehaviourFlags.MonsterAffix), Mathf.Clamp(unit.Level / 30, 1, 3));

                    foreach (var affix in affixes)
                    {
                        behaviours.ApplyAllStacks(affix, entity);
                    }
                }
            }
        }

        private static List<GameObject> GetEntities(Episode episode)
        {
            var chosen = new List<GameObject>();
            var entities = episode.Scene.Entities.All();

            var boss = entities.FirstOrDefault(e => e.IsBoss());

            if (boss != null)
            {
                chosen.Add(boss);
                return chosen;
            }

            var modifiedCount = entities.Count / 3;

            if (modifiedCount < 1)
            {
                return chosen;
            }

            return entities.Where(e => !e.IsAllyOfPlayer() && !e.IsStructure()).Random(modifiedCount).ToList();
        }
    }
}