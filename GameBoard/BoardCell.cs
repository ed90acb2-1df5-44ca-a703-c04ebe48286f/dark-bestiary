using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.GameBoard
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoardCell : MonoBehaviour
    {
        public static event Payload<GameObject, BoardCell> AnyEntityEnterCell;
        public static event Payload<GameObject, BoardCell> AnyEntityExitCell;
        public static event Payload<BoardCell> AnyCellMouseEnter;
        public static event Payload<BoardCell> AnyCellMouseExit;
        public static event Payload<BoardCell> AnyCellOccupied;
        public static event Payload<BoardCell> AnyCellRefused;

        public event Payload<BoardCell> MouseUp;
        public event Payload<BoardCell> MouseDown;
        public event Payload<BoardCell> MouseEnter;
        public event Payload<BoardCell> MouseExit;

        public int Index;
        public int X;
        public int Y;

        public GameObject OccupiedBy { get; private set; }
        public HashSet<GameObject> GameObjectsInside { get; } = new HashSet<GameObject>();
        public bool IsOccupied => OccupiedBy != null;
        public bool IsAvailable { get; private set; }

        public bool IsWalkable
        {
            get => this.isWalkable && gameObject.activeSelf;
            set => this.isWalkable = value;
        }

        public bool IsReserved { get; set; }

        private static BoardCell hovered;

        [SerializeField] private BoardCellHitbox hitbox;
        [SerializeField] private SpriteRenderer highlight;
        [SerializeField] private SpriteRenderer danger;

        private new SpriteRenderer renderer;
        private string originalName;
        private bool isHovered;
        private bool isWalkable;

        public void Construct(int index, int x, int y)
        {
            Index = index;
            X = x;
            Y = y;
        }

        public void Initialize()
        {
            UnitComponent.AnyUnitTerminated += OnAnyUnitTerminated;
            Scenario.AnyScenarioStarted += OnScenarioAnyScenarioStarted;

            this.originalName = name;
            this.renderer = GetComponent<SpriteRenderer>();
            this.hitbox.Initialize(this);

            NotAvailable();
        }

        public bool IsLineOfSightWalkable(Vector3 target)
        {
            return IsOnLineOfSight(target, cell => cell.IsWalkable);
        }

        public bool IsLineOfSightWalkableAndEmpty(Vector3 target)
        {
            return IsOnLineOfSight(target, cell => cell.IsWalkable && !cell.IsOccupied);
        }

        public bool IsOnLineOfSight(Vector3 target, Func<BoardCell, bool> predicate)
        {
            if (!IsWalkable)
            {
                return false;
            }

            var magnitude = (target - transform.position).magnitude;
            var direction = (target - transform.position).normalized;

            return Physics2D
                .RaycastAll(transform.position, direction, magnitude - Board.Instance.CellSize)
                .ToCells()
                .All(predicate);
        }

        public void Available(Color color)
        {
            IsAvailable = true;
            this.renderer.color = SettingsManager.Instance.HighContrastMode ? color.With(a: color.a + 0.5f) : color;
        }

        public void NotAvailable()
        {
            IsAvailable = false;
            this.renderer.color = Color.white.With(a: 0);
        }

        public void Dangerous()
        {
            this.danger.material.color = this.danger.material.color.With(a: 1);
            this.danger.color = this.danger.color.With(a: 1);

            Timer.Instance.Wait(2.0f, () => this.danger.FadeOut(1.5f));
        }

        public void Clear()
        {
            Unhighlight();
            GameObjectsInside.Clear();

            if (!IsOccupied)
            {
                return;
            }

            gameObject.name = this.originalName;
            OccupiedBy.GetComponent<HealthComponent>().Died -= OnEntityDied;
            OccupiedBy = null;
        }

        private void Highlight(float alpha)
        {
            if (!IsOccupied)
            {
                this.highlight.color = Color.white.With(a: alpha);
                return;
            }

            Color color;

            if (OccupiedBy.IsOwnedByPlayer())
            {
                color = Color.green;
            }
            else if (OccupiedBy.IsOwnedByNeutral())
            {
                color = Color.yellow;
            }
            else
            {
                color = Color.red;
            }

            this.highlight.color = color.With(a: alpha);
        }

        private void Unhighlight()
        {
            this.highlight.color = Color.white.With(a: 0);
        }

        public void OnEnter(GameObject entity)
        {
            if (entity.IsMovingViaScript())
            {
                return;
            }

            GameObjectsInside.Add(entity);
            AnyEntityEnterCell?.Invoke(entity, this);

            var unit = entity.GetComponent<UnitComponent>();

            if (unit == null || unit.IsDummy && !unit.IsBlocksMovement)
            {
                return;
            }

            if (!entity.GetComponent<HealthComponent>().IsAlive)
            {
                return;
            }

            Occupy(entity);
        }

        public void OnExit(GameObject entity)
        {
            GameObjectsInside.Remove(entity);

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                // Scenario termination disables entities which is triggering OnTriggerExit events.
                if (entity == null)
                {
                    return;
                }

                AnyEntityExitCell?.Invoke(entity, this);
            });

            if (entity != OccupiedBy)
            {
                return;
            }

            Refuse();
        }

        private void Occupy(GameObject entity)
        {
            if (IsOccupied)
            {
                return;
            }

            OccupiedBy = entity;
            OccupiedBy.GetComponent<HealthComponent>().Died += OnEntityDied;

            gameObject.name = this.originalName + " - " + entity.name;

            AnyCellOccupied?.Invoke(this);
        }

        private void Refuse()
        {
            if (!IsOccupied)
            {
                return;
            }

            gameObject.name = this.originalName;

            OccupiedBy.GetComponent<HealthComponent>().Died -= OnEntityDied;
            OccupiedBy = null;

            AnyCellRefused?.Invoke(this);

            MaybeOccupyByUnitInside();
        }

        private void MaybeOccupyByUnitInside()
        {
            var unitInside = GameObjectsInside.FirstOrDefault(
                inside => !inside.IsDummy() && !inside.IsMovingViaScript() && inside.IsAlive() && inside.IsBlocksMovement());

            if (unitInside == null)
            {
                return;
            }

            Occupy(unitInside);
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            Refuse();
        }

        private void OnScenarioAnyScenarioStarted(Scenario scenario)
        {
            OnMouseExit();
        }

        private void OnAnyUnitTerminated(UnitComponent unit)
        {
            GameObjectsInside.Remove(unit.gameObject);

            if (OccupiedBy != unit.gameObject)
            {
                return;
            }

            Refuse();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnEnter(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnExit(other.gameObject);
        }

        private void OnMouseEnter()
        {
            if (!IsWalkable)
            {
                return;
            }

            if (hovered != null)
            {
                hovered.Unhighlight();
            }

            hovered = this;
            hovered.Highlight(0.3f);

            MouseEnter?.Invoke(this);
            AnyCellMouseEnter?.Invoke(this);
        }

        private void OnMouseExit()
        {
            if (!IsWalkable)
            {
                return;
            }

            if (hovered == this)
            {
                hovered.Unhighlight();
                hovered = null;
            }

            MouseExit?.Invoke(this);
            AnyCellMouseExit?.Invoke(this);
        }

        private void OnMouseDown()
        {
            if (!IsWalkable || UIManager.Instance.IsAnyFullscreenUiOpen())
            {
                return;
            }

            if (hovered == this)
            {
                hovered.Highlight(0.15f);
            }

            MouseDown?.Invoke(this);
        }

        private void OnMouseUp()
        {
            if (!IsWalkable || UIManager.Instance.IsAnyFullscreenUiOpen())
            {
                return;
            }

            if (hovered == this)
            {
                hovered.Highlight(0.3f);
            }

            MouseUp?.Invoke(this);
        }
    }
}