using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Skills;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnUsingSkillBehaviour : OnUseSkillBehaviour
    {
        public OnUsingSkillBehaviour(OnUseSkillBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Skill.AnySkillUsing += OnSkillUsed;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Skill.AnySkillUsing -= OnSkillUsed;
        }
    }
}