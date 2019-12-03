using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Items;
using DarkBestiary.Movers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class LaunchAOEMissileEffect : Effect
    {
        private readonly LaunchAOEMissileEffectData data;
        private readonly List<Validator> validators;
        private readonly IMissileRepository missileRepository;
        private readonly IEffectRepository effectRepository;

        private readonly List<GameObject> hits = new List<GameObject>();

        public LaunchAOEMissileEffect(LaunchAOEMissileEffectData data, List<Validator> validators,
            IMissileRepository missileRepository, IEffectRepository effectRepository) : base(data, new List<Validator>())
        {
            this.data = data;
            this.validators = validators;
            this.missileRepository = missileRepository;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new LaunchAOEMissileEffect(
                this.data, this.validators, this.missileRepository, this.effectRepository);
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
                GetAttachmentPoint(origin, this.data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target.transform.position),
                GetAttachmentPoint(target, this.data.TargetAttachmentPoint) + GetTargetOffset()
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
                GetAttachmentPoint(origin, this.data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target),
                target + GetTargetOffset()
            );
        }

        private Vector3 GetCasterOffset(Vector3 origin, Vector3 target)
        {
            var direction = (target - origin).normalized;

            return new Vector3(this.data.CasterOffsetX, this.data.CasterOffsetY) +
                   new Vector3(this.data.DirectionalOffsetX * direction.x, this.data.DirectionalOffsetY * direction.y);
        }

        private Vector3 GetTargetOffset()
        {
            return new Vector3(this.data.TargetOffsetX, this.data.TargetOffsetY);
        }

        protected void Launch(GameObject caster, object target, Vector3 origin, Vector3 destination)
        {
            var missile = this.missileRepository.FindOrFail(this.data.MissileId);

            AssignEffects(missile);

            missile.Mover = Mover.Factory(this.data.Mover);
            missile.Mover.Finished += OnMoverFinished;
            missile.EnterCell += OnMissileEnterCell;
            missile.FlyHeight = this.data.MissileFlyHeight;
            missile.StopOnEntityCollision = this.data.StopOnEntityCollision;
            missile.StopOnEnvironmentCollision = this.data.StopOnEnvironmentCollision;
            missile.gameObject.SetActive(false);
            missile.transform.position = origin;
            missile.Launch(caster, target, destination);
            missile.gameObject.SetActive(true);
        }

        private void OnMissileEnterCell(BoardCell cell)
        {
            var entities = BoardNavigator.Instance
                .WithinCircle(cell.transform.position, this.data.Radius)
                .ToEntities()
                .Where(entity => !this.hits.Contains(entity) && this.validators.All(v => v.Validate(Caster, entity)))
                .ToList();

            foreach (var entity in entities)
            {
                this.effectRepository.Find(this.data.EffectId).Apply(Caster, entity);
                this.hits.Add(entity);
            }
        }

        private void OnMoverFinished()
        {
            TriggerFinished();
        }

        protected static Vector3 GetAttachmentPoint(GameObject entity, AttachmentPoint attachmentPointType)
        {
            return entity.GetComponent<ActorComponent>().Model.GetAttachmentPoint(attachmentPointType).position;
        }

        private void AssignEffects(Missile missile)
        {
            var finalEffect = this.effectRepository.Find(this.data.FinalEffectId);

            if (finalEffect != null)
            {
                finalEffect.Skill = Skill;
                missile.FinalEffect = finalEffect;
            }

            var collideWithEntitiesEffect = this.effectRepository.Find(this.data.CollideWithEntitiesEffectId);

            if (collideWithEntitiesEffect != null)
            {
                collideWithEntitiesEffect.Skill = Skill;
                missile.CollideWithEntitiesEffect = collideWithEntitiesEffect;
            }

            var collideWithEnvironmentEffect = this.effectRepository.Find(this.data.CollideWithEnvironmentEffectId);

            if (collideWithEnvironmentEffect != null)
            {
                collideWithEnvironmentEffect.Skill = Skill;
                missile.CollideWithEnvironmentEffect = collideWithEnvironmentEffect;
            }

            var enterCellEffect = this.effectRepository.Find(this.data.EnterCellEffectId);

            if (enterCellEffect != null)
            {
                enterCellEffect.Skill = Skill;
                missile.EnterCellEffect = enterCellEffect;
            }

            var exitCellEffect = this.effectRepository.Find(this.data.ExitCellEffectId);

            if (exitCellEffect != null)
            {
                exitCellEffect.Skill = Skill;
                missile.ExitCellEffect = exitCellEffect;
            }
        }
    }
}