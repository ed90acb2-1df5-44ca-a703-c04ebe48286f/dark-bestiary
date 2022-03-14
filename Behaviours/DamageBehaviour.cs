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
        public ModifierType ModifierType => this.data.ModifierType;

        private readonly DamageBehaviourData data;

        protected DamageBehaviour(DamageBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        public float GetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            if (this.data.DamageTypes.Count > 0 && !this.data.DamageTypes.Contains(damage.Type))
            {
                return 0;
            }

            return OnGetDamageMultiplier(victim, attacker, ref damage);
        }

        protected abstract float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage);
    }
}