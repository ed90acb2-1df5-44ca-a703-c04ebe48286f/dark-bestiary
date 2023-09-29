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
        private readonly Interactor m_Interactor;
        private readonly IPathfinder m_Pathfinder;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly CursorManager m_CursorManager;
        private readonly PathDrawer m_PathDrawer;

        private MovementComponent m_Movement;

        public MoveState(
            Interactor interactor,
            BoardNavigator boardNavigator,
            CursorManager cursorManager,
            IPathfinder pathfinder,
            PathDrawer pathDrawer)
        {
            m_Interactor = interactor;
            m_Pathfinder = pathfinder;
            m_BoardNavigator = boardNavigator;
            m_CursorManager = cursorManager;
            m_PathDrawer = pathDrawer;
        }

        public void Setup(MovementComponent movement)
        {
            m_Movement = movement;
        }

        public override void Enter()
        {
            m_CursorManager.ChangeState(CursorManager.CursorState.Move);

            m_BoardNavigator.Board.CellMouseUp += OnCellMouseUp;
            m_BoardNavigator.Board.CellMouseEnter += OnCellMouseEnter;

            m_Movement.Stop();

            MarkWalkableCells();

            OnCellMouseEnter(m_BoardNavigator.Board.LastHoveredCell);
        }

        public override void Exit()
        {
            m_CursorManager.ChangeState(CursorManager.CursorState.Normal);

            m_BoardNavigator.Board.CellMouseUp -= OnCellMouseUp;
            m_BoardNavigator.Board.CellMouseEnter -= OnCellMouseEnter;
            m_BoardNavigator.Board.Clear();

            m_PathDrawer.Erase();
        }

        public override void Tick(float delta)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                m_Interactor.EnterSelectionState();
            }
        }

        private void MarkWalkableCells()
        {
            var cageBehaviour = m_Movement.GetComponent<BehavioursComponent>().Behaviours
                .FirstOrDefault(behaviour => behaviour is CageBehaviour) as CageBehaviour;

            if (cageBehaviour?.LeaveRadiusEffect == null)
            {
                m_BoardNavigator.HighlightWalkableRadius(
                    m_Movement.gameObject.transform.position,
                    m_Movement.CalculateAvailableTravelRangeCells(),
                    new Color(1.0f, 1.0f, 1.0f, 0.25f)
                );
            }
            else
            {
                m_BoardNavigator.HighlightWalkableRadius(
                    cageBehaviour.Epicenter,
                    cageBehaviour.Radius,
                    new Color(1.0f, 1.0f, 1.0f, 0.25f)
                );
            }
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            var path = m_Pathfinder.FindPath(m_Movement.gameObject, cell.transform.position, false);
            m_PathDrawer.ChangeColor(m_Movement.HasEnoughResourcesToCompletePath(path) ? Color.white : Color.red);
            m_PathDrawer.Draw(path);
        }

        private void OnCellMouseUp(BoardCell cell)
        {
            if (!cell.IsAvailable)
            {
                m_Interactor.EnterSelectionState();
                return;
            }

            m_Movement.Move(cell.transform.position);
            m_Interactor.EnterWaitState(() =>
            {
                if (Input.GetKeyDown(KeyBindings.Get(KeyType.Stop)))
                {
                    m_Movement.Stop();
                }

                return !m_Movement.IsMoving;
            });
        }
    }
}