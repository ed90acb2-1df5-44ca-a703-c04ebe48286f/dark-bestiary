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
        private readonly PerRangeDamageBehaviourData data;

        public PerRangeDamageBehaviour(PerRangeDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            if (this.data.DamageFlags != DamageFlags.None && (damage.Flags & this.data.DamageFlags) <= 0 ||
                this.data.DamageInfoFlags != DamageInfoFlags.None && (damage.InfoFlags & this.data.DamageInfoFlags) <= 0)
            {
                return 0;
            }

            var distance = BoardNavigator.Instance.DistanceInCells(
                attacker.transform.position,
                victim.transform.position
            );

            return Mathf.Clamp(
                distance * (this.data.AmountPerCell * StackCount),
                this.data.Min,
                this.data.Max
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