using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityNearestEnemySkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;
        private readonly int skillId;

        public SetTargetEntityNearestEnemySkill(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            this.boardNavigator = boardNavigator;
            this.skillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId);

            var entitiesTooClose = this.boardNavigator.EntitiesInRadius(
                context.Entity.transform.position, skill.RangeMin
            );

            var availableTargets = this.boardNavigator
                .EntitiesInRadius(context.Entity.transform.position, skill.GetMaxRange())
                .Where(enemy => enemy.IsEnemyOf(context.Entity))
                .Where(enemy => !entitiesTooClose.Contains(enemy))
                .ToList();

            if (availableTargets.Count == 0)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetEntity = availableTargets.First();

            return BehaviourTreeStatus.Success;
        }
    }
}