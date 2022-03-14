using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using DG.Tweening;
using UnityEngine;

namespace DarkBestiary.MatchThree
{
    public class MatchThree : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;

        [Header("Cells")]
        [SerializeField] private MatchThreeCell cellPrefab;
        [SerializeField] private Transform cellContainer;

        private readonly List<Color> colors = new List<Color>
        {
            Color.white,
            Color.green,
            Color.blue,
            Color.magenta,
            Color.red,
            Color.grey
        };

        private MonoBehaviourPool<MatchThreeCell> pool;
        private List<MatchThreeCell> cells;
        private MatchThreeCell selected;
        private Vector3 offset;

        private void Start()
        {
            this.pool = MonoBehaviourPool<MatchThreeCell>.Factory(
                this.cellPrefab, this.cellContainer, this.width * this.height);

            this.cells = new List<MatchThreeCell>();
            this.cells.AddRange(Enumerable.Repeat(default(MatchThreeCell), this.width * this.height));

            this.offset = new Vector3(
                this.cellSize * this.width / 2 - this.cellSize / 2.0f,
                this.cellSize * this.height / 2 - this.cellSize / 2.0f
            );

            for (var i = 0; i < this.cells.Count; i++)
            {
                Spawn();
            }

            Timer.Instance.Wait(1, () =>
            {
                DespawnMatched(Match());
            });
        }

        private void Spawn()
        {
            var index = this.cells.FindIndex(c => c == null);

            if (index == -1)
            {
                throw new InvalidOperationException("No free cells.");
            }

            var cell = this.pool.Spawn();
            var position = IndexToWorldPosition(index);

            var random = RNG.Range(0, this.colors.Count - 1);

            cell.transform.position = position.With(y: 12);
            cell.Clicked += OnCellClicked;
            cell.Id = random;
            cell.Setup(index, this.colors[random]);
            cell.Deselect();

            TweenMove(30, cell, position);

            this.cells[index] = cell;
        }

        private void OnCellClicked(MatchThreeCell clicked)
        {
            if (this.selected == clicked)
            {
                this.selected.Deselect();
                this.selected = null;
                return;
            }

            if (this.selected == null)
            {
                this.selected = clicked;
                this.selected.Select();
                return;
            }

            if ((this.selected.transform.position - clicked.transform.position).magnitude <= this.cellSize)
            {
                SwapAndMatch(this.selected, clicked, matched =>
                {
                    DespawnMatched(matched);
                    this.selected.Deselect();
                    this.selected = null;
                });
            }
        }

        private void DespawnMatched(IReadOnlyCollection<MatchThreeCell> cells)
        {
            foreach (var cell in cells)
            {
                Despawn(cell);
            }

            DropDown();

            return;

            foreach (var cell in cells)
            {
                Spawn();
            }

            Timer.Instance.Wait(1, () => DespawnMatched(Match()));
        }

        private void DropDown()
        {
            for (var i = 0; i < this.cells.Count; i++)
            {
                var cell = this.cells[i];

                if (cell == null)
                {
                    continue;
                }

                var x = i % this.width;

                var emptyIndex = -1;

                for (var y = 0; y < i / this.width; y++)
                {
                    var index = x + y * this.width;

                    if (this.cells[index] == null)
                    {
                        emptyIndex = index;
                    }
                    else
                    {
                        break;
                    }
                }

                if (emptyIndex == -1)
                {
                    continue;
                }

                this.cells[i] = null;
                this.cells[emptyIndex] = cell;

                cell.Index = emptyIndex;
                cell.Text.text = emptyIndex.ToString();

                TweenMove(1, cell, IndexToWorldPosition(emptyIndex));

                DropDown();
                break;
            }
        }

        private void SwapAndMatch(MatchThreeCell cellA, MatchThreeCell cellB, Action<List<MatchThreeCell>> callback)
        {
            Swap(cellA, cellB, () =>
            {
                var matched = Match();

                if (matched.Count > 0)
                {
                    callback.Invoke(matched);
                }
                else
                {
                    Swap(cellB, cellA, () => callback.Invoke(matched));
                }
            });
        }

        private List<MatchThreeCell> Match()
        {
            return Enumerable.Empty<List<MatchThreeCell>>()
                .Concat(this.cells.GroupBy(cell => cell.Index % this.width).Select(group => group.ToList()).SelectMany(Match))
                .Concat(this.cells.GroupBy(cell => cell.Index / this.width).Select(group => group.ToList()).SelectMany(Match))
                .SelectMany(match => match)
                .ToList();
        }

        private void Swap(MatchThreeCell cellA, MatchThreeCell cellB, Action callback = null)
        {
            var indexA = cellA.Index;
            var indexB = cellB.Index;

            this.cells[indexA] = cellB;
            this.cells[indexB] = cellA;

            cellA.Index = indexB;
            cellB.Index = indexA;

            TweenMove(10, cellB, cellA.transform.position);
            TweenMove(10, cellA, cellB.transform.position, callback);
        }

        private static List<List<MatchThreeCell>> Match(List<MatchThreeCell> cells)
        {
            var matches = new List<List<MatchThreeCell>>();
            var match = new List<MatchThreeCell>();

            MatchThreeCell previous = null;

            foreach (var cell in cells)
            {
                var isMatch = previous?.Id == cell.Id;

                previous = cell;

                if (isMatch)
                {
                    match.Add(cell);
                    continue;
                }

                if (match.Count >= 3)
                {
                    matches.Add(match);
                }

                match = new List<MatchThreeCell> {cell};
            }

            if (match.Count >= 3)
            {
                matches.Add(match);
            }

            return matches;
        }

        private static void TweenMove(int duration, Component cell, Vector3 position, Action callback = null)
        {
            cell.transform
                .DOMove(position, duration)
                .SetAs(new TweenParams().SetSpeedBased().SetEase(Ease.Linear))
                .OnComplete(() => callback?.Invoke());
        }

        private void Despawn(MatchThreeCell cell)
        {
            Despawn(cell.Index);
        }

        private void Despawn(int index)
        {
            if (this.cells[index] == null)
            {
                return;
            }

            this.cells[index].Despawn();
            this.cells[index].Clicked -= OnCellClicked;
            this.cells[index] = null;
        }

        private Vector3 IndexToWorldPosition(int index)
        {
            return TableToWorldPosition(IndexToTablePosition(index));
        }

        private Vector3 IndexToTablePosition(int index)
        {
            return new Vector3(index % this.width, index / this.width);
        }

        private Vector3 TableToWorldPosition(Vector3 position)
        {
            return position * this.cellSize - this.offset;
        }

        private Vector3 WorldToTablePosition(Vector3 position)
        {
            var index = Mathf.FloorToInt(position.x + position.y * this.width);
            return IndexToTablePosition(index);
        }
    }
}