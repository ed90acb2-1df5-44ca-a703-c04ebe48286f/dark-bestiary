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
        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;
        [SerializeField] private float m_CellSize;


        [Header("Cells")]
        [SerializeField] private MatchThreeCell m_CellPrefab;
        [SerializeField] private Transform m_CellContainer;

        private readonly List<Color> m_Colors = new()
        {
            Color.white,
            Color.green,
            Color.blue,
            Color.magenta,
            Color.red,
            Color.grey
        };

        private MonoBehaviourPool<MatchThreeCell> m_Pool;
        private List<MatchThreeCell> m_Cells;
        private MatchThreeCell m_Selected;
        private Vector3 m_Offset;

        private void Start()
        {
            m_Pool = MonoBehaviourPool<MatchThreeCell>.Factory(
                m_CellPrefab, m_CellContainer, m_Width * m_Height);

            m_Cells = new List<MatchThreeCell>();
            m_Cells.AddRange(Enumerable.Repeat(default(MatchThreeCell), m_Width * m_Height));

            m_Offset = new Vector3(
                m_CellSize * m_Width / 2 - m_CellSize / 2.0f,
                m_CellSize * m_Height / 2 - m_CellSize / 2.0f
            );

            for (var i = 0; i < m_Cells.Count; i++)
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
            var index = m_Cells.FindIndex(c => c == null);

            if (index == -1)
            {
                throw new InvalidOperationException("No free cells.");
            }

            var cell = m_Pool.Spawn();
            var position = IndexToWorldPosition(index);

            var random = Rng.Range(0, m_Colors.Count - 1);

            cell.transform.position = position.With(y: 12);
            cell.Clicked += OnCellClicked;
            cell.Id = random;
            cell.Setup(index, m_Colors[random]);
            cell.Deselect();

            TweenMove(30, cell, position);

            m_Cells[index] = cell;
        }

        private void OnCellClicked(MatchThreeCell clicked)
        {
            if (m_Selected == clicked)
            {
                m_Selected.Deselect();
                m_Selected = null;
                return;
            }

            if (m_Selected == null)
            {
                m_Selected = clicked;
                m_Selected.Select();
                return;
            }

            if ((m_Selected.transform.position - clicked.transform.position).magnitude <= m_CellSize)
            {
                SwapAndMatch(m_Selected, clicked, matched =>
                {
                    DespawnMatched(matched);
                    m_Selected.Deselect();
                    m_Selected = null;
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
            for (var i = 0; i < m_Cells.Count; i++)
            {
                var cell = m_Cells[i];

                if (cell == null)
                {
                    continue;
                }

                var x = i % m_Width;

                var emptyIndex = -1;

                for (var y = 0; y < i / m_Width; y++)
                {
                    var index = x + y * m_Width;

                    if (m_Cells[index] == null)
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

                m_Cells[i] = null;
                m_Cells[emptyIndex] = cell;

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
                .Concat(m_Cells.GroupBy(cell => cell.Index % m_Width).Select(group => group.ToList()).SelectMany(Match))
                .Concat(m_Cells.GroupBy(cell => cell.Index / m_Width).Select(group => group.ToList()).SelectMany(Match))
                .SelectMany(match => match)
                .ToList();
        }

        private void Swap(MatchThreeCell cellA, MatchThreeCell cellB, Action callback = null)
        {
            var indexA = cellA.Index;
            var indexB = cellB.Index;

            m_Cells[indexA] = cellB;
            m_Cells[indexB] = cellA;

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
            if (m_Cells[index] == null)
            {
                return;
            }

            m_Cells[index].Despawn();
            m_Cells[index].Clicked -= OnCellClicked;
            m_Cells[index] = null;
        }

        private Vector3 IndexToWorldPosition(int index)
        {
            return TableToWorldPosition(IndexToTablePosition(index));
        }

        private Vector3 IndexToTablePosition(int index)
        {
            return new Vector3(index % m_Width, index / m_Width);
        }

        private Vector3 TableToWorldPosition(Vector3 position)
        {
            return position * m_CellSize - m_Offset;
        }

        private Vector3 WorldToTablePosition(Vector3 position)
        {
            var index = Mathf.FloorToInt(position.x + position.y * m_Width);
            return IndexToTablePosition(index);
        }
    }
}