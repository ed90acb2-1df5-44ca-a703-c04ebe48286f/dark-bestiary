using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerRangeDamageBehaviour : DamageBehaviour
    {
        private readonly PerRangeDamageBehaviourData data;
        private readonly IPathfinder pathfinder;

        public PerRangeDamageBehaviour(PerRangeDamageBehaviourData data,
            List<Validator> validators) : base(data, validators)
        {
            this.data = data;
            this.pathfinder = Container.Instance.Resolve<IPathfinder>();
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
        {
            if (this.data.DamageFlags != DamageFlags.None && (damage.Flags & this.data.DamageFlags) <= 0 ||
                this.data.DamageInfoFlags != DamageInfoFlags.None && (damage.InfoFlags & this.data.DamageInfoFlags) <= 0)
            {
                return damage;
            }

            return new Damage(
                new FloatModifier(
                    Mathf.Clamp(
                        this.pathfinder.FindPath(victim, attacker.transform.position, false).Count * (this.data.AmountPerCell * StackCount),
                        this.data.Min,
                        this.data.Max
                    ),
                    ModifierType).Modify(damage.Amount),
                damage.Type,
                damage.WeaponSound,
                damage.Flags,
                damage.InfoFlags
            );
        }

        public string GetDamageString(GameObject entity)
        {
            var amount = this.data.AmountPerCell * StackCount;

            if (this.data.ModifierType == ModifierType.Flat)
            {
                return ((int) amount).ToString();
            }

            return (amount * 100f).ToString("0.00") + "%";
        }
    }
}