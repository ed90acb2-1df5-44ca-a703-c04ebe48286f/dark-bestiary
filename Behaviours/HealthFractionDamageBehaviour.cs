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
        private readonly HealthFractionDamageBehaviourData data;

        public HealthFractionDamageBehaviour(HealthFractionDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            HealthComponent health;

            switch (this.data.Target)
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

            if (!Comparator.Compare(health.HealthFraction, this.data.RequiredHealthFraction, this.data.Comparator))
            {
                return 0;
            }

            return this.data.Amount;
        }
    }
}