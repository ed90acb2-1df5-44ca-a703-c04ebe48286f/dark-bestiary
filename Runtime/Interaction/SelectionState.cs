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
        private readonly Interactor m_Interactor;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly SelectionManager m_SelectionManager;
        private readonly CursorManager m_CursorManager;

        public SelectionState(
            Interactor interactor,
            BoardNavigator boardNavigator,
            SelectionManager selectionManager,
            CursorManager cursorManager)
        {
            m_Interactor = interactor;
            m_BoardNavigator = boardNavigator;
            m_SelectionManager = selectionManager;
            m_CursorManager = cursorManager;
        }

        public override void Enter()
        {
            m_BoardNavigator.Board.CellMouseUp += OnBoardCellMouseUp;
        }

        public override void Exit()
        {
            m_CursorManager.ChangeState(CursorManager.CursorState.Normal);

            m_BoardNavigator.Board.CellMouseUp -= OnBoardCellMouseUp;
            m_BoardNavigator.Board.Clear();
        }

        public override void Tick(float delta)
        {
            if (Input.GetKeyDown(KeyBindings.Get(KeyType.Move)) && Game.Instance.IsScenario)
            {
                MaybeSelect(Game.Instance.Character.Entity);
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

            m_SelectionManager.Select(entity);

            if (!entity.IsOwnedByPlayer() || entity.GetComponent<MovementComponent>().IsMoving)
            {
                return;
            }

            m_Interactor.EnterMoveState(entity.GetComponent<MovementComponent>());
        }
    }
}