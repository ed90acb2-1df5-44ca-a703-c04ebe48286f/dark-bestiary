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
        private readonly SummonedDamageBehaviourData m_Data;

        public SummonedDamageBehaviour(SummonedDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            if (!victim.IsSummoned())
            {
                return 0;
            }

            return m_Data.Amount * StackCount;
        }
    }
}