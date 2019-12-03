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

        protected DamageBehaviour(DamageBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        public Damage Modify(GameObject victim, GameObject attacker, Damage damage)
        {
            if (this.data.DamageTypes.Count > 0 && !this.data.DamageTypes.Contains(damage.Type))
            {
                return damage;
            }

            return OnModify(victim, attacker, damage);
        }

        protected abstract Damage OnModify(GameObject victim, GameObject attacker, Damage damage);
    }
}