using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;
using DarkBestiary.Pathfinding;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class RangeDamageBehaviour : DamageBehaviour
    {
        private readonly IPathfinder pathfinder;
        private readonly ComparatorMethod comparator;
        private readonly float range;
        private readonly float amount;

        public RangeDamageBehaviour(RangeDamageBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.pathfinder = Container.Instance.Resolve<IPathfinder>();
            this.comparator = data.Comparator;
            this.range = data.Range;
            this.amount = data.Amount;
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
        {
            if (!Comparator.Compare(this.pathfinder.FindPath(victim, attacker.transform.position, false).Count, this.range,
                this.comparator))
            {
                return damage;
            }

            return new Damage(new FloatModifier(this.amount, ModifierType).Modify(damage.Amount),
                damage.Type, damage.WeaponSound, damage.Flags, damage.InfoFlags);
        }
    }
}