using System.Linq;
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
        [SerializeField] private Interactable completeButton;
        [SerializeField] private DeathRecapRow recapRowPrefab;
        [SerializeField] private Transform recapRowContainer;

        public void Initialize(Scenario scenario)
        {
            AudioManager.Instance.PlayDefeat();

            CreateRecap(scenario.GetDeathRecap());

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

            this.experience.Construct(character.GetComponent<ExperienceComponent>().Experience, scenarioLoot.SkillPoints);

            Timer.Instance.Wait(1, () => this.experience.Simulate());

            this.completeButton.PointerClick += OnCompleteButtonPointerClick;
        }

        public void Terminate()
        {
            this.loot.Terminate();
        }

        private void CreateRecap(DeathRecapRecorder recap)
        {
            foreach (var record in recap.Records.Reverse().Take(20))
            {
                if (record.IsDamage)
                {
                    Instantiate(this.recapRowPrefab, this.recapRowContainer).Construct(record.Damage, record.Source);
                    continue;
                }

                Instantiate(this.recapRowPrefab, this.recapRowContainer).Construct(record.Healing, record.Source);
            }
        }

        private void OnCompleteButtonPointerClick()
        {
            ReturnToTown?.Invoke();
            this.completeButton.Active = false;
        }
    }
}