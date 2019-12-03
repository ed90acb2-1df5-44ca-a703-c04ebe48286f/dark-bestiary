using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointSummonSkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;
        private readonly int skillId;

        public SetTargetPointSummonSkill(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            this.boardNavigator = boardNavigator;
            this.skillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cellsSuitableForSkillUse = this.boardNavigator
                .WithinSkillRange(
                    context.Entity.transform.position,
                    context.Entity.transform.position,
                    context.Entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId))
                .Where(cell => !cell.IsOccupied && cell.IsWalkable)
                .ToList();

            if (cellsSuitableForSkillUse.Count == 0)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetPoint = cellsSuitableForSkillUse
                .OrderBy(cell => (cell.transform.position - context.Entity.transform.position).sqrMagnitude)
                .First()
                .transform.position;

            return BehaviourTreeStatus.Success;
        }
    }
}