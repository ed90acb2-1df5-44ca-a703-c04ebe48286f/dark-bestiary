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
        private readonly OnUseSkillBehaviourData data;
        private readonly Effect effect;

        public OnUseSkillBehaviour(OnUseSkillBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
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

            if (this.data.SkillRarityId > 0 && this.data.SkillRarityId != data.Skill.Rarity?.Id)
            {
                return;
            }

            if (this.data.SkillFlags != SkillFlags.None && (data.Skill.Flags & this.data.SkillFlags) == 0)
            {
                return;
            }

            if (!this.Validators.ByPurpose(ValidatorPurpose.Other).Validate(data.Caster, data.Target))
            {
                return;
            }

            var clone = this.effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Target);
        }
    }
}