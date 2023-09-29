﻿using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
 using DarkBestiary.Events;
 using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
 using DarkBestiary.Movers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Effects
{
    public class MoveEffect : Effect
    {
        private readonly MoveEffectData m_Data;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly IPathfinder m_Pathfinder;

        private Queue<Vector3> m_Queue;
        private Mover m_Mover;
        private BoardCell m_Destination;
        private ActorComponent m_Actor;
        private BehavioursComponent m_Behaviours;

        public MoveEffect(MoveEffectData data, List<ValidatorWithPurpose> validators,
            IPathfinder pathfinder, BoardNavigator boardNavigator) : base(data, validators)
        {
            m_Data = data;
            m_Pathfinder = pathfinder;
            m_BoardNavigator = boardNavigator;
        }

        protected override Effect New()
        {
            return new MoveEffect(m_Data, Validators, m_Pathfinder, m_BoardNavigator);
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

            m_Destination = BoardNavigator.Instance.WithinCircle(target, 0).FirstOrDefault();

            if (m_Destination == null)
            {
                TriggerFinished();
                return;
            }

            var path = m_Pathfinder.FindPath(caster, m_Destination.transform.position, true);

            var nearest = path.Count > 0 ? path.Last() : caster.transform.position;

            if ((nearest - m_Destination.transform.position).magnitude <= 2.3f)
            {
                path.Add(m_Destination.transform.position);
            }

            if (path.Count == 0)
            {
                TriggerFinished();
                return;
            }

            m_Queue = new Queue<Vector3>(path.Take(GetDistance(caster) + 1));

            m_Actor = caster.GetComponent<ActorComponent>();
            m_Actor.PlayAnimation(m_Data.Animation);

            caster.GetComponent<HealthComponent>().Died += OnDeath;

            m_Behaviours = casterBehaviours;
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;

            m_Mover = Mover.Factory(new MoverData(MoverType.Linear, GetSpeed(caster), 0, 0, false));
            m_Mover.Stopped += OnMoverStopped;

            OnMoverStopped();
        }

        private void OnDeath(EntityDiedEventData data)
        {
            m_Queue.Clear();
            m_Mover.Stop();
        }

        private float GetSpeed(GameObject entity)
        {
            return entity.GetComponent<BehavioursComponent>().IsSlowed
                ? m_Data.Speed / 2
                : m_Data.Speed;
        }

        private int GetDistance(GameObject entity)
        {
            return entity.GetComponent<BehavioursComponent>().IsSlowed
                ? Math.Max(1, m_Data.Distance / 2)
                : m_Data.Distance;
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!m_Behaviours.IsUncontrollable && !m_Behaviours.IsImmobilized)
            {
                return;
            }

            m_Queue.Clear();
            m_Mover.Stop();
        }

        private void OnMoverStopped()
        {
            if (m_Queue.Count > 0)
            {
                var nextPosition = m_Queue.Dequeue();

                m_Mover.Move(m_Actor.gameObject, nextPosition);
                m_Actor.Model.LookAt(nextPosition);
                return;
            }

            m_Mover.Stopped -= OnMoverStopped;

            m_Actor.GetComponent<HealthComponent>().Died -= OnDeath;

            m_Behaviours.BehaviourApplied -= OnBehaviourApplied;

            m_Actor.GetComponent<ActorComponent>().PlayAnimation("idle");
            m_Actor.transform.position = m_Actor.transform.position.Snapped();

            TriggerFinished();
        }
    }
}