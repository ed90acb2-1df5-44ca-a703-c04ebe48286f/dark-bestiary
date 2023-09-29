using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointAroundTargetEntitySkill : BehaviourTreeLogicNode
    {
        private readonly int m_SkillId;
        private readonly BoardNavigator m_BoardNavigator;

        public SetTargetPointAroundTargetEntitySkill(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            m_SkillId = properties.SkillId;
            m_BoardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var skill = context.Entity.GetComponent<SpellbookComponent>().Get(m_SkillId);

            var inRange = m_BoardNavigator.WithinSkillRange(
                context.Entity.transform.position,
                context.RequireTargetEntity().transform.position, skill);

            var targetCell = m_BoardNavigator
                .WithinCircle(context.TargetEntity.transform.position, skill.Aoe)
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