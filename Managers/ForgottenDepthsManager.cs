using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Leaderboards;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Zenject;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Managers
{
    public class ForgottenDepthsManager : IInitializable
    {
        public static ForgottenDepthsManager Instance { get; private set; }

        public int Depth { get; private set; } = 1;
        public List<Behaviour> Behaviours { get; private set; } = new List<Behaviour>();

        private readonly IScenarioRepository scenarioRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly ILeaderboard leaderboard;
        private readonly IFileReader reader;
        private readonly StorageId storageId;

        public ForgottenDepthsManager(
            IScenarioRepository scenarioRepository, IBehaviourRepository behaviourRepository, ILeaderboard leaderboard,
            IFileReader reader, StorageId storageId)
        {
            this.scenarioRepository = scenarioRepository;
            this.behaviourRepository = behaviourRepository;
            this.leaderboard = leaderboard;
            this.reader = reader;
            this.storageId = storageId;
        }

        public void Initialize()
        {
            var data = new ForgottenDepthsSaveData();

            try
            {
                data = this.reader.Read<ForgottenDepthsSaveData>(GetDataPath()) ?? new ForgottenDepthsSaveData();
            }
            catch (Exception exception)
            {
                // ignored
            }

            Depth = Mathf.Max(data.Depth, 1);

            if (data.Behaviours.Count > 0)
            {
                Behaviours = data.Behaviours.Select(id => this.behaviourRepository.Find(id)).ToList();
            }
            else
            {
                ReRollBehaviours();
            }

            Application.quitting += OnApplicationQuitting;

            Scenario.AnyScenarioCompleted += OnAnyScenarioCompleted;
            Scenario.AnyScenarioFailed += OnAnyScenarioFailed;

            Instance = this;
        }

        private void OnAnyScenarioFailed(Scenario scenario)
        {
            if (!scenario.IsDepths)
            {
                return;
            }

            Depth = Mathf.Max(1, Depth - 1);
            this.leaderboard.UpdateScore(Depth);

            ReRollBehaviours();
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            if (!scenario.IsDepths)
            {
                return;
            }

            this.leaderboard.UpdateScore(Depth);
            Depth += 1;

            ReRollBehaviours();
        }

        private void OnApplicationQuitting()
        {
            var data = new ForgottenDepthsSaveData();
            data.Depth = Depth;
            data.Behaviours = Behaviours.Select(x => x.Id).ToList();
            this.reader.Write(data, GetDataPath());
        }

        public void LaunchScenario()
        {
            ScreenFade.Instance.To(() =>
                {
                    var scenario = this.scenarioRepository.Find(GetScenarioId());

                    foreach (var episode in scenario.Episodes)
                    {
                        foreach (var entity in episode.Scene.Entities.All().Where(entity => entity.IsEnemyOfPlayer()))
                        {
                            entity.GetComponent<UnitComponent>().Level = Mathf.Max(1, GetMonsterLevel() + RNG.Range(-1, 1));

                            foreach (var behaviour in Behaviours)
                            {
                                entity.GetComponent<BehavioursComponent>().ApplyStack(behaviour, entity);
                            }

                            entity.GetComponent<HealthComponent>().Restore();
                        }
                    }

                    Game.Instance.SwitchState(() => new ScenarioGameState(scenario, CharacterManager.Instance.Character), true);
                }
            );
        }

        public int GetMonsterLevel()
        {
            return Depth * 5;
        }

        private string GetDataPath()
        {
            return Environment.PersistentDataPath + $"/{this.storageId}/depths.save";
        }

        private int GetScenarioId()
        {
            if (Depth.InRange(0, 5))
            {
                return 78;
            }

            if (Depth.InRange(5, 10))
            {
                return 79;
            }

            if (Depth.InRange(10, 15))
            {
                return 80;
            }

            if (Depth.InRange(15, 20))
            {
                return 81;
            }

            return 82;
        }

        private void ReRollBehaviours()
        {
            Behaviours.Clear();

            var commonCount = Mathf.Clamp(Depth / 5 + 1, 1, 10);
            var common = this.behaviourRepository.Find(x => x.IsAncient && x.RarityId == Constants.ItemRarityIdCommon).Random(commonCount);
            Behaviours.AddRange(common);

            var magicCount = Mathf.Clamp(Depth / 10, 0, 5);
            var magic = this.behaviourRepository.Find(x => x.IsAncient && x.RarityId == Constants.ItemRarityIdMagic).Random(magicCount);
            Behaviours.AddRange(magic);

            var rareCount = Mathf.Clamp(Depth / 20, 0, 3);
            var rare = this.behaviourRepository.Find(x => x.IsAncient && x.RarityId == Constants.ItemRarityIdRare).Random(rareCount);
            Behaviours.AddRange(rare);
        }
    }
}