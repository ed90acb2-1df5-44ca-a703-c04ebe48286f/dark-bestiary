using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Movers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class DragEffect : Effect
    {
        public static event Payload<DragEffect> AnyDragEffectFinished;

        private readonly DragEffectData data;
        private readonly IPathfinder pathfinder;

        private Mover mover;

        public DragEffect(DragEffectData data, List<Validator> validators,
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
            caster.GetComponent<ActorComponent>().PlayAnimation(this.data.StartAnimation);

            if (this.data.IsAirborne)
            {
                caster.GetComponent<UnitComponent>().Flags |= UnitFlags.Airborne;
            }

            this.mover = Mover.Factory(this.data.Mover);
            this.mover.Finished += OnMoverFinished;
            this.mover.CollidingWithEntity += OnCollidingWithEntity;
            this.mover.CollidingWithEnvironment += OnCollidingWithEnvironment;
            this.mover.Start(caster, target);
        }

        private void OnCollidingWithEnvironment()
        {
            var effect = Container.Instance.Resolve<IEffectRepository>().Find(this.data.CollideWithEnvironmentEffectId);
            effect?.Apply(Caster, Caster);

            if (this.data.StopOnEnvironmentCollision)
            {
                this.mover.Stop();
            }
        }

        private void OnCollidingWithEntity(GameObject entity)
        {
            var effect = Container.Instance.Resolve<IEffectRepository>().Find(this.data.CollideWithEntityEffectId);
            effect?.Apply(Caster, entity);

            if (this.data.StopOnEntityCollision)
            {
                this.mover.Stop();
            }
        }

        private void OnMoverFinished()
        {
            Container.Instance.Resolve<IEffectRepository>()
                .Find(this.data.FinalEffectId)?
                .Apply(Caster, Caster);

            Caster.GetComponent<ActorComponent>().PlayAnimation(this.data.EndAnimation);
            Caster.transform.position = Caster.transform.position.Snapped();

            if (this.data.IsAirborne)
            {
                Caster.GetComponent<UnitComponent>().Flags &= ~UnitFlags.Airborne;
            }

            this.mover.CollidingWithEnvironment -= OnCollidingWithEnvironment;
            this.mover.CollidingWithEntity -= OnCollidingWithEntity;
            this.mover.Finished -= OnMoverFinished;

            AnyDragEffectFinished?.Invoke(this);

            TriggerFinished();
        }
    }
}