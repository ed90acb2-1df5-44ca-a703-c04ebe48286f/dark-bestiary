using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Movers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class LaunchMissileEffect : Effect
    {
        protected readonly LaunchMissileEffectData Data;
        protected readonly IMissileRepository MissileRepository;

        public LaunchMissileEffect(LaunchMissileEffectData data, List<Validator> validators,
            IMissileRepository missileRepository) : base(data, validators)
        {
            this.Data = data;
            this.MissileRepository = missileRepository;
        }

        protected override Effect New()
        {
            return new LaunchMissileEffect(this.Data, this.Validators, this.MissileRepository);
        }

        public virtual Missile GetMissile()
        {
            return this.MissileRepository.FindOrFail(this.Data.MissileId);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var origin = caster;

            if (Origin is GameObject gameObject)
            {
                origin = gameObject;
            }

            Launch(
                caster,
                target,
                GetAttachmentPoint(origin, this.Data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target.transform.position),
                GetAttachmentPoint(target, this.Data.TargetAttachmentPoint) + GetTargetOffset()
            );
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var origin = caster;

            if (Origin is GameObject gameObject)
            {
                origin = gameObject;
            }

            Launch(
                caster,
                target,
                GetAttachmentPoint(origin, this.Data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target),
                target + GetTargetOffset()
            );
        }

        private Vector3 GetCasterOffset(Vector3 origin, Vector3 target)
        {
            var direction = (target - origin).normalized;

            return new Vector3(this.Data.CasterOffsetX, this.Data.CasterOffsetY) +
                   new Vector3(this.Data.DirectionalOffsetX * direction.x, this.Data.DirectionalOffsetY * direction.y);
        }

        private Vector3 GetTargetOffset()
        {
            return new Vector3(this.Data.TargetOffsetX, this.Data.TargetOffsetY);
        }

        protected void Launch(GameObject caster, object target, Vector3 origin, Vector3 destination)
        {
            var missile = GetMissile();
            AssignEffects(missile);

            missile.Mover = Mover.Factory(this.Data.Mover);

            if (!this.Data.FinishImmediately)
            {
                missile.Finished += OnMissileFinished;
            }

            missile.FlyHeight = this.Data.MissileFlyHeight;
            missile.StopOnEntityCollision = this.Data.StopOnEntityCollision;
            missile.StopOnEnvironmentCollision = this.Data.StopOnEnvironmentCollision;
            missile.gameObject.SetActive(false);
            missile.transform.position = origin;
            missile.Launch(caster, target, destination);
            missile.gameObject.SetActive(true);

            if (this.Data.FinishImmediately)
            {
                TriggerFinished();
            }
        }

        private void AssignEffects(Missile missile)
        {
            var effectRepository = Container.Instance.Resolve<IEffectRepository>();

            var finalEffect = effectRepository.Find(this.Data.FinalEffectId);

            if (finalEffect != null)
            {
                missile.FinalEffect = Inherit(finalEffect);
            }

            var collideWithEntitiesEffect = effectRepository.Find(this.Data.CollideWithEntitiesEffectId);

            if (collideWithEntitiesEffect != null)
            {
                missile.CollideWithEntitiesEffect = Inherit(collideWithEntitiesEffect);
            }

            var collideWithEnvironmentEffect = effectRepository.Find(this.Data.CollideWithEnvironmentEffectId);

            if (collideWithEnvironmentEffect != null)
            {
                missile.CollideWithEnvironmentEffect = Inherit(collideWithEnvironmentEffect);
            }

            var enterCellEffect = effectRepository.Find(this.Data.EnterCellEffectId);

            if (enterCellEffect != null)
            {
                missile.EnterCellEffect = Inherit(enterCellEffect);
            }

            var exitCellEffect = effectRepository.Find(this.Data.ExitCellEffectId);

            if (exitCellEffect != null)
            {
                missile.ExitCellEffect = Inherit(exitCellEffect);
            }
        }

        private void OnMissileFinished(Missile missile)
        {
            TriggerFinished();
        }

        protected static Vector3 GetAttachmentPoint(GameObject entity, AttachmentPoint attachmentPointType)
        {
            return entity.GetComponent<ActorComponent>().Model.GetAttachmentPoint(attachmentPointType).position;
        }
    }
}