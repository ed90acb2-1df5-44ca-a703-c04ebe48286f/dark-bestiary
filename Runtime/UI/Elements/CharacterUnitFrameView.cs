using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.UI.Views;
using DarkBestiary.UI.Views.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class CharacterUnitFrameView : View, ICharacterUnitFrameView
    {
        [SerializeField] private Image m_HealthImage;
        [SerializeField] private TextMeshProUGUI m_HealthText;
        [SerializeField] private Image m_ExperienceImage;
        [SerializeField] private TextMeshProUGUI m_ExperienceText;
        [SerializeField] private TextMeshProUGUI m_LevelText;
        [SerializeField] private BehaviourView m_BehaviourPrefab;
        [SerializeField] private Transform m_BehaviourContainer;

        private readonly List<BehaviourView> m_BehaviourViews = new();

        private HealthComponent m_Health;
        private ExperienceComponent m_Experience;
        private BehavioursComponent m_Behaviours;

        public void Construct(GameObject entity)
        {
            m_Health = entity.GetComponent<HealthComponent>();
            m_Health.HealthChanged += OnHealthChanged;

            m_Experience = entity.GetComponent<ExperienceComponent>();
            m_Experience.Experience.Changed += OnExperienceChanged;

            m_Behaviours = entity.GetComponent<BehavioursComponent>();
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;
            m_Behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in m_Behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            OnExperienceChanged(m_Experience.Experience);
            OnHealthChanged(m_Health);
        }

        protected override void OnTerminate()
        {
            m_Health.HealthChanged -= OnHealthChanged;

            m_Experience.Experience.Changed -= OnExperienceChanged;

            m_Behaviours.BehaviourApplied -= OnBehaviourApplied;
            m_Behaviours.BehaviourRemoved -= OnBehaviourRemoved;

            foreach (var behaviourView in m_BehaviourViews)
            {
                behaviourView.Terminate();
            }
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour.IsHidden)
            {
                return;
            }

            var behaviourView = Instantiate(m_BehaviourPrefab, m_BehaviourContainer);
            behaviourView.Initialize(behaviour);
            m_BehaviourViews.Add(behaviourView);
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.IsHidden)
            {
                return;
            }

            var behaviourView = m_BehaviourViews.FirstOrDefault(view => view.Behaviour.Id == behaviour.Id);

            if (behaviourView == null)
            {
                return;
            }

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
            m_BehaviourViews.Remove(behaviourView);
        }

        private void OnHealthChanged(HealthComponent health)
        {
            m_HealthText.text = $"{health.Health:N0}/{health.HealthMax:N0}";
            m_HealthImage.fillAmount = health.Health / health.HealthMax;
        }

        private void OnExperienceChanged(Experience experience)
        {
            m_ExperienceImage.fillAmount = experience.GetObtainedFraction();
            m_LevelText.text = experience.Level.ToString();
            m_ExperienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
        }
    }
}