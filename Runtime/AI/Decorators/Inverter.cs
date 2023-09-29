using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.AI.Decorators
{
    public class Inverter : BehaviourTreeNode, IBehaviourTreeNodeParent
    {
        public List<IBehaviourTreeNode> Children { get; } = new();

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (Children.Count == 0)
            {
                throw new ApplicationException($"{GetType().Name} must have a child node!");
            }

            var childStatus = Children.First().Tick(context, delta);

            if (childStatus == BehaviourTreeStatus.Failure)
            {
                return BehaviourTreeStatus.Success;
            }

            if (childStatus == BehaviourTreeStatus.Success)
            {
                return BehaviourTreeStatus.Failure;
            }

            return childStatus;
        }

        public virtual void Open()
        {
        }

        public void AddChild(IBehaviourTreeNode child)
        {
            if (Children.Count > 0)
            {
                throw new ApplicationException($"Can't add more than a single child to {GetType().Name}!");
            }

            Children.Add(child);
        }
    }
}
