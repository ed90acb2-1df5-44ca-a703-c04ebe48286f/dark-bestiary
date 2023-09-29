using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.GameBoard
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoardCell : MonoBehaviour
    {
        public static event Action<GameObject, BoardCell>? AnyEntityEnterCell;
        public static event Action<GameObject, BoardCell>? AnyEntityExitCell;
        public static event Action<BoardCell>? AnyCellMouseEnter;
        public static event Action<BoardCell>? AnyCellMouseExit;
        public static event Action<BoardCell>? AnyCellOccupied;
        public static event Action<BoardCell>? AnyCellRefused;

        public event Action<BoardCell>? MouseUp;
        public event Action<BoardCell>? MouseDown;
        public event Action<BoardCell>? MouseEnter;
        public event Action<BoardCell>? MouseExit;

        public int Index;
        public int X;
        public int Y;

        public GameObject? OccupiedBy { get; private set; }
        public HashSet<GameObject> GameObjectsInside { get; } = new();
        public bool IsOccupied => OccupiedBy != null;
        public bool IsAvailable { get; private set; }

        public bool IsWalkable
        {
            get => m_IsWalkable && gameObject.activeSelf;
            set => m_IsWalkable = value;
        }

        public bool IsReserved { get; set; }

        private static BoardCell? s_Hovered;

        [SerializeField] private BoardCellHitbox m_Hitbox = null!;
        [SerializeField] private SpriteRenderer m_Highlight = null!;
        [SerializeField] private SpriteRenderer m_Danger = null!;

        private SpriteRenderer m_Renderer;
        private string m_OriginalName;
        private bool m_IsHovered;
        private bool m_IsWalkable;

        public void Initialize()
        {
            UnitComponent.AnyUnitTerminated += OnAnyUnitTerminated;
            Scenario.AnyScenarioStarted += OnScenarioAnyScenarioStarted;

            m_OriginalName = name;
            m_Renderer = GetComponent<SpriteRenderer>();
            m_Hitbox.Initialize(this);

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
            m_Renderer.color = SettingsManager.Instance.HighContrastMode ? color.With(a: color.a + 0.5f) : color;
        }

        public void NotAvailable()
        {
            IsAvailable = false;
            m_Renderer.color = Color.white.With(a: 0);
        }

        public void Dangerous()
        {
            m_Danger.material.color = m_Danger.material.color.With(a: 1);
            m_Danger.color = m_Danger.color.With(a: 1);

            Timer.Instance.Wait(5.0f, () => m_Danger.FadeOut(1.5f));
        }

        public void Clear()
        {
            Unhighlight();
            GameObjectsInside.Clear();

            if (!IsOccupied)
            {
                return;
            }

            gameObject.name = m_OriginalName;
            OccupiedBy.GetComponent<HealthComponent>().Died -= OnEntityDied;
            OccupiedBy = null;
        }

        private void Highlight(float alpha)
        {
            if (!IsOccupied)
            {
                m_Highlight.color = Color.white.With(a: alpha);
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

            m_Highlight.color = color.With(a: alpha);
        }

        private void Unhighlight()
        {
            m_Highlight.color = Color.white.With(a: 0);
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

            gameObject.name = m_OriginalName + " - " + entity.name;

            AnyCellOccupied?.Invoke(this);
        }

        private void Refuse()
        {
            if (!IsOccupied)
            {
                return;
            }

            gameObject.name = m_OriginalName;

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

            if (s_Hovered != null)
            {
                s_Hovered.Unhighlight();
            }

            s_Hovered = this;
            s_Hovered.Highlight(0.3f);

            MouseEnter?.Invoke(this);
            AnyCellMouseEnter?.Invoke(this);
        }

        private void OnMouseExit()
        {
            if (!IsWalkable)
            {
                return;
            }

            if (s_Hovered == this)
            {
                s_Hovered.Unhighlight();
                s_Hovered = null;
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

            if (s_Hovered == this)
            {
                s_Hovered.Highlight(0.15f);
            }

            MouseDown?.Invoke(this);
        }

        private void OnMouseUp()
        {
            if (!IsWalkable || UIManager.Instance.IsAnyFullscreenUiOpen())
            {
                return;
            }

            if (s_Hovered == this)
            {
                s_Hovered.Highlight(0.3f);
            }

            MouseUp?.Invoke(this);
        }
    }
}