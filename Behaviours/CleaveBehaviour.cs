using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class CleaveBehaviour : Behaviour
    {
        private static bool isCleaving;

        private readonly CleaveBehaviourData data;
        private readonly BoardNavigator boardNavigator;

        public CleaveBehaviour(CleaveBehaviourData data, BoardNavigator boardNavigator, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.boardNavigator = boardNavigator;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Attacker != Target || !data.Damage.Flags.HasFlag(DamageFlags.Melee) || data.Damage.Amount < 1)
            {
                return;
            }

            if (isCleaving)
            {
                return;
            }

            isCleaving = true;

            var cleaveDamage = data.Damage * this.data.Fraction;
            cleaveDamage.Flags |= DamageFlags.CantBeBlocked;
            cleaveDamage.Flags |= DamageFlags.CantBeDodged;
            cleaveDamage.InfoFlags |= DamageInfoFlags.Cleave;

            if (!string.IsNullOrEmpty(this.data.Prefab))
            {
                Object.Instantiate(Resources.Load<GameObject>(this.data.Prefab), Target.transform.position, Quaternion.identity).DestroyAsVisualEffect();
            }

            foreach (var cell in this.boardNavigator.WithinCircle(Target.transform.position, 2))
            {
                if (!cell.IsOccupied)
                {
                    continue;
                }

                if (cell.OccupiedBy.IsDummy() || cell.OccupiedBy == data.Victim || cell.OccupiedBy.IsAllyOf(Target))
                {
                    continue;
                }

                cell.OccupiedBy.GetComponent<HealthComponent>().Damage(Target, cleaveDamage);
            }

            isCleaving = false;
        }
    }
}