using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointInRangeSkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;
        private readonly int skillId;

        public IsTargetPointInRangeSkill(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            this.boardNavigator = boardNavigator;
            this.skillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var entity = context.Entity;
            var target = context.RequireTargetPoint();

            var cellsInSkillRange = this.boardNavigator
                .WithinSkillRange(
                    entity.transform.position, target,
                    entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId));

            return cellsInSkillRange.Any(cell => cell.transform.position == target)
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}