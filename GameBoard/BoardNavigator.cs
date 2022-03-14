using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using DarkBestiary.Utility;
using UnityEngine;

namespace DarkBestiary.GameBoard
{
    public class BoardNavigator
    {
        public static BoardNavigator Instance => instance ?? (instance = new BoardNavigator(Board.Instance));
        private static BoardNavigator instance;

        public Board Board { get; }

        public BoardNavigator(Board board)
        {
            Board = board;
        }

        public List<BoardCell> Walkable()
        {
            return Board.Cells.Where(cell => cell.gameObject.activeSelf && cell.IsWalkable).ToList();
        }

        public BoardCell BehindTheBackOfOccupying(Vector3 position)
        {
            var cell = WithinCircle(position, 0).FirstOrDefault();

            if (cell == null)
            {
                return null;
            }

            return BehindTheBackOfOccupying(cell);
        }

        public BoardCell BehindTheBackOfOccupying(BoardCell cell)
        {
            if (!cell.IsOccupied)
            {
                return null;
            }

            var index = Board.Cells.IndexOf(cell) +
                        (cell.OccupiedBy.GetComponent<ActorComponent>().Model.IsFacingLeft ? 1 : -1);

            if (!Board.Cells.IndexInBounds(index))
            {
                return null;
            }

            var behindTheBack = Board.Cells[index];

            if (!behindTheBack.IsWalkable)
            {
                return null;
            }

            return behindTheBack;
        }

        public IEnumerable<Collider2D> OverlapCircle(Vector3 center, int radius)
        {
            var edge = radius * Mathf.Sqrt(Board.CellSize * Board.CellSize + Board.CellSize * Board.CellSize);
            edge = Mathf.Max(0.25f, edge);

            return Physics2D.OverlapBoxAll(center.Snapped(), new Vector2(edge, edge), 45);
        }

        public IEnumerable<Collider2D> OverlapSquare(Vector3 center, int radius)
        {
            return Physics2D.OverlapBoxAll(
                center.Snapped(), new Vector2(radius * Board.CellSize * 2, radius * Board.CellSize * 2), 0);
        }

        public List<BoardCell> WithinSquare(Vector3 center, int radius)
        {
            return OverlapSquare(center, radius).ToCellsPrecise().ToList();
        }

        public List<BoardCell> WithinCircle(Vector3 center, int radius)
        {
            return OverlapCircle(center, radius).ToCellsPrecise().ToList();
        }

        public List<BoardCell> WithinCross(Vector3 center, int radius)
        {
            var size = radius * Board.CellSize * 2 + 1;

            var result = new List<BoardCell>();

            result.AddRange(Physics2D.OverlapBoxAll(center.Snapped(), new Vector2(size, 0.25f), 0).ToCellsPrecise());
            result.AddRange(Physics2D.OverlapBoxAll(center.Snapped(), new Vector2(0.25f, size), 0).ToCellsPrecise());
            result = result.Distinct().ToList();

            return result;
        }

        public List<BoardCell> WithinLine(Vector3 origin, Vector3 direction, int length)
        {
            return Physics2D
                .RaycastAll(origin.Snapped(), direction, length * Board.CellSize)
                .ToCells()
                .ToList();
        }

        public List<BoardCell> WithinCleave(Vector3 origin, Vector3 target)
        {
            return WithinCollider(Board.CleaveShapeCollider, origin, target);
        }

        public List<BoardCell> WithinCone2(Vector3 origin, Vector3 target)
        {
            return WithinCollider(Board.Cone2ShapeCollider, origin, target);
        }

        public List<BoardCell> WithinCone3(Vector3 origin, Vector3 target)
        {
            return WithinCollider(Board.Cone3ShapeCollider, origin, target);
        }

        public List<BoardCell> WithinCone5(Vector3 origin, Vector3 target)
        {
            return WithinCollider(Board.Cone5ShapeCollider, origin, target);
        }

