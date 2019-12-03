using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
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
        private const int ActionPointCostPerCell = 2;

        public event Payload<MovementComponent> Started;
        public event Payload<MovementComponent> Stopped;

        public bool IsMoving { get; private set; }

        private Mover mover;
        private IPathfinder pathfinder;

        private ResourcesComponent resources;
        private BehavioursComponent behaviours;
        private Vector3 currentPoint;
        private int currentPointIndex;
        private ActorComponent actor;
        private List<Vector3> points;

        public MovementComponent Construct(IPathfinder pathfinder)
        {
            this.pathfinder = pathfinder;

            this.mover = new LinearMover(new MoverData(MoverType.Linear, 6, 0, 0, false));
            this.mover.Finished += OnMoverFinish;

            return this;
        }

        protected override void OnInitialize()
        {
            Scenario.AnyScenarioExit += ScenarioOnAnyScenarioExit;

            this.actor = GetComponent<ActorComponent>();
            this.resources = GetComponent<ResourcesComponent>();

            this.behaviours = GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;

            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;

            GetComponent<HealthComponent>().Died += OnEntityDied;
        }

        protected override void OnTerminate()
        {
            Scenario.AnyScenarioExit -= ScenarioOnAnyScenarioExit;

            this.mover.Finished -= OnMoverFinish;

            Stop();

            this.behaviours.BehaviourApplied -= OnBehaviourApplied;

            Episode.AnyEpisodeStarted -= OnAnyEpisodeStarted;

            GetComponent<HealthComponent>().Died -= OnEntityDied;
        }

        public bool CanMove()
        {
            return !this.behaviours.IsImmobilized && !this.behaviours.IsUncontrollable && HasEnoughActionPoints();
        }

        public void Move(Vector3 destination, bool partial = true)
        {
            if (IsMoving)
            {
                return;
            }

            Started?.Invoke(this);
            IsMoving = true;

            if (!CanMove() || !HasEnoughActionPoints())
            {
                Stop();
                return;
            }

            FindPathAndMove(destination, partial);
        }

        private void FindPathAndMove(Vector3 destination, bool partial)
        {
            this.pathfinder.FindPathAsync(gameObject, destination, partial, points =>
            {
                this.points = points;

                if (this.points.Count == 0)
                {
                    Stop();
                    return;
                }

                this.currentPointIndex = 0;
                this.actor.Model.LookAt(this.points[this.currentPointIndex]);
                this.actor.PlayAnimation("walk");

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
            this.actor.PlayAnimation("idle");
            this.mover.Stop();

            Stopped?.Invoke(this);
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

            return (int) (this.resources.Get(ResourceType.ActionPoint).Amount / GetMovementCost());
        }

        public int GetMovementCost(int cells = 1)
        {
            var cost = ActionPointCostPerCell;

            if (this.behaviours.IsSlowed)
            {
                cost *= 2;
            }

            if (this.behaviours.IsSwift)
            {
                cost /= 2;
            }

            return cost * cells;
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (this.behaviours.IsImmobilized || this.behaviours.IsUncontrollable)
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

            this.currentPointIndex++;

            if (this.currentPointIndex >= this.points.Count || !HasEnoughActionPoints())
            {
                Stop();
                return;
            }

            ConsumeActionPoints();
            AdjustMoverSpeed();

            this.currentPoint = this.points[this.currentPointIndex];
            this.actor.Model.LookAt(this.currentPoint);
            this.mover.Start(gameObject, this.currentPoint);
        }

        private void AdjustMoverSpeed()
        {
            this.mover.Speed = this.mover.DefaultSpeed;

            if (this.behaviours.IsSwift)
            {
                this.mover.Speed *= 2;
            }

            if (this.behaviours.IsSlowed)
            {
                this.mover.Speed /= 2;
            }
        }

        public bool HasEnoughActionPoints()
        {
            return !Encounter.IsCombat || this.resources.HasEnough(ResourceType.ActionPoint, GetMovementCost());
        }

        private void ConsumeActionPoints()
        {
            if (!Encounter.IsCombat)
            {
                return;
            }

            this.resources.Consume(ResourceType.ActionPoint, GetMovementCost());
        }
    }
}