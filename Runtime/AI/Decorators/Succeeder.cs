using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.AI.Decorators
{
    public class Succeeder : BehaviourTreeNode, IBehaviourTreeNodeParent
    {
        public List<IBehaviourTreeNode> Children { get; } = new();

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (Children.Count == 0)
            {
                throw new ApplicationException($"{GetType().Name} must have a child node!");
            }

            return Children.First().Tick(context, delta) == BehaviourTreeStatus.Running
                ? BehaviourTreeStatus.Running
                : BehaviourTreeStatus.Success;
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
