using System.Collections.Generic;

namespace DarkBestiary.AI.Composites
{
    public class Race : BehaviourTreeNode, IBehaviourTreeNodeParent
    {
        public List<IBehaviourTreeNode> Children { get; } = new();

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var numChildrenFailed = 0;

            foreach (var child in Children)
            {
                var childStatus = child.Tick(context, delta);

                if (childStatus == BehaviourTreeStatus.Success)
                {
                    return childStatus;
                }

                if (childStatus == BehaviourTreeStatus.Failure)
                {
                    numChildrenFailed++;
                }
            }

            return numChildrenFailed == Children.Count ? BehaviourTreeStatus.Failure : BehaviourTreeStatus.Running;
        }

        public void AddChild(IBehaviourTreeNode child)
        {
            Children.Add(child);
        }
    }
}