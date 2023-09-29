using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Randomization;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class EpisodeMapper : Mapper<EpisodeData, Episode>
    {
        private readonly ISceneRepository m_SceneRepository;
        private readonly IUnitDataRepository m_UnitDataRepository;
        private readonly IUnitRepository m_UnitRepository;
        private readonly IUnitGroupDataRepository m_UnitGroupDataRepository;

        public EpisodeMapper(
            ISceneRepository sceneRepository,
            IUnitDataRepository unitDataRepository,
            IUnitRepository unitRepository,
            IUnitGroupDataRepository unitGroupDataRepository)
        {
            m_SceneRepository = sceneRepository;
            m_UnitDataRepository = unitDataRepository;
            m_UnitRepository = unitRepository;
            m_UnitGroupDataRepository = unitGroupDataRepository;
        }

        public override EpisodeData ToData(Episode entity)
        {
            throw new NotImplementedException();
        }

        public override Episode ToEntity(EpisodeData data)
        {
            var scene = GetScene(data);
            scene.Entities.Add(GetEntities(data));
            scene.Entities.Add(Game.Instance.Character.Entity);
            scene.Hide();

            return new Episode(scene, GetEncounter(data.Encounter));
        }

        private Scene GetScene(EpisodeData data)
        {
            return data.Scenes.Count > 0
                ? m_SceneRepository.FindOrFail(data.Scenes.Random())
                : m_SceneRepository.Random(
                    s => !s.HasNoExit &&
                         !s.IsScripted && (s.Environment.Id == data.EnvironmentId || data.EnvironmentId == 0));
        }

        private Encounter GetEncounter(EncounterData data)
        {
            Encounter encounter;

            switch (data.Type)
            {
                case EncounterType.Combat:
                    encounter = Container.Instance.Instantiate<CombatEncounter>();
                    break;
                case EncounterType.Treasure:
                    encounter = Container.Instance.Instantiate<TreasureEncounter>();
                    break;
                case EncounterType.Empty:
                    encounter = new EmptyEncounter();
                    break;
                default:
                    encounter = new EmptyEncounter();
                    break;
            }

            return encounter;
        }

        private List<GameObject> GetEntities(EpisodeData data)
        {
            List<GameObject> entities;

            switch (data.Encounter.UnitSourceType)
            {
                case EncounterUnitSourceType.Table:
                    entities = FromTable(data);
                    break;
                case EncounterUnitSourceType.Environment:
                    entities = FromEnvironment(data);
                    break;
                case EncounterUnitSourceType.UnitGroup:
                    entities = FromUnitGroup(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var entity in entities)
            {
                entity.transform.position = new Vector3(-100, 0, 0);
                entity.gameObject.SetActive(false);

                var unit = entity.GetComponent<UnitComponent>();
                unit.Owner = Owner.Hostile;
                unit.TeamId = 2;
            }

            return entities;
        }

        private List<GameObject> FromTable(EpisodeData data)
        {
            var table = new RandomTable(data.Encounter.UnitTable.Count);

            foreach (var episodeUnitData in data.Encounter.UnitTable.Units)
            {
                var value = new RandomTableNumberEntry(
                    episodeUnitData.UnitId,
                    new RandomTableEntryParameters(
                        episodeUnitData.Probability,
                        episodeUnitData.IsUnique,
                        episodeUnitData.IsGuaranteed,
                        episodeUnitData.IsEnabled
                    )
                );

                table.AddEntry(value);
            }

            return table.Evaluate()
                .OfType<RandomTableNumberEntry>()
                .Select(value => value.Value)
                .Select(unitId => m_UnitRepository.Find(unitId))
                .ToList();
        }

        private List<GameObject> FromEnvironment(EpisodeData data)
        {
            var maxChallengeRating = data.Encounter.UnitGroupChallengeRating < 8 ? 3 : 10;

            var units = m_UnitDataRepository.FindAll()
                .Where(unit => data.Encounter.UnitGroupEnvironmentId == 0 &&
                               unit.ChallengeRating <= maxChallengeRating &&
                               unit.Environment.Id != Constants.c_EnvironmentTowerId ||
                               unit.Environment.Id == data.Encounter.UnitGroupEnvironmentId)
                .Where(unit => !unit.Flags.HasFlag(UnitFlags.Dummy) &&
                               !unit.Flags.HasFlag(UnitFlags.Boss) &&
                               !unit.Flags.HasFlag(UnitFlags.Summoned) &&
                               !unit.Flags.HasFlag(UnitFlags.CampaignOnly) &&
                               !unit.Flags.HasFlag(UnitFlags.Playable))
                .ToList();

            var result = new List<UnitData>();
            var iterations = 0;

            while (true)
            {
                var possible = units
                    .Where(u1 => u1.ChallengeRating <= data.Encounter.UnitGroupChallengeRating - result.Sum(u2 => u2.ChallengeRating))
                    .ToList();

                if (possible.Count == 0)
                {
                    break;
                }

                result.Add(possible.Random());

                if (++iterations <= 1000)
                {
                    continue;
                }

                Debug.LogWarning("Maximum iterations hit!");
                break;
            }

            return m_UnitRepository.Find(result.Select(unit => unit.Id).ToList());
        }

        private List<GameObject> FromUnitGroup(EpisodeData data)
        {
            var units = new List<GameObject>();

            var unitGroup = m_UnitGroupDataRepository.FindOrFail(data.Encounter.UnitGroups.Random());

            foreach (var unitId in unitGroup.Units)
            {
                units.Add(m_UnitRepository.Find(unitId));
            }

            return units;
        }
    }
}