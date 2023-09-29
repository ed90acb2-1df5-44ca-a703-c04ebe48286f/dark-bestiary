using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointInRangeSkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;
        private readonly int m_SkillId;

        public IsTargetPointInRangeSkill(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
            m_SkillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var entity = context.Entity;
            var target = context.RequireTargetPoint();

            var cellsInSkillRange = m_BoardNavigator
                .WithinSkillRange(
                    entity.transform.position, target,
                    entity.GetComponent<SpellbookComponent>().Get(m_SkillId));

            return cellsInSkillRange.Any(cell => cell.transform.position == target)
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}