        public List<BoardCell> WithinCollider(Collider2D collider, Vector3 origin, Vector3 target)
        {
            collider.transform.position = origin;
            collider.transform.rotation = QuaternionUtility.LookRotation2D(target - origin);

            var colliders = new Collider2D[50];
            var overlaps = Physics2D.OverlapCollider(collider, new ContactFilter2D(), colliders);

            collider.transform.position = new Vector3(-100, 0, 0);

            return colliders.Take(overlaps).ToCellsPrecise().ToList();
        }

        public List<GameObject> EntitiesInRadius(Vector3 center, int radius)
        {
            return WithinCircle(center, radius).ToEntities().ToList();
        }

        public List<BoardCell> WithinShape(Vector3 origin, Vector3 target, Shape shape, int size)
        {
            switch (shape)
            {
                case Shape.Circle:
                    return WithinCircle(origin, size);
                case Shape.Cross:
                    return WithinCross(origin, size);
                case Shape.Line:
                    return WithinLine(origin, (target - origin).normalized, size);
                case Shape.Cone2:
                    return WithinCone2(origin, target);
                case Shape.Cone3:
                    return WithinCone3(origin, target);
                case Shape.Cone5:
                    return WithinCone5(origin, target);
                case Shape.Cleave:
                    return WithinCleave(origin, target);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<GameObject> EntitiesInSkillRange(Vector3 origin, Vector3 target, Skill skill)
        {
            return WithinSkillRange(origin, target, skill).ToEntities().ToList();
        }

        public void HighlightSkillRangeDefault(Skill skill, Vector3 origin, Vector3 target)
        {
            HighlightSkillRange(skill, origin, target, Color.white.With(a: 0.15f), Color.yellow.With(a: 0.15f));
        }

        public void HighlightSkillRange(Skill skill, Vector3 origin, Vector3 target, Color rangeColor, Color moveColor)
        {
            if (!skill.Caster.GetComponent<BehavioursComponent>().IsImmobilized)
            {
                var actionPoints = skill.Caster.GetComponent<ResourcesComponent>().Get(ResourceType.ActionPoint).Amount;
                var movementCost = skill.Caster.GetComponent<MovementComponent>().GetMovementCost();

                var travelRange = 20;

                if (Encounter.IsCombat)
                {
                    travelRange = (int) (actionPoints - skill.GetCost(ResourceType.ActionPoint)) / movementCost + skill.GetMaxRange();
                }

                var cells = WithinSkillRange(
                    origin,
                    target,
                    skill.RangeShape,
                    skill.RangeMin,
                    travelRange,
                    skill.Flags.HasFlag(SkillFlags.CheckLineOfSight));

                foreach (var cell in cells)
                {
                    if (!cell.IsWalkable)
                    {
                        continue;
                    }

                    cell.Available(moveColor);
                }
            }

            foreach (var cell in WithinSkillRange(origin, target, skill))
            {
                if (!cell.IsWalkable)
                {
                    continue;
                }

                cell.Available(rangeColor);
            }
        }

        public List<BoardCell> WithinSkillRange(Vector3 origin, Vector3 target, Skill skill)
        {
            return WithinSkillRange(origin, target, skill.RangeShape, skill.RangeMin, skill.GetMaxRange(),
                skill.Flags.HasFlag(SkillFlags.CheckLineOfSight));
        }

        public List<BoardCell> WithinSkillRange(
            Vector3 origin, Vector3 target, Shape shape, int rangeMin, int rangeMax, bool checkSight)
        {
            var cellsInRange = WithinShape(origin, target, shape, rangeMax);

            if (rangeMin > 0)
            {
                var cellsTooClose = WithinShape(origin, target, shape, rangeMin);
                cellsInRange = cellsInRange.Where(cell => !cellsTooClose.Contains(cell)).ToList();
            }

            if (checkSight)
            {
                cellsInRange = cellsInRange.OnLineOfSight(origin).ToList();
            }

            return cellsInRange;
        }

        public void HighlightSkillAOE(Vector3 origin, Vector3 target, Skill skill, Color color)
        {
            foreach (var cell in WithinSkillAOE(origin, target, skill))
            {
                if (!cell.IsWalkable)
                {
                    continue;
                }

                cell.Available(color);
            }
        }

        public void HighlightRadius(Vector3 origin, int radius, Color color)
        {
            foreach (var cell in WithinCircle(origin, radius))
            {
                if (!cell.IsWalkable)
                {
                    continue;
                }

                cell.Available(color);
            }
        }

        private List<BoardCell> WithinSkillAOE(Vector3 origin, Vector3 target, Skill skill)
        {
            List<BoardCell> cells;

            switch (skill.AOEShape)
            {
                case Shape.Circle:
                    cells = WithinCircle(target, skill.AOE);
                    break;
                case Shape.Cross:
                    cells = WithinCross(target, skill.AOE);
                    break;
                case Shape.Line:
                    cells = WithinLine(origin, target - origin, skill.AOE);
                    break;
                case Shape.Cone2:
                    cells = WithinCone2(origin, target);
                    break;
                case Shape.Cone3:
                    cells = WithinCone3(origin, target);
                    break;
                case Shape.Cone5:
                    cells = WithinCone5(origin, target);
                    break;
                case Shape.Cleave:
                    cells = WithinCleave(origin, target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return skill.Flags.HasFlag(SkillFlags.CheckLineOfSight) ? cells.OnLineOfSight(origin).ToList() : cells;
        }

        public bool IsWithinSkillRange(GameObject entity, Vector3 target, Skill skill)
        {
            return WithinSkillRange(target, entity.transform.position, skill).Any(cell => cell.OccupiedBy == entity);
        }

        public void HighlightWalkableRadius(Vector3 center, int radius, Color color)
        {
            var cells = WalkableInRadius(center, radius);

            foreach (var cell in cells)
            {
                cell.Available(color);
            }
        }

        public int DistanceInCells(Vector3 origin, Vector3 target)
        {
            var cellA = NearestCell(origin);
            var cellB = NearestCell(target);

            return Mathf.Abs(cellA.X - cellB.X) + Mathf.Abs(cellA.Y - cellB.Y);
        }

        public BoardCell NearestCell(Vector3 position)
        {
            return Board.Cells.OrderBy(c => (c.transform.position - position).sqrMagnitude).First();
        }

        public List<BoardCell> WalkableInRadius(Vector3 center, int radius)
        {
            return WithinCircle(center, radius).Walkable().ToList();
        }

        public BoardCell Center()
        {
            return CellAt((int) Math.Ceiling((Board.Width - 1) / 2.0f), (int) Math.Ceiling((Board.Height - 1) / 2.0f));
        }

        public BoardCell Top()
        {
            return CellAt((int) Math.Ceiling((Board.Width - 1) / 2.0f), Board.Height - 1);
        }

        public BoardCell Right()
        {
            return CellAt(Board.Width - 1, (int) Math.Ceiling((Board.Height - 1) / 2.0f));
        }

        public BoardCell Bottom()
        {
            return CellAt((int) Math.Ceiling((Board.Width - 1) / 2.0f), 0);
        }

        public BoardCell Left()
        {
            return CellAt(0, (int) Math.Ceiling((Board.Height - 1) / 2.0f));
        }

        private BoardCell CellAt(int row, int column)
        {
            return Board.Cells[column * Board.Width + row];
        }

        public IEnumerable<BoardCell> PerimeterTop()
        {
            var cells = new List<BoardCell>();

            for (var x = 1; x < Board.Width - 2; x++)
            {
                cells.Add(CellAt(x, 1));
            }

            return cells;
        }

        public IEnumerable<BoardCell> PerimeterRight()
        {
            var cells = new List<BoardCell>();

            for (var y = 1; y < Board.Height - 2; y++)
            {
                cells.Add(CellAt(Board.Width - 2, y));
            }

            return cells;
        }

        public IEnumerable<BoardCell> PerimeterBottom()
        {
            var cells = new List<BoardCell>();

            for (var x = 1; x < Board.Width - 2; x++)
            {
                cells.Add(CellAt(x, Board.Height - 2));
            }

            return cells;
        }

        public IEnumerable<BoardCell> PerimeterLeft()
        {
            var cells = new List<BoardCell>();

            for (var y = 1; y < Board.Height - 2; y++)
            {
                cells.Add(CellAt(1, y));
            }

            return cells;
        }
    }
}