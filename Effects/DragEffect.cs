using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
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
    public class DragEffect : Effect
    {
        public static event Payload<DragEffect> AnyDragEffectFinished;

        private readonly DragEffectData data;
        private readonly IPathfinder pathfinder;

        private BehavioursComponent behaviours;
        private Mover mover;

        public DragEffect(DragEffectData data, List<ValidatorWithPurpose> validators,
            IPathfinder pathfinder) : base(data, validators)
        {
            this.data = data;
            this.pathfinder = pathfinder;
        }

        protected override Effect New()
        {
            return new DragEffect(this.data, this.Validators, this.pathfinder);
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

            caster.GetComponent<ActorComponent>().PlayAnimation(this.data.StartAnimation);

            if (this.data.IsAirborne)
            {
                unit.Flags |= UnitFlags.Airborne;
            }
            else
            {
                this.behaviours = caster.GetComponent<BehavioursComponent>();
                this.behaviours.BehaviourApplied += OnBehaviourApplied;
            }

            this.mover = Mover.Factory(this.data.Mover);
            this.mover.Stopped += OnMoverStopped;

            if (this.data.CollideWithEntityEffectId > 0 || this.data.CollideWithEnvironmentEffectId > 0)
            {
                this.mover.FindAnyCollisionAndMove(caster, target);
            }
            else if (this.data.StopOnCollision)
            {
                this.mover.FindAnyCollisionAndMove(caster, target, (caster.transform.position - target).magnitude);
            }
            else
            {
                this.mover.Move(caster, target);
            }
        }

        private Effect GetEffect(int effectId)
        {
            var effect = Container.Instance.Resolve<IEffectRepository>().Find(effectId);

            return effect == null ? null : Inherit(effect);
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (!this.behaviours.IsUncontrollable && !this.behaviours.IsImmobilized)
            {
                return;
            }

            this.mover.Stop();
        }

        private void OnMoverStopped()
        {
            Caster.GetComponent<ActorComponent>().PlayAnimation(this.data.EndAnimation);

            var unit = Caster.GetComponent<UnitComponent>();
            unit.Flags &= ~UnitFlags.MovingViaScript;

            if (this.data.IsAirborne)
            {
                unit.Flags &= ~UnitFlags.Airborne;
            }

            if (this.behaviours != null)
            {
                this.behaviours.BehaviourApplied -= OnBehaviourApplied;
            }

            this.mover.Stopped -= OnMoverStopped;

            Caster.transform.position = Caster.transform.position.Snapped();
            BoardNavigator.Instance.NearestCell(Caster.transform.position).OnEnter(Caster);

            ApplyEffects();

            AnyDragEffectFinished?.Invoke(this);
            TriggerFinished();
        }

        private void ApplyEffects()
        {
            if (this.mover.CollidesWithEntity)
            {
                GetEffect(this.data.CollideWithEntityEffectId)?.Apply(Caster, this.mover.CollidesWithEntity);
            }
            else if (this.mover.CollidesWithEnvironment)
            {
                GetEffect(this.data.CollideWithEnvironmentEffectId)?.Apply(Caster, Caster);
            }
            else
            {
                GetEffect(this.data.FinalEffectId)?.Apply(Caster, Caster);
            }
        }
    }
}