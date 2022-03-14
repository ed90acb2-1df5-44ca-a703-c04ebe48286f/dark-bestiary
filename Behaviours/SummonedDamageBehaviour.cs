using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class SummonedDamageBehaviour : DamageBehaviour
    {
        private readonly SummonedDamageBehaviourData data;

        public SummonedDamageBehaviour(SummonedDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            if (!victim.IsSummoned())
            {
                return 0;
            }

            return this.data.Amount * StackCount;
        }
    }
}