using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
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
        public List<Behaviour> Behaviours { get; private set; } = new();

        private readonly IScenarioRepository m_ScenarioRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly ILeaderboard m_Leaderboard;
        private readonly IFileReader m_Reader;
        private readonly StorageId m_StorageId;

        public ForgottenDepthsManager(
            IScenarioRepository scenarioRepository, IBehaviourRepository behaviourRepository, ILeaderboard leaderboard,
            IFileReader reader, StorageId storageId)
        {
            m_ScenarioRepository = scenarioRepository;
            m_BehaviourRepository = behaviourRepository;
            m_Leaderboard = leaderboard;
            m_Reader = reader;
            m_StorageId = storageId;
        }

        public void Initialize()
        {
            var data = new ForgottenDepthsSaveData();

            try
            {
                data = m_Reader.Read<ForgottenDepthsSaveData>(GetDataPath()) ?? new ForgottenDepthsSaveData();
            }
            catch (Exception exception)
            {
                // ignored
            }

            Depth = Mathf.Max(data.Depth, 1);

            if (data.Behaviours.Count > 0)
            {
                Behaviours = data.Behaviours.Select(id => m_BehaviourRepository.Find(id)).ToList();
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
            m_Leaderboard.UpdateScore(Depth);

            ReRollBehaviours();
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            if (!scenario.IsDepths)
            {
                return;
            }

            m_Leaderboard.UpdateScore(Depth);
            Depth += 1;

            ReRollBehaviours();
        }

        private void OnApplicationQuitting()
        {
            var data = new ForgottenDepthsSaveData();
            data.Depth = Depth;
            data.Behaviours = Behaviours.Select(x => x.Id).ToList();
            m_Reader.Write(data, GetDataPath());
        }

        public void LaunchScenario()
        {
            ScreenFade.Instance.To(() =>
            {
                var scenario = m_ScenarioRepository.Find(GetScenarioId());

                foreach (var episode in scenario.Episodes)
                {
                    foreach (var entity in episode.Scene.Entities.All().Where(entity => entity.IsEnemyOfPlayer()))
                    {
                        entity.GetComponent<UnitComponent>().Level = Mathf.Max(1, GetMonsterLevel() + Rng.Range(-1, 1));

                        foreach (var behaviour in Behaviours)
                        {
                            entity.GetComponent<BehavioursComponent>().ApplyStack(behaviour, entity);
                        }

                        entity.GetComponent<HealthComponent>().Restore();
                    }
                }

                Game.Instance.ToScenario(scenario);
            });
        }

        public int GetMonsterLevel()
        {
            return Depth * 5;
        }

        private string GetDataPath()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/depths.save";
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
            var common = m_BehaviourRepository.Find(x => x.IsAncient && x.RarityId == Constants.c_ItemRarityIdCommon).Random(commonCount);
            Behaviours.AddRange(common);

            var magicCount = Mathf.Clamp(Depth / 10, 0, 5);
            var magic = m_BehaviourRepository.Find(x => x.IsAncient && x.RarityId == Constants.c_ItemRarityIdMagic).Random(magicCount);
            Behaviours.AddRange(magic);

            var rareCount = Mathf.Clamp(Depth / 20, 0, 3);
            var rare = m_BehaviourRepository.Find(x => x.IsAncient && x.RarityId == Constants.c_ItemRarityIdRare).Random(rareCount);
            Behaviours.AddRange(rare);
        }
    }
}