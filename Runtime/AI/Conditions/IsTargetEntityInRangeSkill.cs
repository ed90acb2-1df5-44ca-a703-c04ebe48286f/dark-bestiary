using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityInRangeSkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;
        private readonly int m_SkillId;

        public IsTargetEntityInRangeSkill(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
            m_SkillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var entity = context.Entity;
            var target = context.RequireTargetEntity();
            var skill = entity.GetComponent<SpellbookComponent>().Get(m_SkillId);

            var entitiesInSkillRange = m_BoardNavigator
                .EntitiesInSkillRange(
                    entity.transform.position,
                    target.transform.position,
                    skill
                );

            return entitiesInSkillRange.Contains(target) ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}