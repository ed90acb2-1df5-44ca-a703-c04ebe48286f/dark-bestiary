using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Movers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Effects
{
    public class RunAwayEffect : Effect
    {
        private readonly RunAwayEffectData data;
        private readonly BoardNavigator boardNavigator;
        private readonly IPathfinder pathfinder;

        private Queue<Vector3> path;
        private Mover mover;
        private BoardCell destination;
        private ActorComponent actor;
        private BehavioursComponent behaviours;

        public RunAwayEffect(RunAwayEffectData data, List<ValidatorWithPurpose> validators,
            IPathfinder pathfinder, BoardNavigator boardNavigator) : base(data, validators)
        {
            this.data = data;
            this.pathfinder = pathfinder;
            this.boardNavigator = boardNavigator;
        }

        protected override Effect New()
        {
            return new RunAwayEffect(this.data, this.Validators, this.pathfinder, this.boardNavigator);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (target.GetComponent<BehavioursComponent>().IsImmobilized ||
                target.GetComponent<UnitComponent>().IsMovingViaScript ||
                target.GetComponent<UnitComponent>().IsImmovable)
            {
                TriggerFinished();
                return;
            }

            this.destination = GetDestination(caster, target);

            if (this.destination == null)
            {
                TriggerFinished();
                return;
            }

            this.path = new Queue<Vector3>(this.pathfinder.FindPath(target, this.destination.transform.position, true));

            if (this.path.Count == 0)
            {
                TriggerFinished();
                return;
            }

            this.destination.IsReserved = true;

            this.actor = target.GetComponent<ActorComponent>();
            this.actor.PlayAnimation(this.data.Animation);

            this.behaviours = target.GetComponent<BehavioursComponent>();
            this.behaviours.BehaviourApplied += OnBehaviourApplied;

            this.mover = Mover.Factory(new MoverData(MoverType.Linear, this.data.Speed, 0, 0, false));
            this.mover.Stopped += OnMoverStopped;

            this.actor.GetComponent<UnitComponent>().Flags |= UnitFlags.MovingViaScript;

            OnMoverStopped();
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!this.behaviours.IsUncontrollable && !this.behaviours.IsImmobilized)
            {
                return;
            }

            this.path.Clear();
            this.mover.Stop();
        }

        private BoardCell GetDestination(GameObject caster, GameObject target)
        {
            if (this.data.RandomDirection)
            {
                return this.boardNavigator
                    .WalkableInRadius(target.transform.position, this.data.Distance)
                    .Shuffle()
                    .FirstOrDefault(c => !c.IsReserved && !c.IsOccupied);
            }

            if (this.data.ToCaster)
            {
                return this.boardNavigator
                    .WalkableInRadius(caster.transform.position, 5)
                    .Where(c => !c.IsReserved && !c.IsOccupied)
                    .OrderBy(c => (c.transform.position - caster.transform.position).sqrMagnitude)
                    .FirstOrDefault();
            }

            return GetOpposite(caster, target, 0.5f);
        }

        private BoardCell GetOpposite(GameObject caster, GameObject target, float dot)
        {
            var direction = (target.transform.position - caster.transform.position).normalized;

            return this.boardNavigator
                .WithinCircle(target.transform.position, this.data.Distance)
                .Where(cell => cell.IsWalkable && !cell.IsReserved)
                .Where(cell =>
                    Vector3.Dot(direction, (cell.transform.position - caster.transform.position).normalized) >= dot)
                .OrderByDescending(cell => (cell.transform.position - target.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void OnMoverStopped()
        {
            if (this.path.Count > 0)
            {
                var nextPosition = this.path.Dequeue();

                this.mover.Move(this.actor.gameObject, nextPosition);
                this.actor.Model.LookAt(nextPosition);

                return;
            }

            this.actor.GetComponent<UnitComponent>().Flags &= ~UnitFlags.MovingViaScript;

            this.behaviours.BehaviourApplied -= OnBehaviourApplied;

            this.actor.GetComponent<ActorComponent>().PlayAnimation("idle");
            this.actor.transform.position = this.destination.transform.position;

            this.destination.OnEnter(this.actor.gameObject);
            this.destination.IsReserved = false;

            TriggerFinished();
        }
    }
}