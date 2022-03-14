using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Skills;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class MulticastBehaviour : Behaviour
    {
        private readonly MulticastBehaviourData data;

        public MulticastBehaviour(MulticastBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Skill.AnySkillUsed += OnSkillUsed;
        }

        protected override void OnRemove(GameObject source, GameObject target)
        {
            Skill.AnySkillUsed -= OnSkillUsed;
        }

        private void OnSkillUsed(SkillUseEventData data)
        {
            if (data.Caster != Target || !RNG.Test(this.data.Chance) ||
                !data.Skill.Flags.HasFlag(SkillFlags.Magic) ||
                data.Skill.Flags.HasFlag(SkillFlags.DisableMulticast))
            {
                return;
            }

            data.Skill.FaceTargetAndPlayAnimation(data.Target, () =>
            {
                var effect = data.Skill.Effect.Clone();
                effect.Skill = data.Skill;
                effect.Apply(Caster, data.Target);
            });
        }
    }
}