using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerSurroundingEnemyDamageBehaviour : DamageBehaviour
    {
        private readonly PerSurroundingEnemyDamageBehaviourData data;

        public PerSurroundingEnemyDamageBehaviour(PerSurroundingEnemyDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override float OnGetDamageMultiplier(GameObject victim, GameObject attacker, ref Damage damage)
        {
            var enemyCount = BoardNavigator.Instance
                .EntitiesInRadius(attacker.transform.position, this.data.Range)
                .Count(entity => entity.IsEnemyOf(attacker));

            if (enemyCount < this.data.MinimumNumberOfEnemies)
            {
                return 0;
            }

            return Mathf.Clamp(
                enemyCount * (this.data.AmountPerEnemy * StackCount),
                this.data.Min,
                this.data.Max
            );
        }
    }
}