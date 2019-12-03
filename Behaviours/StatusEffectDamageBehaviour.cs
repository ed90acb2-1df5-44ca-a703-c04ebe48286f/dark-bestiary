using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class StatusEffectDamageBehaviour : DamageBehaviour
    {
        private readonly StatusEffectDamageBehaviourData data;

        public StatusEffectDamageBehaviour(StatusEffectDamageBehaviourData data,
            List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
        {
            var statusFlags = victim.GetComponent<BehavioursComponent>().Behaviours.Aggregate(StatusFlags.None, (current, b) => current | b.StatusFlags);

            if ((statusFlags & this.data.DamageStatusFlags) == 0)
            {
                return damage;
            }

            return new Damage(
                new FloatModifier(this.data.Amount, ModifierType).Modify(damage.Amount),
                damage.Type,
                damage.WeaponSound,
                damage.Flags,
                damage.InfoFlags
            );
        }
    }
}