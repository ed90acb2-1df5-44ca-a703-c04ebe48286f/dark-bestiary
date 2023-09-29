using System;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanel : MonoBehaviour
    {
        public event Action? CompleteButtonClicked;
        public event Action<VictoryPanelReward>? ChooseReward;

        [SerializeField]
        private VictoryPanelExperience m_Experience = null!;

        [SerializeField]
        private VictoryPanelLoot m_Loot = null!;

        [SerializeField]
        private Transform m_Rewards = null!;

        [SerializeField]
        private VictoryPanelReward m_RewardPrefab = null!;

        [SerializeField]
        private Transform m_RewardContainer = null!;

        [SerializeField]
        private VictoryPanelRelic m_RelicPrefab = null!;

        [SerializeField]
        private Transform m_RelicContainer = null!;

        [SerializeField]
        private Interactable m_CompleteButton = null!;

        private VictoryPanelReward? m_ChosenReward;

        public void Initialize(Scenario scenario)
        {
            AudioManager.Instance.PlayVictory();

            var scenarioLoot = scenario.GetLoot();

            m_Loot.Initialize();

            if (scenarioLoot.Items.Count > 0)
            {
                m_Loot.Simulate(scenarioLoot.Items);
            }
            else
            {
                m_Loot.gameObject.SetActive(false);
            }

            var character = Game.Instance.Character.Entity;

            m_Experience.Construct(character.GetComponent<ExperienceComponent>().Experience.Snapshot);

            var reliquary = character.GetComponent<ReliquaryComponent>();

            foreach (var relic in reliquary.GetActiveRelics())
            {
                Instantiate(m_RelicPrefab, m_RelicContainer).Construct(relic);
            }

            Timer.Instance.Wait(1, () =>
            {
                m_Experience.Simulate();
            });

            m_CompleteButton.PointerClick += OnCompleteButtonPointerClick;

            CreateRewards(scenario);
        }

        public void Terminate()
        {
            m_Loot.Terminate();
        }

        private void CreateRewards(Scenario scenario)
        {
            if (scenario.ChoosableRewards.Count == 0 && scenario.GuaranteedRewards.Count == 0)
            {
                m_Rewards.gameObject.SetActive(false);
                return;
            }

            foreach (var item in scenario.GuaranteedRewards)
            {
                var reward = Instantiate(m_RewardPrefab, m_RewardContainer);
                reward.Clicked += OnRewardClicked;
                reward.Construct(item, false);
            }

            foreach (var item in scenario.ChoosableRewards)
            {
                var reward = Instantiate(m_RewardPrefab, m_RewardContainer);
                reward.Clicked += OnRewardClicked;
                reward.Construct(item, true);
            }
        }

        private void OnRewardClicked(VictoryPanelReward reward)
        {
            if (!reward.IsChoosable)
            {
                return;
            }

            if (m_ChosenReward != null)
            {
                m_ChosenReward.Deselect();
            }

            m_ChosenReward = reward;
            m_ChosenReward.Select();

            ChooseReward?.Invoke(reward);
        }

        private void OnCompleteButtonPointerClick()
        {
            try
            {
                CompleteButtonClicked?.Invoke();
                m_CompleteButton.Active = false;
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}