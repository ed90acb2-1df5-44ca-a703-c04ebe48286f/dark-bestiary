using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityOnLineOfSight : BehaviourTreeLogicNode
    {
        public IsTargetEntityOnLineOfSight(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var difference = context.RequireTargetEntity().transform.position - context.Entity.transform.position;

            var direction = difference.normalized;
            var magnitude = difference.magnitude;

            var onLineOfSight = Physics2D
                .RaycastAll(context.Entity.transform.position + direction, direction, magnitude - 2.0f)
                .ToCellsPrecise()
                .All(hitCell => hitCell.IsWalkable && !hitCell.IsOccupied);

            return onLineOfSight ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}