using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Movers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Effects
{
    public class DragEffect : Effect
    {
        public static event Action<DragEffect> AnyDragEffectFinished;

        private readonly DragEffectData m_Data;
        private readonly IPathfinder m_Pathfinder;

        private BehavioursComponent m_Behaviours;
        private Mover m_Mover;

        public DragEffect(DragEffectData data, List<ValidatorWithPurpose> validators,
            IPathfinder pathfinder) : base(data, validators)
        {
            m_Data = data;
            m_Pathfinder = pathfinder;
        }

        protected override Effect New()
        {
            return new DragEffect(m_Data, Validators, m_Pathfinder);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var unit = caster.GetComponent<UnitComponent>();

            if (unit.IsMovingViaScript)
            {
                TriggerFinished();
                return;
            }

            unit.Flags |= UnitFlags.MovingViaScript;

            caster.GetComponent<ActorComponent>().PlayAnimation(m_Data.StartAnimation);

            if (m_Data.IsAirborne)
            {
                unit.Flags |= UnitFlags.Airborne;
            }
            else
            {
                m_Behaviours = caster.GetComponent<BehavioursComponent>();
                m_Behaviours.BehaviourApplied += OnBehaviourApplied;
            }

            m_Mover = Mover.Factory(m_Data.Mover);
            m_Mover.Stopped += OnMoverStopped;

            if (m_Data.CollideWithEntityEffectId > 0 || m_Data.CollideWithEnvironmentEffectId > 0)
            {
                m_Mover.FindAnyCollisionAndMove(caster, target);
            }
            else if (m_Data.StopOnCollision)
            {
                m_Mover.FindAnyCollisionAndMove(caster, target, (caster.transform.position - target).magnitude);
            }
            else
            {
                m_Mover.Move(caster, target);
            }
        }

        private Effect GetEffect(int effectId)
        {
            var effect = Container.Instance.Resolve<IEffectRepository>().Find(effectId);

            return effect == null ? null : Inherit(effect);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!m_Behaviours.IsUncontrollable && !m_Behaviours.IsImmobilized)
            {
                return;
            }

            m_Mover.Stop();
        }

        private void OnMoverStopped()
        {
            Caster.GetComponent<ActorComponent>().PlayAnimation(m_Data.EndAnimation);

            var unit = Caster.GetComponent<UnitComponent>();
            unit.Flags &= ~UnitFlags.MovingViaScript;

            if (m_Data.IsAirborne)
            {
                unit.Flags &= ~UnitFlags.Airborne;
            }

            if (m_Behaviours != null)
            {
                m_Behaviours.BehaviourApplied -= OnBehaviourApplied;
            }

            m_Mover.Stopped -= OnMoverStopped;

            Caster.transform.position = Caster.transform.position.Snapped();
            BoardNavigator.Instance.NearestCell(Caster.transform.position).OnEnter(Caster);

            ApplyEffects();

            AnyDragEffectFinished?.Invoke(this);
            TriggerFinished();
        }

        private void ApplyEffects()
        {
            if (m_Mover.CollidesWithEntity)
            {
                GetEffect(m_Data.CollideWithEntityEffectId)?.Apply(Caster, m_Mover.CollidesWithEntity);
            }
            else if (m_Mover.CollidesWithEnvironment)
            {
                GetEffect(m_Data.CollideWithEnvironmentEffectId)?.Apply(Caster, Caster);
            }
            else
            {
                GetEffect(m_Data.FinalEffectId)?.Apply(Caster, Caster);
            }
        }
    }
}