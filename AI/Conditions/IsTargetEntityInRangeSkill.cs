using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityInRangeSkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;
        private readonly int skillId;

        public IsTargetEntityInRangeSkill(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            this.boardNavigator = boardNavigator;
            this.skillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var entity = context.Entity;
            var target = context.RequireTargetEntity();

            var entitiesInSkillRange = this.boardNavigator
                .EntitiesInSkillRange(
                    entity.transform.position,
                    target.transform.position,
                    entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId)
                );

            return entitiesInSkillRange.Contains(target) ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}