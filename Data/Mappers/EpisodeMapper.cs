using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Randomization;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class EpisodeMapper : Mapper<EpisodeData, Episode>
    {
        private readonly ISceneRepository sceneRepository;
        private readonly IUnitDataRepository unitDataRepository;
        private readonly IUnitRepository unitRepository;
        private readonly IUnitGroupDataRepository unitGroupDataRepository;
        private readonly IPhraseDataRepository phraseDataRepository;

        public EpisodeMapper(
            ISceneRepository sceneRepository,
            IUnitDataRepository unitDataRepository,
            IUnitRepository unitRepository,
            IUnitGroupDataRepository unitGroupDataRepository,
            IPhraseDataRepository phraseDataRepository)
        {
            this.sceneRepository = sceneRepository;
            this.unitDataRepository = unitDataRepository;
            this.unitRepository = unitRepository;
            this.unitGroupDataRepository = unitGroupDataRepository;
            this.phraseDataRepository = phraseDataRepository;
        }

        public override EpisodeData ToData(Episode entity)
        {
            throw new NotImplementedException();
        }

        public override Episode ToEntity(EpisodeData data)
        {
            var scene = GetScene(data);
            scene.Entities.Add(GetEntities(data.Encounter));
            scene.Entities.Add(CharacterManager.Instance.Character.Entity);
            scene.Hide();

            return new Episode(scene, GetEncounter(data.Encounter));
        }

        private Scene GetScene(EpisodeData data)
        {
            return data.Scenes.Count > 0
                ? this.sceneRepository.FindOrFail(data.Scenes.Random())
                : this.sceneRepository.Random(
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
                case EncounterType.Talk:
                    encounter = Container.Instance.Instantiate<TalkEncounter>(
                        new object[] {this.phraseDataRepository.Find(data.Phrases)});
                    break;
                case EncounterType.Empty:
                    encounter = new EmptyEncounter();
                    break;
                default:
                    encounter = new EmptyEncounter();
                    break;
            }

            encounter.StartPhrase = this.phraseDataRepository.Find(data.StartPhraseId);
            encounter.CompletePhrase = this.phraseDataRepository.Find(data.CompletePhraseId);

            return encounter;
        }

        private List<GameObject> GetEntities(EncounterData data)
        {
            List<GameObject> entities;

            switch (data.UnitSourceType)
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

        private List<GameObject> FromTable(EncounterData data)
        {
            var table = new RandomizerTable(data.UnitTable.Count);

            foreach (var episodeUnitData in data.UnitTable.Units)
            {
                var value = new RandomizerValue<int>(
                    episodeUnitData.UnitId,
                    episodeUnitData.Probability,
                    episodeUnitData.IsUnique,
                    episodeUnitData.IsGuaranteed,
                    episodeUnitData.IsEnabled
                );

                table.Add(value);
            }

            return table.Evaluate()
                .OfType<RandomizerValue<int>>()
                .Select(value => value.Value)
                .Select(unitId => this.unitRepository.Find(unitId))
                .ToList();
        }

        private List<GameObject> FromEnvironment(EncounterData data)
        {
            var units = this.unitDataRepository.FindAll()
                .Where(unit => data.UnitGroupEnvironmentId == 0 || unit.Environment.Id == data.UnitGroupEnvironmentId)
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
                    .Where(u1 => u1.ChallengeRating <= data.UnitGroupChallengeRating - result.Sum(u2 => u2.ChallengeRating))
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

            return this.unitRepository.Find(result.Select(unit => unit.Id).ToList());
        }

        private List<GameObject> FromUnitGroup(EncounterData data)
        {
            var units = new List<GameObject>();

            var unitGroup = this.unitGroupDataRepository.FindOrFail(data.UnitGroups.Random());

            foreach (var unitId in unitGroup.Units)
            {
                units.Add(this.unitRepository.Find(unitId));
            }

            return units;
        }
    }
}