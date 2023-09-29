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
        private readonly RunAwayEffectData m_Data;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly IPathfinder m_Pathfinder;

        private Queue<Vector3> m_Path;
        private Mover m_Mover;
        private BoardCell m_Destination;
        private ActorComponent m_Actor;
        private BehavioursComponent m_Behaviours;

        public RunAwayEffect(RunAwayEffectData data, List<ValidatorWithPurpose> validators,
            IPathfinder pathfinder, BoardNavigator boardNavigator) : base(data, validators)
        {
            m_Data = data;
            m_Pathfinder = pathfinder;
            m_BoardNavigator = boardNavigator;
        }

        protected override Effect New()
        {
            return new RunAwayEffect(m_Data, Validators, m_Pathfinder, m_BoardNavigator);
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

            m_Destination = GetDestination(caster, target);

            if (m_Destination == null)
            {
                TriggerFinished();
                return;
            }

            m_Path = new Queue<Vector3>(m_Pathfinder.FindPath(target, m_Destination.transform.position, true));

            if (m_Path.Count == 0)
            {
                TriggerFinished();
                return;
            }

            m_Destination.IsReserved = true;

            m_Actor = target.GetComponent<ActorComponent>();
            m_Actor.PlayAnimation(m_Data.Animation);

            m_Behaviours = target.GetComponent<BehavioursComponent>();
            m_Behaviours.BehaviourApplied += OnBehaviourApplied;

            m_Mover = Mover.Factory(new MoverData(MoverType.Linear, m_Data.Speed, 0, 0, false));
            m_Mover.Stopped += OnMoverStopped;

            m_Actor.GetComponent<UnitComponent>().Flags |= UnitFlags.MovingViaScript;

            OnMoverStopped();
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!m_Behaviours.IsUncontrollable && !m_Behaviours.IsImmobilized)
            {
                return;
            }

            m_Path.Clear();
            m_Mover.Stop();
        }

        private BoardCell GetDestination(GameObject caster, GameObject target)
        {
            if (m_Data.RandomDirection)
            {
                return m_BoardNavigator
                    .WalkableInRadius(target.transform.position, m_Data.Distance)
                    .Shuffle()
                    .FirstOrDefault(c => !c.IsReserved && !c.IsOccupied);
            }

            if (m_Data.ToCaster)
            {
                return m_BoardNavigator
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

            return m_BoardNavigator
                .WithinCircle(target.transform.position, m_Data.Distance)
                .Where(cell => cell.IsWalkable && !cell.IsReserved)
                .Where(cell =>
                    Vector3.Dot(direction, (cell.transform.position - caster.transform.position).normalized) >= dot)
                .OrderByDescending(cell => (cell.transform.position - target.transform.position).sqrMagnitude)
                .FirstOrDefault();
        }

        private void OnMoverStopped()
        {
            if (m_Path.Count > 0)
            {
                var nextPosition = m_Path.Dequeue();

                m_Mover.Move(m_Actor.gameObject, nextPosition);
                m_Actor.Model.LookAt(nextPosition);

                return;
            }

            m_Actor.GetComponent<UnitComponent>().Flags &= ~UnitFlags.MovingViaScript;

            m_Behaviours.BehaviourApplied -= OnBehaviourApplied;

            m_Actor.GetComponent<ActorComponent>().PlayAnimation("idle");
            m_Actor.transform.position = m_Destination.transform.position;

            m_Destination.OnEnter(m_Actor.gameObject);
            m_Destination.IsReserved = false;

            TriggerFinished();
        }
    }
}