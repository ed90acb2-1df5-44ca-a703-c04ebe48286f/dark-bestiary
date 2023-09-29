using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerMissingHealthPercentDamageBehaviour : DamageBehaviour
    {
        private readonly DamageSubject m_Target;
        private readonly float m_AmountPerPercent;
        private readonly float m_Min;
        private readonly float m_Max;

        public PerMissingHealthPercentDamageBehaviour(PerMissingHealthPercentDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Target = data.Target;
            m_AmountPerPercent = data.AmountPerPercent;
            m_Min = data.Min;
            m_Max = data.Max;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            HealthComponent health;

            switch (m_Target)
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

            return Mathf.Clamp(
                (1 - health.HealthFraction) * 100 * (m_AmountPerPercent * StackCount),
                m_Min,
                m_Max
            );
        }
    }
}