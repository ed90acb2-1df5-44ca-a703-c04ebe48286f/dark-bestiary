using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointSummonSkill : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;
        private readonly int m_SkillId;

        public SetTargetPointSummonSkill(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
            m_SkillId = properties.SkillId;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cellsSuitableForSkillUse = m_BoardNavigator
                .WithinSkillRange(
                    context.Entity.transform.position,
                    context.Entity.transform.position,
                    context.Entity.GetComponent<SpellbookComponent>().Get(m_SkillId))
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