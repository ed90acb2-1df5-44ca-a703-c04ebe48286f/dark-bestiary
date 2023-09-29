using System.Linq;
using DarkBestiary.Commands;
using DarkBestiary.Exceptions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.Skills.Targeting;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.Interaction
{
    public class CastState : InteractorState
    {
        private readonly Interactor m_Interactor;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly CursorManager m_CursorManager;

        public Skill Skill { get; private set; }

        private GameObject m_Entity;

        public CastState(Interactor interactor, BoardNavigator boardNavigator, CursorManager cursorManager)
        {
            m_Interactor = interactor;
            m_BoardNavigator = boardNavigator;
            m_CursorManager = cursorManager;
        }

        public void Setup(GameObject entity, Skill skill)
        {
            m_Entity = entity;
            Skill = skill;
        }

        public override void Enter()
        {
            if (Skill.UseStrategy is NoneSkillUseStrategy)
            {
                TargetSkill(Skill, null);
                m_Interactor.EnterSelectionState();
                return;
            }

            m_CursorManager.ChangeState(CursorManager.CursorState.Cast);

            m_BoardNavigator.Board.CellMouseEnter += OnCellMouseEnter;
            m_BoardNavigator.Board.CellMouseUp += OnCellMouseUp;

            OnCellMouseEnter(m_BoardNavigator.Board.LastHoveredCell);
        }

        public override void Exit()
        {
            m_CursorManager.ChangeState(CursorManager.CursorState.Normal);

            m_BoardNavigator.Board.CellMouseEnter -= OnCellMouseEnter;
            m_BoardNavigator.Board.CellMouseUp -= OnCellMouseUp;
            m_BoardNavigator.Board.Clear();
        }

        public override void Tick(float delta)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                m_Interactor.EnterSelectionState();
            }
        }

        private void OnCellMouseUp(BoardCell cell)
        {
            if (!Skill.UseStrategy.IsValidCell(Skill, cell) || !cell.IsAvailable)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_invalid_target"));
                m_Interactor.EnterSelectionState();
                return;
            }

            var withinSkillRange = m_BoardNavigator
                .WithinSkillRange(cell.transform.position, m_Entity.transform.position, Skill)
                .ToList();

            if (withinSkillRange.All(element => element.OccupiedBy != m_Entity))
            {
                var target = withinSkillRange
                    .Where(c => !c.IsOccupied)
                    .OrderBy(c => (c.transform.position - m_Entity.transform.position).sqrMagnitude)
                    .First();

                var sequence = new CommandSequence()
                    .Add(new MoveCommand(m_Entity, target.transform.position))
                    .Add(new CastCommand(Skill, cell))
                    .Start();

                m_Interactor.EnterWaitState(() => sequence.IsDone);
                return;
            }

            TargetSkill(Skill, cell);
            m_Interactor.EnterSelectionState();
        }

        private void TargetSkill(Skill skill, BoardCell cell)
        {
            try
            {
                Skill.UseStrategy.Use(skill, cell);
                m_Interactor.EnterSelectionState();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            m_BoardNavigator.Board.Clear();
            m_BoardNavigator.HighlightSkillRangeDefault(Skill, m_Entity.transform.position, cell.transform.position);

            if (!cell.IsAvailable)
            {
                return;
            }

            m_BoardNavigator.HighlightSkillAoe(
                Skill.Caster.transform.position,
                cell.transform.position,
                Skill,
                new Color(1.0f, 1.0f, 1.0f, 0.75f)
            );
        }
    }
}