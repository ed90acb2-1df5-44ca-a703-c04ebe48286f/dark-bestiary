using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;
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

        public RangeDamageBehaviour(RangeDamageBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.pathfinder = Container.Instance.Resolve<IPathfinder>();
            this.comparator = data.Comparator;
            this.range = data.Range;
            this.amount = data.Amount;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            var distance = BoardNavigator.Instance.DistanceInCells(
                attacker.transform.position,
                victim.transform.position
            );

            if (!Comparator.Compare(distance, this.range, this.comparator))
            {
                return 0;
            }

            return this.amount * StackCount;
        }
    }
}