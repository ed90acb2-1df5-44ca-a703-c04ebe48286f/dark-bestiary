using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Controllers
{
    public class TargetFrameViewController : ViewController<ITargetFrameView>
    {
        private HealthComponent health;
        private BehavioursComponent behaviours;

        private HealthComponent killing;

        public TargetFrameViewController(ITargetFrameView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.UnsummonButtonClicked += OnUnsummonButtonClicked;

            SelectionManager.Instance.EnemySelected += OnEntitySelected;
            SelectionManager.Instance.EnemyDeselected += OnEntityDeselected;
        }

        protected override void OnTerminate()
        {
            Cleanup();

            View.UnsummonButtonClicked -= OnUnsummonButtonClicked;

            SelectionManager.Instance.EnemySelected -= OnEntitySelected;
            SelectionManager.Instance.EnemyDeselected -= OnEntityDeselected;
        }

        private void OnUnsummonButtonClicked()
        {
            this.killing = this.health;

            ConfirmationWindow.Instance.Cancelled += OnDeleteCancelled;
            ConfirmationWindow.Instance.Confirmed += OnDeleteConfirmed;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_confirm_unsummon_x").ToString(this.killing.GetComponent<UnitComponent>().Name),
                I18N.Instance.Get("ui_unsummon")
            );
        }

        private void OnDeleteConfirmed()
        {
            if (this.killing == null)
            {
                return;
            }

            if (!this.killing.gameObject.GetComponent<BehavioursComponent>().IsMindControlled)
            {
                this.killing.Kill(this.killing.gameObject);
            }

            OnDeleteCancelled();
        }

        private void OnDeleteCancelled()
        {
            ConfirmationWindow.Instance.Cancelled -= OnDeleteCancelled;
            ConfirmationWindow.Instance.Confirmed -= OnDeleteConfirmed;

            this.killing = null;
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
            this.health = entity.GetComponent<HealthComponent>();
            this.health.HealthChanged += OnHealthChanged;
            this.health.ShieldChanged += OnShieldChanged;

            this.behaviours = entity.GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;
            this.behaviours.BehaviourRemoved += OnBehaviourRemoved;

            var unit = entity.GetComponent<UnitComponent>();

            View.SetKillButtonActive(entity.IsSummoned() && entity.IsAllyOfPlayer());
            View.ChangeNameText(unit.Name + $" ({I18N.Instance.Get("ui_level")} {unit.Level})", entity.IsEnemyOfPlayer());
            View.ChangeChallengeRatingText(StringUtils.ToRomanNumeral(unit.ChallengeRating));
            View.ClearBehaviours();

            foreach (var behaviour in this.behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            View.ClearAffixes();
            View.CreateAffixes(this.behaviours.Behaviours.Where(b => b.IsMonsterAffix).ToList());

            OnHealthChanged(this.health);
        }

        private void Cleanup()
        {
            if (this.health != null)
            {
                this.health.HealthChanged -= OnHealthChanged;
                this.health.ShieldChanged -= OnShieldChanged;
            }

            if (this.behaviours != null)
            {
                this.behaviours.BehaviourApplied -= OnBehaviourApplied;
                this.behaviours.BehaviourRemoved -= OnBehaviourRemoved;
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
            View.SetPoisoned(this.behaviours.IsPoisoned);

            if (!behaviour.IsHidden)
            {
                View.AddBehaviour(behaviour);
            }
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            View.SetPoisoned(this.behaviours.IsPoisoned);

            if (!behaviour.IsHidden)
            {
                View.RemoveBehaviour(behaviour);
            }
        }
    }
}