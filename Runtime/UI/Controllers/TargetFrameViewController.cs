using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Controllers
{
    public class TargetFrameViewController : ViewController<ITargetFrameView>
    {
        private HealthComponent m_Health;
        private BehavioursComponent m_Behaviours;

        public TargetFrameViewController(ITargetFrameView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            SelectionManager.Instance.EnemySelected += OnEntitySelected;
            SelectionManager.Instance.EnemyDeselected += OnEntityDeselected;
        }

        protected override void OnTerminate()
        {
            Cleanup();

            SelectionManager.Instance.EnemySelected -= OnEntitySelected;
            SelectionManager.Instance.EnemyDeselected -= OnEntityDeselected;
        }

        private void OnEntitySelected(GameObject entity)
        {
            Cleanup();
            Setup(entity);

            View.Show();
        }

        private void OnEntityDeselected(GameObject entity)
        {
            View.Hide();
        }

        private void Setup(GameObject entity)
        {
            m_Health = entity.GetComponent<HealthComponent>();
            m_Health.HealthChanged += OnHealthChanged;
            m_Health.ShieldChanged += OnShieldChanged;

            m_Behaviours = entity.GetComponent<BehavioursComponent>();
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;
            m_Behaviours.BehaviourRemoved += OnBehaviourRemoved;

            var unit = entity.GetComponent<UnitComponent>();

            View.ChangeNameText(unit.Name + $" ({I18N.Instance.Get("ui_level")} {unit.Level})", entity.IsEnemyOfPlayer());
            View.ChangeChallengeRatingText(StringUtils.ToRomanNumeral(unit.ChallengeRating));
            View.ClearBehaviours();

            foreach (var behaviour in m_Behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            View.ClearAffixes();
            View.CreateAffixes(m_Behaviours.Behaviours.Where(b => b.IsMonsterAffix).ToList());

            OnHealthChanged(m_Health);
        }

        private void Cleanup()
        {
            if (m_Health != null)
            {
                m_Health.HealthChanged -= OnHealthChanged;
                m_Health.ShieldChanged -= OnShieldChanged;
            }

            if (m_Behaviours != null)
            {
                m_Behaviours.BehaviourApplied -= OnBehaviourApplied;
                m_Behaviours.BehaviourRemoved -= OnBehaviourRemoved;
            }
        }

        private void OnHealthChanged(HealthComponent health)
        {
            View.RefreshHealth(health.Health, health.Shield, health.HealthAndShieldMax);
        }

        private void OnShieldChanged(HealthComponent health)
        {
            View.RefreshHealth(health.Health, health.Shield, health.HealthAndShieldMax);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            View.SetPoisoned(m_Behaviours.IsPoisoned);

            if (!behaviour.IsHidden)
            {
                View.AddBehaviour(behaviour);
            }
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            View.SetPoisoned(m_Behaviours.IsPoisoned);

            if (!behaviour.IsHidden)
            {
                View.RemoveBehaviour(behaviour);
            }
        }
    }
}