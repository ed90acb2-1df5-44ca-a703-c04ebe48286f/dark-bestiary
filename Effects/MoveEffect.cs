﻿using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Movers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Effects
{
    public class MoveEffect : Effect
    {
        private readonly MoveEffectData data;
        private readonly BoardNavigator boardNavigator;
        private readonly IPathfinder pathfinder;

        private Queue<Vector3> queue;
        private Mover mover;
        private BoardCell destination;
        private ActorComponent actor;
        private BehavioursComponent behaviours;

        public MoveEffect(MoveEffectData data, List<ValidatorWithPurpose> validators,
            IPathfinder pathfinder, BoardNavigator boardNavigator) : base(data, validators)
        {
            this.data = data;
            this.pathfinder = pathfinder;
            this.boardNavigator = boardNavigator;
        }

        protected override Effect New()
        {
            return new MoveEffect(this.data, this.Validators, this.pathfinder, this.boardNavigator);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var casterBehaviours = caster.GetComponent<BehavioursComponent>();

            if (casterBehaviours.IsImmobilized || casterBehaviours.IsUncontrollable)
            {
                TriggerFinished();
                return;
            }

            this.destination = BoardNavigator.Instance.WithinCircle(target, 0).FirstOrDefault();

            if (this.destination == null)
            {
                TriggerFinished();
                return;
            }

            var path = this.pathfinder.FindPath(caster, this.destination.transform.position, true);

            var nearest = path.Count > 0 ? path.Last() : caster.transform.position;

            if ((nearest - this.destination.transform.position).magnitude <= 2.3f)
            {
                path.Add(this.destination.transform.position);
            }

            if (path.Count == 0)
            {
                TriggerFinished();
                return;
            }

            this.queue = new Queue<Vector3>(path.Take(GetDistance(caster) + 1));

            this.actor = caster.GetComponent<ActorComponent>();
            this.actor.PlayAnimation(this.data.Animation);

            caster.GetComponent<HealthComponent>().Died += OnDeath;

            this.behaviours = casterBehaviours;
            this.behaviours.BehaviourApplied += OnBehaviourApplied;

            this.mover = Mover.Factory(new MoverData(MoverType.Linear, GetSpeed(caster), 0, 0, false));
            this.mover.Stopped += OnMoverStopped;

            OnMoverStopped();
        }

        private void OnDeath(EntityDiedEventData data)
        {
            this.queue.Clear();
            this.mover.Stop();
        }

        private float GetSpeed(GameObject entity)
        {
            return entity.GetComponent<BehavioursComponent>().IsSlowed
                ? this.data.Speed / 2
                : this.data.Speed;
        }

        private int GetDistance(GameObject entity)
        {
            return entity.GetComponent<BehavioursComponent>().IsSlowed
                ? Math.Max(1, this.data.Distance / 2)
                : this.data.Distance;
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!this.behaviours.IsUncontrollable && !this.behaviours.IsImmobilized)
            {
                return;
            }

            this.queue.Clear();
            this.mover.Stop();
        }

        private void OnMoverStopped()
        {
            if (this.queue.Count > 0)
            {
                var nextPosition = this.queue.Dequeue();

                this.mover.Move(this.actor.gameObject, nextPosition);
                this.actor.Model.LookAt(nextPosition);
                return;
            }

            this.mover.Stopped -= OnMoverStopped;

            this.actor.GetComponent<HealthComponent>().Died -= OnDeath;

            this.behaviours.BehaviourApplied -= OnBehaviourApplied;

            this.actor.GetComponent<ActorComponent>().PlayAnimation("idle");
            this.actor.transform.position = this.actor.transform.position.Snapped();

            TriggerFinished();
        }
    }
}