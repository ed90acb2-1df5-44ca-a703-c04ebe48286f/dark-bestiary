using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Interaction
{
    public class SelectionState : InteractorState
    {
        private readonly Interactor interactor;
        private readonly BoardNavigator boardNavigator;
        private readonly SelectionManager selectionManager;
        private readonly CursorManager cursorManager;

        public SelectionState(
            Interactor interactor,
            BoardNavigator boardNavigator,
            SelectionManager selectionManager,
            CursorManager cursorManager)
        {
            this.interactor = interactor;
            this.boardNavigator = boardNavigator;
            this.selectionManager = selectionManager;
            this.cursorManager = cursorManager;
        }

        public override void Enter()
        {
            this.boardNavigator.Board.CellMouseUp += OnBoardCellMouseUp;
        }

        public override void Exit()
        {
            this.cursorManager.ChangeState(CursorManager.CursorState.Normal);

            this.boardNavigator.Board.CellMouseUp -= OnBoardCellMouseUp;
            this.boardNavigator.Board.Clear();
        }

        public override void Tick(float delta)
        {
            if (Input.GetKeyDown(KeyBindings.Get(KeyType.Move)) && Game.Instance.State.IsScenario)
            {
                MaybeSelect(CharacterManager.Instance.Character.Entity);
            }
        }

        private void OnBoardCellMouseUp(BoardCell cell)
        {
            if (cell.OccupiedBy == null)
            {
                return;
            }

            var occupiedBy = cell.GameObjectsInside.FirstOrDefault(e => e.IsCharacter()) ?? cell.OccupiedBy;

            MaybeSelect(occupiedBy);
        }

        private void MaybeSelect(GameObject entity)
        {
            if (entity.IsDummy() || entity.IsInvisible() && entity.IsEnemyOfPlayer())
            {
                return;
            }

            this.selectionManager.Select(entity);

            if (!entity.IsOwnedByPlayer() || entity.GetComponent<MovementComponent>().IsMoving)
            {
                return;
            }

            this.interactor.EnterMoveState(entity.GetComponent<MovementComponent>());
        }
    }
}