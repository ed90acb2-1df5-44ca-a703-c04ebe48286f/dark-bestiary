using System;
using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Movers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Components
{
    public class MovementComponent : Component
    {
        private const int c_ActionPointCostPerCell = 2;

        public static event Action<MovementComponent> AnyMovementStarted;
        public static event Action<MovementComponent> AnyMovementStopped;

        public event Action<MovementComponent> Started;
        public event Action<MovementComponent> Stopped;

        public bool IsMoving { get; private set; }

        private Mover m_Mover;
        private IPathfinder m_Pathfinder;

        private ResourcesComponent m_Resources;
        private BehavioursComponent m_Behaviours;
        private Vector3 m_CurrentPoint;
        private int m_CurrentPointIndex;
        private ActorComponent m_Actor;
        private List<Vector3> m_Points;

        public MovementComponent Construct(IPathfinder pathfinder)
        {
            m_Pathfinder = pathfinder;

            m_Mover = new LinearMover(new MoverData(MoverType.Linear, 6, 0, 0, false));
            m_Mover.Stopped += OnMoverFinish;

            return this;
        }

        protected override void OnInitialize()
        {
            Scenario.AnyScenarioExit += ScenarioOnAnyScenarioExit;

            m_Actor = GetComponent<ActorComponent>();
            m_Resources = GetComponent<ResourcesComponent>();

            m_Behaviours = GetComponent<BehavioursComponent>();
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;

            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;

            GetComponent<HealthComponent>().Died += OnEntityDied;
        }

        protected override void OnTerminate()
        {
            Scenario.AnyScenarioExit -= ScenarioOnAnyScenarioExit;

            m_Mover.Stopped -= OnMoverFinish;

            Stop();

            m_Behaviours.BehaviourApplied -= OnBehaviourApplied;

            Episode.AnyEpisodeStarted -= OnAnyEpisodeStarted;

            GetComponent<HealthComponent>().Died -= OnEntityDied;
        }

        public bool CanMove()
        {
            return !m_Behaviours.IsImmobilized && !m_Behaviours.IsUncontrollable && HasEnoughActionPoints();
        }

        public void Move(Vector3 destination, bool partial = true)
        {
            if (IsMoving)
            {
                return;
            }

            Started?.Invoke(this);
            AnyMovementStarted?.Invoke(this);
            IsMoving = true;

            if (!CanMove())
            {
                Stop();
                return;
            }

            FindPathAndMove(destination, partial);
        }

        private void FindPathAndMove(Vector3 destination, bool partial)
        {
            m_Pathfinder.FindPathAsync(gameObject, destination, partial, points =>
            {
                m_Points = points;

                if (m_Points.Count == 0)
                {
                    Stop();
                    return;
                }

                m_CurrentPointIndex = 0;
                m_Actor.Model.LookAt(m_Points[m_CurrentPointIndex]);
                m_Actor.PlayAnimation("walk");

                OnMoverFinish();
            });
        }

        public void Stop()
        {
            if (!IsMoving)
            {
                return;
            }

            IsMoving = false;

            transform.position = transform.position.Snapped();
            m_Actor.PlayAnimation("idle");
            m_Mover.Stop();

            Stopped?.Invoke(this);
            AnyMovementStopped?.Invoke(this);
        }

        public bool HasEnoughResourcesToCompletePath(List<Vector3> path)
        {
            if (!Encounter.IsCombat)
            {
                return true;
            }

            return path.Count - 1 <= CalculateAvailableTravelRangeCells();
        }

        public int CalculateAvailableTravelRangeCells()
        {
            if (!Encounter.IsCombat)
            {
                return 32;
            }

            return (int) (m_Resources.Get(ResourceType.ActionPoint).Amount / GetMovementCost());
        }

        public int GetMovementCost(int cells = 1)
        {
            var cost = c_ActionPointCostPerCell;

            if (m_Behaviours.IsSlowed)
            {
                cost *= 2;
            }

            if (m_Behaviours.IsSwift)
            {
                cost /= 2;
            }

            return cost * cells;
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (m_Behaviours.IsImmobilized || m_Behaviours.IsUncontrollable)
            {
                Stop();
            }
        }

        private void OnAnyEpisodeStarted(Episode episode)
        {
            Stop();
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            Stop();
        }

        private void ScenarioOnAnyScenarioExit(Scenario scenario)
        {
            Stop();
        }

        private void OnMoverFinish()
        {
            if (!IsMoving)
            {
                return;
            }

            m_CurrentPointIndex++;

            if (m_CurrentPointIndex >= m_Points.Count || !HasEnoughActionPoints())
            {
                transform.position = m_CurrentPoint;
                Stop();
                return;
            }

            ConsumeActionPoints();
            AdjustMoverSpeed();

            m_CurrentPoint = m_Points[m_CurrentPointIndex];
            m_Actor.Model.LookAt(m_CurrentPoint);
            m_Mover.Move(gameObject, m_CurrentPoint);
        }

        private void AdjustMoverSpeed()
        {
            m_Mover.Speed = m_Mover.DefaultSpeed;

            if (m_Behaviours.IsSwift)
            {
                m_Mover.Speed *= 2;
            }

            if (m_Behaviours.IsSlowed)
            {
                m_Mover.Speed /= 2;
            }
        }

        public bool HasEnoughActionPoints()
        {
            return !Encounter.IsCombat || m_Resources.HasEnough(ResourceType.ActionPoint, GetMovementCost());
        }

        private void ConsumeActionPoints()
        {
            if (!Encounter.IsCombat)
            {
                return;
            }

            m_Resources.Consume(ResourceType.ActionPoint, GetMovementCost());
        }
    }
}