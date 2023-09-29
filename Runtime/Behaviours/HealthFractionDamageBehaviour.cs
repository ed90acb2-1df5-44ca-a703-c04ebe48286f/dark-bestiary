using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class HealthFractionDamageBehaviour : DamageBehaviour
    {
        private readonly HealthFractionDamageBehaviourData m_Data;

        public HealthFractionDamageBehaviour(HealthFractionDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            HealthComponent health;

            switch (m_Data.Target)
            {
                case DamageSubject.Victim:
                    health = victim.GetComponent<HealthComponent>();
                    break;
                case DamageSubject.Attacker:
                    health = attacker.GetComponent<HealthComponent>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!Comparator.Compare(health.HealthFraction, m_Data.RequiredHealthFraction, m_Data.Comparator))
            {
                return 0;
            }

            return m_Data.Amount;
        }
    }
}