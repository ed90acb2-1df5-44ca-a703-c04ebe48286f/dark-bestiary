using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanel : MonoBehaviour
    {
        public event Payload Complete;
        public event Payload<VictoryPanelReward> ChooseReward;

        [SerializeField] private VictoryPanelExperience experience;
        [SerializeField] private VictoryPanelLoot loot;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Transform rewards;
        [SerializeField] private VictoryPanelReward rewardPrefab;
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private VictoryPanelRelic relicPrefab;
        [SerializeField] private Transform relicContainer;
        [SerializeField] private Interactable completeButton;

        private VictoryPanelReward chosenReward;

        public void Initialize(Scenario scenario)
        {
            AudioManager.Instance.PlayVictory();

            var scenarioLoot = scenario.GetLoot();

            this.loot.Initialize();

            if (scenarioLoot.Items.Count > 0)
            {
                this.loot.Simulate(scenarioLoot.Items);
            }
            else
            {
                this.loot.gameObject.SetActive(false);
            }

            var character = CharacterManager.Instance.Character.Entity;

            this.experience.Construct(character.GetComponent<ExperienceComponent>().Experience.Snapshot);

            var reliquary = character.GetComponent<ReliquaryComponent>();

            foreach (var relic in reliquary.GetActiveRelics())
            {
                Instantiate(this.relicPrefab, this.relicContainer).Construct(relic);
            }

            Timer.Instance.Wait(1, () =>
            {
                this.experience.Simulate();
            });

            this.completeButton.PointerUp += OnCompleteButtonPointerUp;
            this.text.text = scenario.CompleteText;

            CreateRewards(scenario);
        }

        public void Terminate()
        {
            this.loot.Terminate();
        }

        private void CreateRewards(Scenario scenario)
        {
            if (scenario.ChoosableRewards.Count == 0 && scenario.GuaranteedRewards.Count == 0)
            {
                this.rewards.gameObject.SetActive(false);
                return;
            }

            foreach (var item in scenario.GuaranteedRewards)
            {
                var reward = Instantiate(this.rewardPrefab, this.rewardContainer);
                reward.Clicked += OnRewardClicked;
                reward.Initialize(item, false);
            }

            foreach (var item in scenario.ChoosableRewards)
            {
                var reward = Instantiate(this.rewardPrefab, this.rewardContainer);
                reward.Clicked += OnRewardClicked;
                reward.Initialize(item, true);
            }
        }

        private void OnRewardClicked(VictoryPanelReward reward)
        {
            if (!reward.IsChoosable)
            {
                return;
            }

            if (this.chosenReward != null)
            {
                this.chosenReward.Deselect();
            }

            this.chosenReward = reward;
            this.chosenReward.Select();

            ChooseReward?.Invoke(reward);
        }

        private void OnCompleteButtonPointerUp()
        {
            try
            {
                Complete?.Invoke();
                this.completeButton.Active = false;
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }
    }
}