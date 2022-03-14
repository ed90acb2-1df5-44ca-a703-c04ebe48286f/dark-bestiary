using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class LaunchMissileFromTargetEffect : LaunchMissileEffect
    {
        private readonly LaunchMissileEffectData data;
        private readonly IMissileRepository missileRepository;

        public LaunchMissileFromTargetEffect(LaunchMissileEffectData data, List<ValidatorWithPurpose> validators,
            IMissileRepository missileRepository) : base(data, validators, missileRepository)
        {
            this.data = data;
            this.missileRepository = missileRepository;
        }

        protected override Effect New()
        {
            return new LaunchMissileFromTargetEffect(this.data, this.Validators, this.missileRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Launch(
                caster,
                caster,
                GetAttachmentPoint(target, this.data.TargetAttachmentPoint),
                GetAttachmentPoint(caster, this.data.CasterAttachmentPoint)
            );
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            Launch(
                caster,
                caster,
                target,
                GetAttachmentPoint(caster, this.data.CasterAttachmentPoint)
            );
        }
    }
}