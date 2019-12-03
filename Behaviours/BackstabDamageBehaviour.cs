using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class BackstabDamageBehaviour : DamageBehaviour
    {
        private readonly BackstabDamageBehaviourData data;

        public BackstabDamageBehaviour(BackstabDamageBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
        {
            if (victim.GetComponent<ActorComponent>().Model.IsFacingLeft !=
                attacker.GetComponent<ActorComponent>().Model.IsFacingLeft)
            {
                return damage;
            }

            var backstab = new Damage(new FloatModifier(this.data.Amount, ModifierType)
                .Modify(damage.Amount), damage.Type, damage.WeaponSound, damage.Flags, damage.InfoFlags);

            backstab.InfoFlags |= DamageInfoFlags.Backstab;
            return backstab;
        }
    }
}