using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RemoveShieldsEffect : Effect
    {
        private readonly EmptyEffectData m_Data;

        public RemoveShieldsEffect(EmptyEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new RemoveShieldsEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            foreach (var behaviour in target.GetComponent<BehavioursComponent>().Behaviours.ToList())
            {
                if (behaviour is ShieldBehaviour shield)
                {
                    shield.ChangeAmount(0);
                }

                TriggerFinished();
            }
        }
    }
}