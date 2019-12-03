using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.GameBoard
{
    public class Board : Singleton<Board>
    {
        public event Payload<BoardCell> CellMouseUp;
        public event Payload<BoardCell> CellMouseDown;
        public event Payload<BoardCell> CellMouseEnter;
        public event Payload<BoardCell> CellMouseExit;
        public event Payload ScanCompleted;

        public float CellSize => this.cellSize;
        public int Width => this.width;
        public int Height => this.height;
        public List<BoardCell> Cells { get; private set; } = new List<BoardCell>();
        public BoardCell LastHoveredCell { get; private set; }

        public Collider2D CleaveShapeCollider => this.cleaveShapeCollider;
        public Collider2D Cone2ShapeCollider => this.cone2ShapeCollider;
        public Collider2D Cone3ShapeCollider => this.cone3ShapeCollider;
        public Collider2D Cone5ShapeCollider => this.cone5ShapeCollider;

        [SerializeField] private BoardCell cellPrefab;
        [SerializeField] private Transform cellContainer;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private Vector3 center;
        [SerializeField] private float cellSize;
        [SerializeField] private float cellGap;

        [Header("Sounds")]
        [FMODUnity.EventRef] [SerializeField] private string cellClickSound;

        [Header("Shapes")]
        [SerializeField] private Collider2D cleaveShapeCollider;
        [SerializeField] private Collider2D cone2ShapeCollider;
        [SerializeField] private Collider2D cone3ShapeCollider;
        [SerializeField] private Collider2D cone5ShapeCollider;

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
                this.cellSize * this.width / 2 - this.cellSize / 2.0f,
                this.cellSize * this.height / 2 - this.cellSize / 2.0f
            );

            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    var relative = new Vector3(x * this.cellSize - offset.x, y * this.cellSize - offset.y, 0);
                    var position = this.center + relative;

                    var cell = Instantiate(this.cellPrefab, this.cellContainer);
                    cell.name = $"Cell {x}, {y} #{Cells.Count}";
                    cell.transform.localPosition = position;
                    cell.transform.localRotation = Quaternion.identity;
                    cell.transform.localScale = Vector3.one * (this.cellSize - this.cellGap);

                    Cells.Add(cell);
                }
            }
        }

        private void DestroyCells()
        {
            foreach (var cell in this.cellContainer.GetComponentsInChildren<BoardCell>())
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
            AudioManager.Instance.PlayOneShot(this.cellClickSound);
            CellMouseDown?.Invoke(cell);
        }

        private void OnCellMouseUp(BoardCell cell)
        {
            CellMouseUp?.Invoke(cell);
        }
    }
}