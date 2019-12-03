using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointAroundTargetEntitySkill : BehaviourTreeLogicNode
    {
        private readonly int skillId;
        private readonly BoardNavigator boardNavigator;

        public SetTargetPointAroundTargetEntitySkill(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            this.skillId = properties.SkillId;
            this.boardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId);

            var inRange = this.boardNavigator.WithinSkillRange(
                context.Entity.transform.position,
                context.RequireTargetEntity().transform.position, skill);

            var targetCell = this.boardNavigator
                .WithinCircle(context.TargetEntity.transform.position, skill.AOE)
                .Where(cell => cell.IsWalkable && !cell.IsOccupied && inRange.Contains(cell))
                .OrderBy(cell => (cell.transform.position - context.RequireTargetEntity().transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (targetCell == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetPoint = targetCell.transform.position;
            return BehaviourTreeStatus.Success;
        }
    }
}