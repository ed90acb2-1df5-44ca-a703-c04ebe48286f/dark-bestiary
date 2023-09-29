using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class LaunchMissileFromTargetEffect : LaunchMissileEffect
    {
        private readonly LaunchMissileEffectData m_Data;
        private readonly IMissileRepository m_MissileRepository;

        public LaunchMissileFromTargetEffect(LaunchMissileEffectData data, List<ValidatorWithPurpose> validators,
            IMissileRepository missileRepository) : base(data, validators, missileRepository)
        {
            m_Data = data;
            m_MissileRepository = missileRepository;
        }

        protected override Effect New()
        {
            return new LaunchMissileFromTargetEffect(m_Data, Validators, m_MissileRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Launch(
                caster,
                caster,
                GetAttachmentPoint(target, m_Data.TargetAttachmentPoint),
                GetAttachmentPoint(caster, m_Data.CasterAttachmentPoint)
            );
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            Launch(
                caster,
                caster,
                target,
                GetAttachmentPoint(caster, m_Data.CasterAttachmentPoint)
            );
        }
    }
}