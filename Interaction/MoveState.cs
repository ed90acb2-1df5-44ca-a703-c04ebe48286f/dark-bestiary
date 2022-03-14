using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Pathfinding;
using UnityEngine;

namespace DarkBestiary.Interaction
{
    public class MoveState : InteractorState
    {
        private readonly Interactor interactor;
        private readonly IPathfinder pathfinder;
        private readonly BoardNavigator boardNavigator;
        private readonly CursorManager cursorManager;
        private readonly PathDrawer pathDrawer;

        private MovementComponent movement;

        public MoveState(
            Interactor interactor,
            BoardNavigator boardNavigator,
            CursorManager cursorManager,
            IPathfinder pathfinder,
            PathDrawer pathDrawer)
        {
            this.interactor = interactor;
            this.pathfinder = pathfinder;
            this.boardNavigator = boardNavigator;
            this.cursorManager = cursorManager;
            this.pathDrawer = pathDrawer;
        }

        public void Setup(MovementComponent movement)
        {
            this.movement = movement;
        }

        public override void Enter()
        {
            this.cursorManager.ChangeState(CursorManager.CursorState.Move);

            this.boardNavigator.Board.CellMouseUp += OnCellMouseUp;
            this.boardNavigator.Board.CellMouseEnter += OnCellMouseEnter;

            this.movement.Stop();

            MarkWalkableCells();

            OnCellMouseEnter(this.boardNavigator.Board.LastHoveredCell);
        }

        public override void Exit()
        {
            this.cursorManager.ChangeState(CursorManager.CursorState.Normal);

            this.boardNavigator.Board.CellMouseUp -= OnCellMouseUp;
            this.boardNavigator.Board.CellMouseEnter -= OnCellMouseEnter;
            this.boardNavigator.Board.Clear();

            this.pathDrawer.Erase();
        }

        public override void Tick(float delta)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                this.interactor.EnterSelectionState();
            }
        }

        private void MarkWalkableCells()
        {
            var cageBehaviour = this.movement.GetComponent<BehavioursComponent>().Behaviours
                .FirstOrDefault(behaviour => behaviour is CageBehaviour) as CageBehaviour;

            if (cageBehaviour?.LeaveRadiusEffect == null)
            {
                this.boardNavigator.HighlightWalkableRadius(
                    this.movement.gameObject.transform.position,
                    this.movement.CalculateAvailableTravelRangeCells(),
                    new Color(1.0f, 1.0f, 1.0f, 0.25f)
                );
            }
            else
            {
                this.boardNavigator.HighlightWalkableRadius(
                    cageBehaviour.Epicenter,
                    cageBehaviour.Radius,
                    new Color(1.0f, 1.0f, 1.0f, 0.25f)
                );
            }
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            var path = this.pathfinder.FindPath(this.movement.gameObject, cell.transform.position, false);
            this.pathDrawer.ChangeColor(this.movement.HasEnoughResourcesToCompletePath(path) ? Color.white : Color.red);
            this.pathDrawer.Draw(path);
        }

        private void OnCellMouseUp(BoardCell cell)
        {
            if (!cell.IsAvailable)
            {
                this.interactor.EnterSelectionState();
                return;
            }

            this.movement.Move(cell.transform.position);
            this.interactor.EnterWaitState(() =>
            {
                if (Input.GetKeyDown(KeyBindings.Get(KeyType.Stop)))
                {
                    this.movement.Stop();
                }

                return !this.movement.IsMoving;
            });
        }
    }
}