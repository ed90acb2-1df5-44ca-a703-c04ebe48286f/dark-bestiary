using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;
using DarkBestiary.Modifiers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerRangeDamageBehaviour : DamageBehaviour
    {
        private readonly PerRangeDamageBehaviourData m_Data;

        public PerRangeDamageBehaviour(PerRangeDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            if (m_Data.DamageFlags != DamageFlags.None && (damage.Flags & m_Data.DamageFlags) <= 0 ||
                m_Data.DamageInfoFlags != DamageInfoFlags.None && (damage.InfoFlags & m_Data.DamageInfoFlags) <= 0)
            {
                return 0;
            }

            var distance = BoardNavigator.Instance.DistanceInCells(
                attacker.transform.position,
                victim.transform.position
            );

            return Mathf.Clamp(
                distance * (m_Data.AmountPerCell * StackCount),
                m_Data.Min,
                m_Data.Max
            );
        }

        public string GetDamageString(GameObject entity)
        {
            var amount = m_Data.AmountPerCell * StackCount;

            if (m_Data.ModifierType == ModifierType.Flat)
            {
                return ((int) amount).ToString();
            }

            return (amount * 100f).ToString("0.00") + "%";
        }
    }
}