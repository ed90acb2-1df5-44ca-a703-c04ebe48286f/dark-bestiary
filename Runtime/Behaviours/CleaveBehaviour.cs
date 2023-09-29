using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class CleaveBehaviour : Behaviour
    {
        private static bool s_IsCleaving;

        private readonly CleaveBehaviourData m_Data;
        private readonly BoardNavigator m_BoardNavigator;

        public CleaveBehaviour(CleaveBehaviourData data, BoardNavigator boardNavigator, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_BoardNavigator = boardNavigator;
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
            if (data.Source != Target || !data.Damage.Flags.HasFlag(DamageFlags.Melee) || data.Damage.Amount < 1)
            {
                return;
            }

            if (s_IsCleaving)
            {
                return;
            }

            s_IsCleaving = true;

            var cleaveDamage = data.Damage * m_Data.Fraction;
            cleaveDamage.Flags |= DamageFlags.CantBeBlocked;
            cleaveDamage.Flags |= DamageFlags.CantBeDodged;
            cleaveDamage.InfoFlags |= DamageInfoFlags.Cleave;

            if (!string.IsNullOrEmpty(m_Data.Prefab))
            {
                Object.Instantiate(Resources.Load<GameObject>(m_Data.Prefab), Target.transform.position, Quaternion.identity).DestroyAsVisualEffect();
            }

            foreach (var cell in m_BoardNavigator.WithinCircle(Target.transform.position, 2))
            {
                if (!cell.IsOccupied)
                {
                    continue;
                }

                if (cell.OccupiedBy.IsDummy() || cell.OccupiedBy == data.Target || cell.OccupiedBy.IsAllyOf(Target))
                {
                    continue;
                }

                cell.OccupiedBy.GetComponent<HealthComponent>().Damage(Target, cleaveDamage);
            }

            s_IsCleaving = false;
        }
    }
}