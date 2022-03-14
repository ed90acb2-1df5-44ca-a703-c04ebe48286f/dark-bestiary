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
        [SerializeField] private Image healthImage;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image experienceImage;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private BehaviourView behaviourPrefab;
        [SerializeField] private Transform behaviourContainer;

        private readonly List<BehaviourView> behaviourViews = new List<BehaviourView>();

        private HealthComponent health;
        private ExperienceComponent experience;
        private BehavioursComponent behaviours;

        public void Construct(GameObject entity)
        {
            this.health = entity.GetComponent<HealthComponent>();
            this.health.HealthChanged += OnHealthChanged;

            this.experience = entity.GetComponent<ExperienceComponent>();
            this.experience.Experience.Changed += OnExperienceChanged;

            this.behaviours = entity.GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;
            this.behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in this.behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            OnExperienceChanged(this.experience.Experience);
            OnHealthChanged(this.health);
        }

        protected override void OnTerminate()
        {
            this.health.HealthChanged -= OnHealthChanged;

            this.experience.Experience.Changed -= OnExperienceChanged;

            this.behaviours.BehaviourApplied -= OnBehaviourApplied;
            this.behaviours.BehaviourRemoved -= OnBehaviourRemoved;

            foreach (var behaviourView in this.behaviourViews)
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

            var behaviourView = Instantiate(this.behaviourPrefab, this.behaviourContainer);
            behaviourView.Initialize(behaviour);
            this.behaviourViews.Add(behaviourView);
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.IsHidden)
            {
                return;
            }

            var behaviourView = this.behaviourViews.FirstOrDefault(view => view.Behaviour.Id == behaviour.Id);

            if (behaviourView == null)
            {
                return;
            }

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
            this.behaviourViews.Remove(behaviourView);
        }

        private void OnHealthChanged(HealthComponent health)
        {
            this.healthText.text = $"{health.Health:N0}/{health.HealthMax:N0}";
            this.healthImage.fillAmount = health.Health / health.HealthMax;
        }

        private void OnExperienceChanged(Experience experience)
        {
            this.experienceImage.fillAmount = experience.GetObtainedFraction();
            this.levelText.text = experience.Level.ToString();
            this.experienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
        }
    }
}