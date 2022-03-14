using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ChangeOwnerEffect : Effect
    {
        private readonly ChangeOwnerEffectData data;

        public ChangeOwnerEffect(ChangeOwnerEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new ChangeOwnerEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var unit = target.GetComponent<UnitComponent>();

            if (this.data.IsNeutral)
            {
                unit.ChangeOwner(Owner.Neutral, caster.GetComponent<UnitComponent>().TeamId);
            }
            else
            {
                unit.ChangeOwner(caster.GetComponent<UnitComponent>());
            }

            TriggerFinished();
        }
    }
}