using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Skills;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class MulticastBehaviour : Behaviour
    {
        private readonly MulticastBehaviourData m_Data;

        public MulticastBehaviour(MulticastBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
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
            if (data.Caster != Target || !Rng.Test(m_Data.Chance) ||
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