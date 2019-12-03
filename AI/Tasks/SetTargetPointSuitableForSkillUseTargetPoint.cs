using DarkBestiary.Data;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointSuitableForSkillUseTargetPoint : SetTargetPointSuitableForSkillUse
    {
        public SetTargetPointSuitableForSkillUseTargetPoint(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator, IPathfinder pathfinder) : base(properties, boardNavigator, pathfinder)
        {
        }

        protected override bool IsContextValid(BehaviourTreeContext context)
        {
            return context.TargetPoint.HasValue;
        }

        protected override Vector3 GetTargetPoint(BehaviourTreeContext context)
        {
            return context.TargetPoint ?? Vector3.zero;
        }
    }
}