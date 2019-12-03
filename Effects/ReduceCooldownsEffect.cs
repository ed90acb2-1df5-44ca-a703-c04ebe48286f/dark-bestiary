using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ReduceCooldownsEffect : Effect
    {
        private readonly ReduceCooldownsEffectData data;

        public ReduceCooldownsEffect(ReduceCooldownsEffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new ReduceCooldownsEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            foreach (var slot in target.GetComponent<SpellbookComponent>().Slots)
            {
                slot.Skill.ReduceCooldown(this.data.Amount);;
            }

            TriggerFinished();
        }
    }
}