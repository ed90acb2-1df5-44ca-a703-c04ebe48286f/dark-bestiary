using System.Collections.Generic;
using System.Linq;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class BoardCellCollectionExtensions
    {
        public static IEnumerable<Corpse> Corpses(this IEnumerable<GameObject> gameObjects)
        {
            return gameObjects.Where(gameObject => gameObject != null)
                .Select(gameObject => gameObject.GetComponent<Corpse>())
                .Where(corpse => corpse != null && !corpse.IsConsumed);
        }

        public static IEnumerable<BoardCell> OnLineOfSight(this IEnumerable<BoardCell> cells, Vector3 origin)
        {
            return cells.Where(cell => cell.IsLineOfSightWalkable(origin));
        }

        public static IEnumerable<BoardCell> Walkable(this IEnumerable<BoardCell> cells)
        {
            return cells.Where(cell => cell.IsWalkable).ToList();
        }

        public static IEnumerable<GameObject> ToEntities(this IEnumerable<BoardCell> cells)
        {
            return cells.Where(cell => cell.IsOccupied && !cell.OccupiedBy.IsDummy())
                .Select(cell => cell.OccupiedBy);
        }

        public static IEnumerable<BoardCell> ToCells(this IEnumerable<RaycastHit2D> hits)
        {
            return hits.Select(hit => hit.collider).ToCells();
        }

        public static IEnumerable<BoardCell> ToCells(this IEnumerable<Collider2D> colliders)
        {
            var result = new List<BoardCell>();

            foreach (var item in colliders)
            {
                var cell = item.GetComponent<BoardCell>();

                if (cell == null)
                {
                    continue;
                }

                result.Add(cell);
            }

            return result;
        }

        public static IEnumerable<BoardCell> ToCellsPrecise(this IEnumerable<RaycastHit2D> hits)
        {
            return hits.Select(hit => hit.collider).ToCellsPrecise();
        }

        public static IEnumerable<BoardCell> ToCellsPrecise(this IEnumerable<Collider2D> colliders)
        {
            var result = new List<BoardCell>();

            foreach (var item in colliders)
            {
                var hitbox = item.GetComponent<BoardCellHitbox>();

                if (hitbox == null || hitbox.Cell == null)
                {
                    continue;
                }

                result.Add(hitbox.Cell);
            }

            return result;
        }
    }
}