using DarkBestiary.Data;
using DarkBestiary.GameBoard;
using DarkBestiary.Pathfinding;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointSuitableForSkillUseTargetEntity : SetTargetPointSuitableForSkillUse
    {
        public SetTargetPointSuitableForSkillUseTargetEntity(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator, IPathfinder pathfinder) : base(properties, boardNavigator, pathfinder)
        {
        }

        protected override bool IsContextValid(BehaviourTreeContext context)
        {
            return context.TargetEntity != null;
        }

        protected override Vector3 GetTargetPoint(BehaviourTreeContext context)
        {
            return context.TargetEntity.transform.position;
        }
    }
}