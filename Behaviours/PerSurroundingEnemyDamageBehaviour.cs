using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Modifiers;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerSurroundingEnemyDamageBehaviour : DamageBehaviour
    {
        private readonly PerSurroundingEnemyDamageBehaviourData data;

        public PerSurroundingEnemyDamageBehaviour(PerSurroundingEnemyDamageBehaviourData data,
            List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Damage OnModify(GameObject victim, GameObject attacker, Damage damage)
        {
            var enemyCount = BoardNavigator.Instance
                .EntitiesInRadius(victim.transform.position, this.data.Range)
                .Count(entity => entity.IsEnemyOf(victim));

            if (enemyCount < this.data.MinimumNumberOfEnemies)
            {
                return damage;
            }

            return new Damage(new FloatModifier(
                Mathf.Clamp(enemyCount * this.data.AmountPerEnemy, this.data.Min, this.data.Max),
                ModifierType).Modify(damage.Amount), damage.Type, damage.WeaponSound, damage.Flags, damage.InfoFlags);
        }
    }
}