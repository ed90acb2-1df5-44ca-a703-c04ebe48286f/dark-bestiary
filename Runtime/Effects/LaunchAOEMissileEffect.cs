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
        private readonly LaunchAoeMissileEffectData m_Data;
        private readonly IMissileRepository m_MissileRepository;
        private readonly IEffectRepository m_EffectRepository;

        private readonly List<GameObject> m_Hits = new();

        public LaunchAOEMissileEffect(LaunchAoeMissileEffectData data, List<ValidatorWithPurpose> validators,
            IMissileRepository missileRepository, IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_MissileRepository = missileRepository;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new LaunchAOEMissileEffect(m_Data, Validators, m_MissileRepository, m_EffectRepository);
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
                GetAttachmentPoint(origin, m_Data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target.transform.position),
                GetAttachmentPoint(target, m_Data.TargetAttachmentPoint) + GetTargetOffset()
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
                GetAttachmentPoint(origin, m_Data.CasterAttachmentPoint) + GetCasterOffset(origin.transform.position, target),
                target + GetTargetOffset()
            );
        }

        private Vector3 GetCasterOffset(Vector3 origin, Vector3 target)
        {
            var direction = (target - origin).normalized;

            return new Vector3(m_Data.CasterOffsetX, m_Data.CasterOffsetY) +
                   new Vector3(m_Data.DirectionalOffsetX * direction.x, m_Data.DirectionalOffsetY * direction.y);
        }

        private Vector3 GetTargetOffset()
        {
            return new Vector3(m_Data.TargetOffsetX, m_Data.TargetOffsetY);
        }

        protected void Launch(GameObject caster, object target, Vector3 origin, Vector3 destination)
        {
            var missile = m_MissileRepository.FindOrFail(m_Data.MissileId);

            AssignEffects(missile);

            missile.Mover = Mover.Factory(m_Data.Mover);
            missile.Mover.Stopped += OnMoverStopped;
            missile.EnterCell += OnMissileEnterCell;
            missile.FlyHeight = m_Data.MissileFlyHeight;
            missile.gameObject.SetActive(false);
            missile.transform.position = origin;
            missile.Launch(caster, target, destination);
            missile.gameObject.SetActive(true);
        }

        private void OnMissileEnterCell(BoardCell cell)
        {
            var entities = BoardNavigator.Instance
                .WithinCircle(cell.transform.position, m_Data.Radius)
                .ToEntities()
                .Where(entity => !m_Hits.Contains(entity) && Validators.ByPurpose(ValidatorPurpose.Other).Validate(Caster, entity))
                .ToList();

            foreach (var entity in entities)
            {
                m_EffectRepository.Find(m_Data.EffectId).Apply(Caster, entity);
                m_Hits.Add(entity);
            }
        }

        private void OnMoverStopped()
        {
            TriggerFinished();
        }

        protected static Vector3 GetAttachmentPoint(GameObject entity, AttachmentPoint attachmentPointType)
        {
            return entity.GetComponent<ActorComponent>().Model.GetAttachmentPoint(attachmentPointType).position;
        }

        private void AssignEffects(Missile missile)
        {
            var finalEffect = m_EffectRepository.Find(m_Data.FinalEffectId);

            if (finalEffect != null)
            {
                finalEffect.Skill = Skill;
                finalEffect.DamageMultiplier = DamageMultiplier;
                missile.FinalEffect = finalEffect;
            }

            var collideWithEntitiesEffect = m_EffectRepository.Find(m_Data.CollideWithEntitiesEffectId);

            if (collideWithEntitiesEffect != null)
            {
                collideWithEntitiesEffect.Skill = Skill;
                collideWithEntitiesEffect.DamageMultiplier = DamageMultiplier;
                missile.CollideWithEntitiesEffect = collideWithEntitiesEffect;
            }

            var collideWithEnvironmentEffect = m_EffectRepository.Find(m_Data.CollideWithEnvironmentEffectId);

            if (collideWithEnvironmentEffect != null)
            {
                collideWithEnvironmentEffect.Skill = Skill;
                collideWithEnvironmentEffect.DamageMultiplier = DamageMultiplier;
                missile.CollideWithEnvironmentEffect = collideWithEnvironmentEffect;
            }

            var enterCellEffect = m_EffectRepository.Find(m_Data.EnterCellEffectId);

            if (enterCellEffect != null)
            {
                enterCellEffect.Skill = Skill;
                enterCellEffect.DamageMultiplier = DamageMultiplier;
                missile.EnterCellEffect = enterCellEffect;
            }

            var exitCellEffect = m_EffectRepository.Find(m_Data.ExitCellEffectId);

            if (exitCellEffect != null)
            {
                exitCellEffect.Skill = Skill;
                exitCellEffect.DamageMultiplier = DamageMultiplier;
                missile.ExitCellEffect = exitCellEffect;
            }
        }
    }
}