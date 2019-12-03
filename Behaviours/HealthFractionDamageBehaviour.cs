using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class HealthFractionDamageBehaviour : DamageBehaviour
    {
        private readonly HealthFractionDamageBehaviourData data;

        public HealthFractionDamageBehaviour(HealthFractionDamageBehaviourData data,
            List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
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
                return damage;
            }

            var modified = new FloatModifier(this.data.Amount, ModifierType).Modify(damage.Amount);

            return new Damage(modified, damage.Type, damage.WeaponSound, damage.Flags, damage.InfoFlags);
        }
    }
}