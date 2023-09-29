using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public abstract class DamageBehaviour : Behaviour
    {
        public ModifierType ModifierType => m_Data.ModifierType;

        private readonly DamageBehaviourData m_Data;

        protected DamageBehaviour(DamageBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        public float GetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            if (m_Data.DamageTypes.Count > 0 && !m_Data.DamageTypes.Contains(damage.Type))
            {
                return 0;
            }

            return OnGetDamageMultiplier(victim, attacker, ref damage);
        }

        protected abstract float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage);
    }
}