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
    public class PerMissingHealthPercentDamageBehaviour : DamageBehaviour
    {
        private readonly DamageSubject target;
        private readonly float amountPerPercent;
        private readonly float min;
        private readonly float max;

        public PerMissingHealthPercentDamageBehaviour(PerMissingHealthPercentDamageBehaviourData data,
            List<Validator> validators) : base(data, validators)
        {
            this.target = data.Target;
            this.amountPerPercent = data.AmountPerPercent;
            this.min = data.Min;
            this.max = data.Max;
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
        {
            HealthComponent health;

            switch (this.target)
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

            var modifier = new FloatModifier(
                Mathf.Clamp(
                    (1 - health.HealthFraction) * 100 * this.amountPerPercent,
                    this.min,
                    this.max
                ),
                ModifierType);

            var modified = modifier.Modify(damage.Amount);

            return new Damage(modified, damage.Type, damage.WeaponSound, damage.Flags, damage.InfoFlags);
        }
    }
}