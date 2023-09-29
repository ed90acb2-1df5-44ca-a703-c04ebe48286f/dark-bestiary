using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityNearestEnemySkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;
        private readonly int m_SkillId;

        public SetTargetEntityNearestEnemySkill(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
            m_SkillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().Get(m_SkillId);

            var entitiesTooClose = m_BoardNavigator.EntitiesInRadius(
                context.Entity.transform.position, skill.RangeMin
            );

            var availableTargets = m_BoardNavigator
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