using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
    public abstract class SetTargetPointSuitableForSkillUse : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;
        private readonly IPathfinder pathfinder;
        private readonly int skillId;

        protected SetTargetPointSuitableForSkillUse(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator, IPathfinder pathfinder) : base(properties)
        {
            this.boardNavigator = boardNavigator;
            this.pathfinder = pathfinder;
            this.skillId = properties.SkillId;
        }

        protected abstract Vector3 GetTargetPoint(BehaviourTreeContext context);
        protected abstract bool IsContextValid(BehaviourTreeContext context);

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (!IsContextValid(context))
            {
                return BehaviourTreeStatus.Failure;
            }

            var cellsSuitableForSkillUse = this.boardNavigator
                .WithinSkillRange(
                    GetTargetPoint(context),
                    context.Entity.transform.position,
                    context.Entity.GetComponent<SpellbookComponent>().FindOnActionBar(this.skillId))
                .Where(cell => cell.IsWalkable && !cell.IsOccupied)
                .Where(cell => this.pathfinder.IsTargetReachable(context.Entity, cell.transform.position))
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