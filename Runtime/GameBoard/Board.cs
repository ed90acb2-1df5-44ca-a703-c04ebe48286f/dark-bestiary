using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.GameBoard
{
    public class Board : Singleton<Board>
    {
        public event Action<BoardCell> CellMouseUp;
        public event Action<BoardCell> CellMouseDown;
        public event Action<BoardCell> CellMouseEnter;
        public event Action<BoardCell> CellMouseExit;
        public event Action ScanCompleted;

        public float CellSize => m_CellSize;
        public int Width => m_Width;
        public int Height => m_Height;
        public List<BoardCell> Cells { get; private set; } = new();
        public BoardCell LastHoveredCell { get; private set; }

        public Collider2D CleaveShapeCollider => m_CleaveShapeCollider;
        public Collider2D Cone2ShapeCollider => m_Cone2ShapeCollider;
        public Collider2D Cone3ShapeCollider => m_Cone3ShapeCollider;
        public Collider2D Cone5ShapeCollider => m_Cone5ShapeCollider;

        [SerializeField] private BoardCell m_CellPrefab;
        [SerializeField] private Transform m_CellContainer;
        [SerializeField] private int m_Width;
        [SerializeField] private int m_Height;
        [SerializeField] private Vector3 m_Center;
        [SerializeField] private float m_CellSize;
        [SerializeField] private float m_CellGap;


        [Header("Sounds")]
        [FMODUnity.EventRef] [SerializeField] private string m_CellClickSound;


        [Header("Shapes")]
        [SerializeField] private Collider2D m_CleaveShapeCollider;
        [SerializeField] private Collider2D m_Cone2ShapeCollider;
        [SerializeField] private Collider2D m_Cone3ShapeCollider;
        [SerializeField] private Collider2D m_Cone5ShapeCollider;

        private void Start()
        {
            Cells = GetComponentsInChildren<BoardCell>().ToList();

            foreach (var cell in Cells)
            {
                cell.Initialize();
                cell.MouseUp += OnCellMouseUp;
                cell.MouseDown += OnCellMouseDown;
                cell.MouseEnter += OnCellMouseEnter;
                cell.MouseExit += OnCellMouseExit;
            }

            LastHoveredCell = Cells[Cells.Count / 2];

            Pathfinder.Instance.ScanCompleted += OnPathfinderScanCompleted;
            Pathfinder.Instance.GraphsUpdated += OnPathfinderGraphUpdated;
            Episode.AnyEpisodeStopped += OnAnyEpisodeStopped;

            Instance.gameObject.SetActive(false);
        }

        private void OnAnyEpisodeStopped(Episode episode)
        {
            foreach (var cell in Cells)
            {
                cell.Clear();
            }
        }

        public void Generate()
        {
            DestroyCells();
            CreateCells();
        }

        private void CreateCells()
        {
            var offset = new Vector2(
                m_CellSize * m_Width / 2 - m_CellSize / 2.0f,
                m_CellSize * m_Height / 2 - m_CellSize / 2.0f
            );

            for (var y = 0; y < m_Height; y++)
            {
                for (var x = 0; x < m_Width; x++)
                {
                    var relative = new Vector3(x * m_CellSize - offset.x, y * m_CellSize - offset.y, 0);
                    var position = m_Center + relative;

                    var cell = Instantiate(m_CellPrefab, m_CellContainer);
                    cell.name = $"Cell {x}, {y} #{Cells.Count}";
                    cell.transform.localPosition = position;
                    cell.transform.localRotation = Quaternion.identity;
                    cell.transform.localScale = Vector3.one * (m_CellSize - m_CellGap);
                    cell.Index = Cells.Count;
                    cell.X = x;
                    cell.Y = y;

                    Cells.Add(cell);
                }
            }
        }

        private void DestroyCells()
        {
            foreach (var cell in m_CellContainer.GetComponentsInChildren<BoardCell>())
            {
                DestroyImmediate(cell.gameObject, true);
            }

            Cells.Clear();
        }

        private void OnPathfinderGraphUpdated()
        {
            OnPathfinderScanCompleted();
        }

        private void OnPathfinderScanCompleted()
        {
            foreach (var cell in Cells)
            {
                cell.IsWalkable = Pathfinder.Instance.IsPointWalkable(cell.transform.position);
            }

            ScanCompleted?.Invoke();
        }

        public void Clear()
        {
            foreach (var cell in Cells)
            {
                cell.NotAvailable();
            }
        }

        private void OnCellMouseEnter(BoardCell cell)
        {
            LastHoveredCell = cell;
            CellMouseEnter?.Invoke(cell);
        }

        private void OnCellMouseExit(BoardCell cell)
        {
            CellMouseExit?.Invoke(cell);
        }

        private void OnCellMouseDown(BoardCell cell)
        {
            AudioManager.Instance.PlayOneShot(m_CellClickSound);
            CellMouseDown?.Invoke(cell);
        }

        private void OnCellMouseUp(BoardCell cell)
        {
            CellMouseUp?.Invoke(cell);
        }
    }
}