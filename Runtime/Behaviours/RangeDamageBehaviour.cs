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
        private readonly IPathfinder m_Pathfinder;
        private readonly ComparatorMethod m_Comparator;
        private readonly float m_Range;
        private readonly float m_Amount;

        public RangeDamageBehaviour(RangeDamageBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Pathfinder = Container.Instance.Resolve<IPathfinder>();
            m_Comparator = data.Comparator;
            m_Range = data.Range;
            m_Amount = data.Amount;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            var distance = BoardNavigator.Instance.DistanceInCells(
                attacker.transform.position,
                victim.transform.position
            );

            if (!Comparator.Compare(distance, m_Range, m_Comparator))
            {
                return 0;
            }

            return m_Amount * StackCount;
        }
    }
}