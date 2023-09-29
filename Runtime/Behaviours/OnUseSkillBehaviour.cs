using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnUseSkillBehaviour : Behaviour
    {
        private readonly OnUseSkillBehaviourData m_Data;
        private readonly Effect m_Effect;

        public OnUseSkillBehaviour(OnUseSkillBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Skill.AnySkillUsed += OnSkillUsed;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Skill.AnySkillUsed -= OnSkillUsed;
        }

        protected void OnSkillUsed(SkillUseEventData data)
        {
            if (data.Caster != Target)
            {
                return;
            }

            if (m_Data.SkillRarityId > 0 && m_Data.SkillRarityId != data.Skill.Rarity?.Id)
            {
                return;
            }

            if (m_Data.SkillFlags != SkillFlags.None && (data.Skill.Flags & m_Data.SkillFlags) == 0)
            {
                return;
            }

            if (!Validators.ByPurpose(ValidatorPurpose.Other).Validate(data.Caster, data.Target))
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Target);
        }
    }
}