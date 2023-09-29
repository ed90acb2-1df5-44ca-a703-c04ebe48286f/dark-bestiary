using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ChangeOwnerEffect : Effect
    {
        private readonly ChangeOwnerEffectData m_Data;

        public ChangeOwnerEffect(ChangeOwnerEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new ChangeOwnerEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var unit = target.GetComponent<UnitComponent>();

            if (m_Data.IsNeutral)
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