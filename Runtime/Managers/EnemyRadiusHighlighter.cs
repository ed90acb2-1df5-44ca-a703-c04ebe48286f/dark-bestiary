using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class EnemyRadiusHighlighter : Singleton<EnemyRadiusHighlighter>
    {
        private Interactor m_Interactor;

        private void Start()
        {
            m_Interactor = Container.Instance.Resolve<Interactor>();

            BoardCell.AnyCellMouseEnter += OnAnyCellMouseEnter;
            BoardCell.AnyCellMouseExit += OnAnyCellMouseExit;
        }

        private void OnAnyCellMouseEnter(BoardCell cell)
        {
            if (!cell.IsOccupied || cell.OccupiedBy.IsOwnedByPlayer() || !m_Interactor.IsSelectionState)
            {
                return;
            }

            if (cell.OccupiedBy.GetComponent<MovementComponent>().IsMoving)
            {
                return;
            }

            Highlight(cell.OccupiedBy);
        }

        private void OnAnyCellMouseExit(BoardCell cell)
        {
            if (!cell.IsOccupied || cell.OccupiedBy.IsOwnedByPlayer() || !m_Interactor.IsSelectionState)
            {
                return;
            }

            Clear();
        }

        private void Highlight(GameObject entity)
        {
            var attack = entity.GetComponent<SpellbookComponent>().Slots
                .Take(2)
                .FirstOrDefault(s => !s.Skill.IsEmpty())?.Skill;

            if (attack == null)
            {
                return;
            }

            BoardNavigator.Instance.HighlightSkillRange(
                attack, entity.transform.position,
                entity.transform.position,
                Color.red.With(a: 0.5f),
                Color.red.With(a: 0.15f));
        }

        private void Clear()
        {
            BoardNavigator.Instance.Board.Clear();
        }
    }
}