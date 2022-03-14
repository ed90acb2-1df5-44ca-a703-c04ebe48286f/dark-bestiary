using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class UnlockSkillBehaviour : Behaviour
    {
        private readonly UnlockSkillBehaviourData data;

        public UnlockSkillBehaviour(UnlockSkillBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            var skill = Container.Instance.Resolve<ISkillRepository>().FindOrFail(this.data.SkillId);
            target.GetComponent<SpellbookComponent>().Add(skill);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<SpellbookComponent>().Remove(this.data.SkillId);
        }
    }
}