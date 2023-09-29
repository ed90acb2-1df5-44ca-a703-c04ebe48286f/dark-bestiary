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

        public LaunchMissileEffect(LaunchMissileEffectData data, List<ValidatorWithPurpose> validators,
            IMissileRepository missileRepository) : base(data, validators)
        {
            Data = data;
            MissileRepository = missileRepository;
        }

        protected override Effect New()
        {
            return new LaunchMissileEffect(Data, Validators, MissileRepository);
        }

        public virtual Missile GetMissile()
        {
            return MissileRepository.Find(Data.MissileId);
        }

        public Effect GetFinalEffect()
        {
            return Container.Instance.Resolve<IEffectRepository>().Find(Data.FinalEffectId);
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
                GetAttachmentPoint(origin, Data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target.transform.position),
                GetAttachmentPoint(target, Data.TargetAttachmentPoint) + GetTargetOffset(origin.transform.position, target.transform.position)
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
                GetAttachmentPoint(origin, Data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target),
                target + GetTargetOffset(origin.transform.position, target)
            );
        }

        private Vector3 GetCasterOffset(Vector3 origin, Vector3 target)
        {
            var direction = (target - origin).normalized;

            return new Vector3(Data.CasterOffsetX, Data.CasterOffsetY) +
                   new Vector3(Data.CasterDirectionalOffsetX * direction.x, Data.CasterDirectionalOffsetY * direction.y);
        }

        private Vector3 GetTargetOffset(Vector3 origin, Vector3 target)
        {
            var direction = (target - origin).normalized;

            return new Vector3(Data.TargetOffsetX, Data.TargetOffsetY) +
                   new Vector3(Data.TargetDirectionalOffsetX * direction.x, Data.TargetDirectionalOffsetY * direction.y);
        }

        protected void Launch(GameObject caster, object target, Vector3 origin, Vector3 destination)
        {
            var missile = GetMissile();
            AssignEffects(missile);

            missile.Mover = Mover.Factory(Data.Mover);

            if (!Data.FinishImmediately)
            {
                missile.Finished += OnMissileFinished;
            }

            missile.FlyHeight = Data.MissileFlyHeight;
            missile.IsPiercing = Data.IsPiercing;
            missile.gameObject.SetActive(false);
            missile.transform.position = origin;
            missile.gameObject.SetActive(true);
            missile.Launch(caster, target, destination);

            if (Data.FinishImmediately)
            {
                TriggerFinished();
            }
        }

        private void AssignEffects(Missile missile)
        {
            var effectRepository = Container.Instance.Resolve<IEffectRepository>();

            var finalEffect = effectRepository.Find(Data.FinalEffectId);

            if (finalEffect != null)
            {
                missile.FinalEffect = Inherit(finalEffect);
            }

            var collideWithEntitiesEffect = effectRepository.Find(Data.CollideWithEntitiesEffectId);

            if (collideWithEntitiesEffect != null)
            {
                missile.CollideWithEntitiesEffect = Inherit(collideWithEntitiesEffect);
            }

            var collideWithEnvironmentEffect = effectRepository.Find(Data.CollideWithEnvironmentEffectId);

            if (collideWithEnvironmentEffect != null)
            {
                missile.CollideWithEnvironmentEffect = Inherit(collideWithEnvironmentEffect);
            }

            var enterCellEffect = effectRepository.Find(Data.EnterCellEffectId);

            if (enterCellEffect != null)
            {
                missile.EnterCellEffect = Inherit(enterCellEffect);
            }

            var exitCellEffect = effectRepository.Find(Data.ExitCellEffectId);

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