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
        private readonly Interactor interactor;
        private readonly BoardNavigator boardNavigator;
        private readonly CursorManager cursorManager;

        public Skill Skill { get; private set; }

        private GameObject entity;

        public CastState(Interactor interactor, BoardNavigator boardNavigator, CursorManager cursorManager)
        {
            this.interactor = interactor;
            this.boardNavigator = boardNavigator;
            this.cursorManager = cursorManager;
        }

        public void Setup(GameObject entity, Skill skill)
        {
            this.entity = entity;
            Skill = skill;
        }

        public override void Enter()
        {
            if (Skill.UseStrategy is NoneSkillUseStrategy)
            {
                TargetSkill(Skill, null);
                this.interactor.EnterSelectionState();
                return;
            }

            this.cursorManager.ChangeState(CursorManager.CursorState.Cast);

            this.boardNavigator.Board.CellMouseEnter += OnCellMouseEnter;
            this.boardNavigator.Board.CellMouseUp += OnCellMouseUp;

            OnCellMouseEnter(this.boardNavigator.Board.LastHoveredCell);
        }

        public override void Exit()
        {
            this.cursorManager.ChangeState(CursorManager.CursorState.Normal);

            this.boardNavigator.Board.CellMouseEnter -= OnCellMouseEnter;
            this.boardNavigator.Board.CellMouseUp -= OnCellMouseUp;
            this.boardNavigator.Board.Clear();
        }

        public override void Tick(float delta)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                this.interactor.EnterSelectionState();
            }
        }

        private void OnCellMouseUp(BoardCell cell)
        {
            if (!Skill.UseStrategy.IsValidCell(Skill, cell) || !cell.IsAvailable)
            {
                UiErrorFrame.Instance.Push(I18N.Instance.Get("exception_invalid_target"));
                this.interactor.EnterSelectionState();
                return;
            }

            var withinSkillRange = this.boardNavigator
                .WithinSkillRange(cell.transform.position, this.entity.transform.position, Skill)
                .ToList();

            if (withinSkillRange.All(element => element.OccupiedBy != this.entity))
            {
                var target = withinSkillRange
                    .Where(c => !c.IsOccupied)
                    .OrderBy(c => (c.transform.position - this.entity.transform.position).sqrMagnitude)
                    .First();

                var sequence = new CommandSequence()
                    .Add(new MoveCommand(this.entity, target.transform.position))
                    .Add(new CastCommand(Skill, cell))
                    .Start();

                this.interactor.EnterWaitState(() => sequence.IsDone);
                return;
            }

            TargetSkill(Skill, cell);
            this.interactor.EnterSelectionState();
        }

        private void TargetSkill(Skill skill, BoardCell cell)
        {
            try
            {
                Skill.UseStrategy.Use(skill, cell);
                this.interactor.EnterSelectionState();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            this.boardNavigator.Board.Clear();
            this.boardNavigator.HighlightSkillRangeDefault(Skill, this.entity.transform.position, cell.transform.position);

            if (!cell.IsAvailable)
            {
                return;
            }

            this.boardNavigator.HighlightSkillAOE(
                Skill.Caster.transform.position,
                cell.transform.position,
                Skill,
                new Color(1.0f, 1.0f, 1.0f, 0.75f)
            );
        }
    }
}