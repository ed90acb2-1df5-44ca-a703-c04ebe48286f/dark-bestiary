using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class DefeatPanel : MonoBehaviour
    {
        public event Action? CompleteButtonClicked;

        [SerializeField]
        private VictoryPanelExperience m_Experience = null!;

        [SerializeField]
        private VictoryPanelLoot m_Loot = null!;

        [SerializeField]
        private Interactable m_CompleteButton = null!;

        [SerializeField]
        private DeathRecapRow m_RecapRowPrefab = null!;

        [SerializeField]
        private Transform m_RecapRowContainer = null!;

        public void Initialize(Scenario scenario)
        {
            AudioManager.Instance.PlayDefeat();

            CreateRecap(scenario.GetDeathRecap());

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

            m_Experience.Construct(character.GetComponent<ExperienceComponent>().Experience);

            Timer.Instance.Wait(1, () => m_Experience.Simulate());

            m_CompleteButton.PointerClick += OnCompleteButtonPointerClick;
        }

        public void Terminate()
        {
            m_Loot.Terminate();
        }

        private void CreateRecap(DeathRecapRecorder recap)
        {
            foreach (var record in recap.Records.Reverse().Take(20))
            {
                if (record.IsDamage)
                {
                    Instantiate(m_RecapRowPrefab, m_RecapRowContainer).Construct(record.Damage, record.Source);
                    continue;
                }

                Instantiate(m_RecapRowPrefab, m_RecapRowContainer).Construct(record.Healing, record.Source);
            }
        }

        private void OnCompleteButtonPointerClick()
        {
            CompleteButtonClicked?.Invoke();
            m_CompleteButton.Active = false;
        }
    }
}