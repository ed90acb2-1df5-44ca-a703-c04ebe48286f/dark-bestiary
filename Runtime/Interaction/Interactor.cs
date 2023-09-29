using System;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Interaction
{
    public class Interactor : IInitializable, ITickable
    {
        public static Interactor Instance { get; private set; }

        public event Action<InteractorState> StateEnter;
        public event Action<InteractorState> StateExit;

        public bool IsSelectionState => State is SelectionState;
        public bool IsCastState => State is CastState;
        public bool IsMoveState => State is MoveState;
        public bool IsWaitState => State is WaitState;

        public InteractorState State { get; private set; }

        private readonly SelectionState m_SelectionState;
        private readonly WaitState m_WaitState;
        private readonly MoveState m_MoveState;
        private readonly CastState m_CastState;

        public Interactor(
            BoardNavigator boardNavigator,
            CursorManager cursorManager,
            SelectionManager selectionManager,
            IPathfinder pathfinder,
            PathDrawer pathDrawer)
        {
            m_SelectionState = new SelectionState(this, boardNavigator, selectionManager, cursorManager);
            m_MoveState = new MoveState(this, boardNavigator, cursorManager, pathfinder, pathDrawer);
            m_CastState = new CastState(this, boardNavigator, cursorManager);
            m_WaitState = new WaitState(this);
        }

        public void Initialize()
        {
            Instance = this;

            EnterSelectionState();

            Scenario.AnyScenarioExit += _ => EnterSelectionState();
            Scenario.AnyScenarioCompleted += _ => EnterSelectionState();
            CombatEncounter.AnyCombatTurnEnded += _ => EnterSelectionState();
        }

        public void Tick()
        {
            State?.Tick(Time.deltaTime);
        }

        public void SwitchState(InteractorState state)
        {
            if (State != null)
            {
                StateExit?.Invoke(State);
                State?.Exit();
            }

            State = state;
            StateEnter?.Invoke(state);
            State.Enter();
        }

        public void EnterDisabledState()
        {
            SwitchState(new DisabledState());
        }

        public void EnterWaitState(Func<bool> condition)
        {
            m_WaitState.Setup(condition);
            SwitchState(m_WaitState);
        }

        public void EnterSelectionState()
        {
            SwitchState(m_SelectionState);
        }

        public void EnterCastState(GameObject entity, Skill skill)
        {
            if (!entity.IsOwnedByPlayer())
            {
                Debug.LogWarning($"{entity.name} can't enter cast state, not owned by player.");
                return;
            }

            if (!CombatEncounter.Active?.IsEntityTurn(entity) ?? false)
            {
                Debug.LogWarning($"{entity.name} can't enter cast state, not his/her turn.");
                return;
            }

            m_CastState.Setup(entity, skill);
            SwitchState(m_CastState);
        }

        public void EnterMoveState(GameObject entity)
        {
            EnterMoveState(entity.GetComponent<MovementComponent>());
        }

        public void EnterMoveState(MovementComponent movement)
        {
            if (!movement.gameObject.IsOwnedByPlayer())
            {
                Debug.LogWarning($"{movement.gameObject.name} can't enter move state, not owned by player.");
                return;
            }

            if (CombatEncounter.Active == null)
            {
                m_MoveState.Setup(movement);
                SwitchState(m_MoveState);
                return;
            }

            if (CombatEncounter.Active.IsEntityTurn(movement.gameObject))
            {
                m_MoveState.Setup(movement);
                SwitchState(m_MoveState);
                return;
            }

            if (!CombatEncounter.Active.TrySwitchTurn(movement.gameObject))
            {
                Debug.LogWarning($"{movement.gameObject.name} can't enter move state, not his/her turn.");
                return;
            }

            m_MoveState.Setup(movement);
            SwitchState(m_MoveState);
        }
    }
}