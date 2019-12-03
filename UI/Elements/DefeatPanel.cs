using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class DefeatPanel : MonoBehaviour
    {
        public event Payload ReturnToTown;

        [SerializeField] private VictoryPanelExperience experience;
        [SerializeField] private VictoryPanelLoot loot;
        [SerializeField] private VictoryPanelSummary summary;
        [SerializeField] private Interactable completeButton;

        public void Initialize(Scenario scenario)
        {
            AudioManager.Instance.PlayDefeat();

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

            this.experience.Construct(
                CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience);

            Timer.Instance.Wait(1, () => this.experience.Simulate());

            this.completeButton.PointerUp += OnCompleteButtonPointerUp;
            this.summary.Construct(scenario.GetSummary());
        }

        public void Terminate()
        {
            this.loot.Terminate();
        }

        private void OnCompleteButtonPointerUp()
        {
            ReturnToTown?.Invoke();
            this.completeButton.Active = false;
        }
    }
}