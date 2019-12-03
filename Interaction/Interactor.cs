using System;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Interaction
{
    public class Interactor : IStateMachine<InteractorState>, IInitializable, ITickable
    {
        public static Interactor Instance { get; private set; }

        public event Payload<InteractorState> StateEnter;
        public event Payload<InteractorState> StateExit;

        public bool IsSelectionState => State is SelectionState;
        public bool IsCastState => State is CastState;
        public bool IsMoveState => State is MoveState;
        public bool IsWaitState => State is WaitState;

        public InteractorState State { get; private set; }

        private readonly CharacterManager characterManager;
        private readonly SelectionState selectionState;
        private readonly WaitState waitState;
        private readonly MoveState moveState;
        private readonly CastState castState;

        public Interactor(
            BoardNavigator boardNavigator,
            CursorManager cursorManager,
            SelectionManager selectionManager,
            CharacterManager characterManager,
            IPathfinder pathfinder,
            PathDrawer pathDrawer)
        {
            this.characterManager = characterManager;
            this.selectionState = new SelectionState(this, boardNavigator, selectionManager, cursorManager);
            this.moveState = new MoveState(this, boardNavigator, cursorManager, pathfinder, pathDrawer);
            this.castState = new CastState(this, boardNavigator, cursorManager);
            this.waitState = new WaitState(this);
        }

        public void Initialize()
        {
            Instance = this;

            EnterSelectionState();

            Scenario.AnyScenarioExit += _ => EnterSelectionState();
            Scenario.AnyScenarioCompleted += _ => EnterSelectionState();

            CombatEncounter.AnyCombatTurnEnded += entity =>
            {
                if (!entity.IsCharacter())
                {
                    return;
                }

                EnterSelectionState();
            };

            Episode.AnyEpisodeCompleted += OnAnyEpisodeCompleted;
            Episode.AnyEpisodeFailed += OnAnyEpisodeCompleted;
        }

        private void OnAnyEpisodeCompleted(Episode episode)
        {
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                if (episode.Scenario.IsActiveEpisodeLast || !this.characterManager.Character.Entity.IsAlive())
                {
                    EnterSelectionState();
                }
                else
                {
                    EnterMoveState(this.characterManager.Character.Entity);
                }
            });
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
            this.waitState.Setup(condition);
            SwitchState(this.waitState);
        }

        public void EnterSelectionState()
        {
            SwitchState(this.selectionState);
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

            this.castState.Setup(entity, skill);
            SwitchState(this.castState);
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
                this.moveState.Setup(movement);
                SwitchState(this.moveState);
                return;
            }

            if (CombatEncounter.Active.IsEntityTurn(movement.gameObject))
            {
                this.moveState.Setup(movement);
                SwitchState(this.moveState);
                return;
            }

            if (!CombatEncounter.Active.TrySwitchTurn(movement.gameObject))
            {
                Debug.LogWarning($"{movement.gameObject.name} can't enter move state, not his/her turn.");
                return;
            }

            this.moveState.Setup(movement);
            SwitchState(this.moveState);
        }
    }
}