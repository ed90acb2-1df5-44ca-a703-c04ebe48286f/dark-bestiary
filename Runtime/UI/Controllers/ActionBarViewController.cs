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
        private readonly Interactor m_Interactor;

        private SpellbookComponent m_Spellbook;
        private EquipmentComponent m_Equipment;
        private MovementComponent m_Movement;
        private ResourcesComponent m_Resources;
        private BehavioursComponent m_Behaviours;
        private ExperienceComponent m_Experience;
        private HealthComponent m_Health;
        private Skill m_CastingSkill;

        public ActionBarViewController(IActionBarView view, Interactor interactor) : base(view)
        {
            m_Interactor = interactor;
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
            View.SkillRemoved += OnSkillRemoved;
            View.SkillPlaced += OnSkillPlaced;

            m_Interactor.StateEnter += OnInteractorStateEnter;
            m_Interactor.StateExit += OnInteractorStateExit;
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
            View.SkillRemoved -= OnSkillRemoved;
            View.SkillPlaced -= OnSkillPlaced;

            m_Interactor.StateEnter -= OnInteractorStateEnter;
            m_Interactor.StateExit -= OnInteractorStateExit;

            Cleanup();
        }

        private void Setup(GameObject entity)
        {
            View.SetPotionBagEnabled(entity.IsCharacter());

            m_Movement = entity.GetComponent<MovementComponent>();
            m_Movement.Started += OnMovementStarted;
            m_Movement.Stopped += OnMovementStopped;

            m_Health = entity.GetComponent<HealthComponent>();
            m_Health.HealthChanged += OnHealthChanged;
            m_Health.ShieldChanged += OnHealthChanged;
            OnHealthChanged(m_Health);

            m_Resources = entity.GetComponent<ResourcesComponent>();
            m_Resources.ActionPointsChanged += OnActionPointsChanged;
            m_Resources.RageChanged += OnRageChanged;

            OnActionPointsChanged(m_Resources.Get(ResourceType.ActionPoint));
            OnRageChanged(m_Resources.Get(ResourceType.Rage));

            m_Behaviours = entity.GetComponent<BehavioursComponent>();
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;
            m_Behaviours.BehaviourRemoved += OnBehaviourRemoved;

            foreach (var behaviour in m_Behaviours.Behaviours)
            {
                OnBehaviourApplied(behaviour);
            }

            m_Equipment = entity.GetComponent<EquipmentComponent>();

            m_Spellbook = entity.GetComponent<SpellbookComponent>();
            m_Spellbook.SkillCooldownStarted += OnSkillCooldownStarted;
            m_Spellbook.SkillCooldownUpdated += OnSkillCooldownUpdated;
            m_Spellbook.SkillCooldownFinished += OnSkillCooldownFinished;

            View.CreateSkillSlots(m_Spellbook.Slots);

            foreach (var slot in m_Spellbook.Slots.Where(slot => slot.Skill.IsOnCooldown()))
            {
                View.StartSkillCooldown(slot.Skill);
            }

            UpdateButtonActiveState();
        }

        private void Cleanup()
        {
            View.RemoveSkillSlots();
            View.RemoveBehaviours();

            if (m_Behaviours != null)
            {
                m_Behaviours.BehaviourApplied -= OnBehaviourApplied;
                m_Behaviours.BehaviourRemoved -= OnBehaviourRemoved;
            }

            if (m_Resources != null)
            {
                m_Resources.ActionPointsChanged -= OnActionPointsChanged;
                m_Resources.RageChanged -= OnRageChanged;
            }

            if (m_Health != null)
            {
                m_Health.HealthChanged -= OnHealthChanged;
                m_Health.ShieldChanged -= OnHealthChanged;
            }

            if (m_Movement != null)
            {
                m_Movement.Started -= OnMovementStarted;
                m_Movement.Stopped -= OnMovementStopped;
            }

            if (m_Spellbook != null)
            {
                m_Spellbook.SkillCooldownStarted -= OnSkillCooldownStarted;
                m_Spellbook.SkillCooldownUpdated -= OnSkillCooldownUpdated;
                m_Spellbook.SkillCooldownFinished -= OnSkillCooldownFinished;
            }
        }

        private void UpdateButtonActiveState()
        {
            var isMyTurn = CombatEncounter.Active == null || CombatEncounter.Active.IsEntityTurn(m_Spellbook.gameObject);

            if (isMyTurn)
            {
                View.EnableEndTurnButton();
            }
            else
            {
                View.DisableEndTurnButton();
            }

            if (!(m_Interactor.State is WaitState) && isMyTurn)
            {
                View.EnableSkillSlots();
            }
            else
            {
                View.DisableSkillSlots();
            }
        }

        private void OnSkillPlaced(SkillSlot slot, Skill skill)
        {
            try
            {
                m_Spellbook.Learn(slot, skill);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnSkillRemoved(Skill skill)
        {
            if (skill.IsOnCooldown())
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_skill_is_on_cooldown"));
                return;
            }

            m_Spellbook.Unlearn(skill);
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
                m_CastingSkill = null;
                View.UnhighlightSkill(castState.Skill);
                View.HideActionPointUsage();
            }
        }

        private void OnSkillClicked(Skill skill)
        {
            if (skill.Id == Skill.s_Empty.Id || skill.IsOnCooldown() || skill.IsDisabled())
            {
                return;
            }

            if (skill == m_CastingSkill)
            {
                m_CastingSkill = null;
                m_Interactor.EnterSelectionState();
            }
            else
            {
                m_CastingSkill = skill;
                m_Interactor.EnterCastState(m_Spellbook.gameObject, skill);
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
            if (m_Spellbook != null && m_Spellbook.gameObject == entity)
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
            View.ShowActionPointUsage(m_Movement.GetMovementCost(path.Count - 1));
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
            if (m_Equipment == null)
            {
                return;
            }

            try
            {
                m_Equipment.SwapWeapon();
                m_Interactor.EnterSelectionState();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
                throw;
            }
        }

        private void OnEndTurnButtonClicked()
        {
            if (m_Movement.IsMoving)
            {
                return;
            }

            CombatEncounter.Active?.EndTurn(m_Resources.gameObject);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            View.SetPoisoned(m_Behaviours.IsPoisoned);

            if (!behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                View.AddBehaviour(behaviour);
            }
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            View.SetPoisoned(m_Behaviours.IsPoisoned);

            if (!behaviour.Flags.HasFlag(BehaviourFlags.Hidden))
            {
                View.RemoveBehaviour(behaviour);
            }
        }
    }
}