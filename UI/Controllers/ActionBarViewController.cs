using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Interaction;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Controllers
{
    public class ActionBarViewController : ViewController<IActionBarView>
    {
        private readonly Interactor interactor;

        private SpellbookComponent spellbook;
        private EquipmentComponent equipment;
        private MovementComponent movement;
        private ResourcesComponent resources;
        private BehavioursComponent behaviours;
        private ExperienceComponent experience;
        private HealthComponent health;
        private Skill castingSkill;

        public ActionBarViewController(IActionBarView view, Interactor interactor) : base(view)
        {
            this.interactor = interactor;
        }

        protected override void OnInitialize()
        {
            SelectionManager.Instance.AllySelected += OnAllySelected;

            PathDrawer.AnyPathDrawn += OnAnyPathDrawn;
            PathDrawer.AnyPathErased += OnAnyPathErased;

            CombatEncounter.AnyCombatTurnStarted += OnAnyCombatTurnStarted;
            CombatEncounter.AnyCombatTurnEnded += OnAnyCombatTurnEnded;

            View.EndTurnButtonClicked += OnEndTurnButtonClicked;
            View.SwapWeaponButtonClicked += OnSwapWeaponButtonClicked;
            View.SkillClicked += OnSkillClicked;
            View.SkillSelected += OnSkillSelected;
            View.SkillRemoved += OnSkillRemoved;
            View.SkillPlaced += OnSkillPlaced;

            this.interactor.StateEnter += OnInteractorStateEnter;
            this.interactor.StateExit += OnInteractorStateExit;
        }

        protected override void OnTerminate()
        {
            SelectionManager.Instance.AllySelected -= OnAllySelected;

            PathDrawer.AnyPathDrawn -= OnAnyPathDrawn;
            PathDrawer.AnyPathErased -= OnAnyPathErased;

            CombatEncounter.AnyCombatTurnStarted -= OnAnyCombatTurnStarted;
            CombatEncounter.AnyCombatTurnEnded -= OnAnyCombatTurnEnded;

            View.EndTurnButtonClicked -= OnEndTurnButtonClicked;
            View.SwapWeaponButtonClicked -= OnSwapWeaponButtonClicked;
            View.SkillClicked -= OnSkillClicked;
            View.SkillSelected -= OnSkillSelected;
            View.SkillRemoved -= OnSkillRemoved;
            View.SkillPlaced -= OnSkillPlaced;

            this.interactor.StateEnter -= OnInteractorStateEnter;
            this.interactor.StateExit -= OnInteractorStateExit;

            Cleanup();
        }

        private void Setup(GameObject entity)
        {
            this.movement = entity.GetComponent<MovementComponent>();
            this.movement.Started += OnMovementStarted;
            this.movement.Stopped += OnMovementStopped;

            this.health = entity.GetComponent<HealthComponent>();
            this.health.HealthChanged += OnHealthChanged;
            this.health.ShieldChanged += OnHealthChanged;
            OnHealthChanged(this.health);

            this.resources = entity.GetComponent<ResourcesComponent>();
            this.resources.ActionPointsChanged += OnActionPointsChanged;
            this.resources.RageChanged += OnRageChanged;

            OnActionPointsChanged(this.resources.Get(ResourceType.ActionPoint));
            OnRageChanged(this.resources.Get(ResourceType.Rage));

            this.behaviours = entity.GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;
            this.behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in this.behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            this.equipment = entity.GetComponent<EquipmentComponent>();

            this.spellbook = entity.GetComponent<SpellbookComponent>();
            this.spellbook.SkillCooldownStarted += OnSkillCooldownStarted;
            this.spellbook.SkillCooldownUpdated += OnSkillCooldownUpdated;
            this.spellbook.SkillCooldownFinished += OnSkillCooldownFinished;

            View.CreateSkills(this.spellbook.Slots);

            foreach (var slot in this.spellbook.Slots.Where(slot => slot.Skill.IsOnCooldown()))
            {
                View.StartSkillCooldown(slot.Skill);
            }

            UpdateButtonActiveState();
        }

        private void Cleanup()
        {
            View.RemoveSkills();
            View.RemoveBehaviours();

            if (this.behaviours != null)
            {
                this.behaviours.BehaviourApplied -= OnBehaviourApplied;
                this.behaviours.BehaviourRemoved -= OnBehaviourRemoved;
            }

            if (this.resources != null)
            {
                this.resources.ActionPointsChanged -= OnActionPointsChanged;
                this.resources.RageChanged -= OnRageChanged;
            }

            if (this.health != null)
            {
                this.health.HealthChanged -= OnHealthChanged;
                this.health.ShieldChanged -= OnHealthChanged;
            }

            if (this.movement != null)
            {
                this.movement.Started -= OnMovementStarted;
                this.movement.Stopped -= OnMovementStopped;
            }

            if (this.spellbook != null)
            {
                this.spellbook.SkillCooldownStarted -= OnSkillCooldownStarted;
                this.spellbook.SkillCooldownUpdated -= OnSkillCooldownUpdated;
                this.spellbook.SkillCooldownFinished -= OnSkillCooldownFinished;
            }
        }

        private void UpdateButtonActiveState()
        {
            var isMyTurn = CombatEncounter.Active == null || CombatEncounter.Active.IsEntityTurn(this.spellbook.gameObject);

            if (isMyTurn)
            {
                View.EnableEndTurnButton();
            }
            else
            {
                View.DisableEndTurnButton();
            }

            if (!(this.interactor.State is WaitState) && isMyTurn)
            {
                View.EnableSkills();
            }
            else
            {
                View.DisableSkills();
            }
        }

        private void OnSkillSelected(Skill skillA, Skill skillB)
        {
            try
            {
                this.spellbook.Replace(skillA, skillB);
                skillB.RunCooldown();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }

        private void OnSkillPlaced(SkillSlot slot, Skill skill)
        {
            try
            {
                this.spellbook.PlaceOnActionBar(slot, skill);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }

        private void OnSkillRemoved(Skill skill)
        {
            try
            {
                this.spellbook.RemoveFromActionBar(skill);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }

        private void OnHealthChanged(HealthComponent health)
        {
            View.UpdateHealth(health.Health, health.Shield, health.HealthAndShieldMax);
        }

        private void OnInteractorStateEnter(InteractorState state)
        {
            UpdateButtonActiveState();

            if (state is CastState castState)
            {
                View.ShowActionPointUsage(
                    (int) castState.Skill.GetCost().GetValueOrDefault(ResourceType.ActionPoint, 0));
                View.HighlightSkill(castState.Skill);
            }
        }

        private void OnInteractorStateExit(InteractorState state)
        {
            if (state is CastState castState)
            {
                this.castingSkill = null;
                View.UnhighlightSkill(castState.Skill);
                View.HideActionPointUsage();
            }
        }

        private void OnSkillClicked(Skill skill)
        {
            if (skill.Id == Skill.Empty.Id || skill.IsOnCooldown() || skill.IsDisabled())
            {
                return;
            }

            if (skill == this.castingSkill)
            {
                this.castingSkill = null;
                this.interactor.EnterSelectionState();
            }
            else
            {
                this.castingSkill = skill;
                this.interactor.EnterCastState(this.spellbook.gameObject, skill);
            }
        }

        private void OnAnyCombatTurnStarted(GameObject entity)
        {
            UpdateButtonActiveState();
        }

        private void OnAnyCombatTurnEnded(GameObject entity)
        {
            UpdateButtonActiveState();
        }

        private void OnAllySelected(GameObject entity)
        {
            if (this.spellbook != null && this.spellbook.gameObject == entity)
            {
                return;
            }

            Cleanup();
            Setup(entity);
        }

        private void OnSkillCooldownStarted(Skill skill)
        {
            View.StartSkillCooldown(skill);
        }

        private void OnSkillCooldownUpdated(Skill skill)
        {
            View.UpdateSkillCooldown(skill);
        }

        private void OnSkillCooldownFinished(Skill skill)
        {
            View.StopSkillCooldown(skill);
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            Timer.Instance.Wait(0.1f, UpdateButtonActiveState);
        }

        private void OnMovementStarted(MovementComponent movement)
        {
            UpdateButtonActiveState();
        }

        private void OnAnyPathDrawn(List<Vector3> path)
        {
            View.ShowActionPointUsage(this.movement.GetMovementCost(path.Count - 1));
        }

        private void OnAnyPathErased()
        {
            View.HideActionPointUsage();
        }

        private void OnRageChanged(Resource resource)
        {
            View.UpdateRageAmount(resource.Amount, resource.MaxAmount);
        }

        private void OnActionPointsChanged(Resource resource)
        {
            View.UpdateActionPointsAmount(resource.Amount, resource.MaxAmount);
        }

        private void OnSwapWeaponButtonClicked()
        {
            if (this.equipment == null)
            {
                return;
            }

            try
            {
                this.equipment.SwapWeapon();
                this.interactor.EnterSelectionState();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }

        private void OnEndTurnButtonClicked()
        {
            if (this.movement.IsMoving)
            {
                return;
            }

            CombatEncounter.Active?.EndTurn(this.resources.gameObject);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            View.SetPoisoned(this.behaviours.IsPoisoned);

            if (!behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                View.AddBehaviour(behaviour);
            }
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            View.SetPoisoned(this.behaviours.IsPoisoned);

            if (!behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                View.RemoveBehaviour(behaviour);
            }
        }
    }
}