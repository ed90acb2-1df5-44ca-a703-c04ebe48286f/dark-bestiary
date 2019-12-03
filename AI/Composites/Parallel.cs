using System.Collections.Generic;

namespace DarkBestiary.AI.Composites
{
    public class Parallel : BehaviourTreeNode, IBehaviourTreeNodeParent
    {
        public List<IBehaviourTreeNode> Children { get; } = new List<IBehaviourTreeNode>();

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var childrenSuceeded = 0;

            foreach (var child in Children)
            {
                var childStatus = child.Tick(context, delta);

                if (childStatus == BehaviourTreeStatus.Failure)
                {
                    return BehaviourTreeStatus.Failure;
                }

                if (childStatus == BehaviourTreeStatus.Success)
                {
                    childrenSuceeded++;
                }
            }

            return childrenSuceeded == Children.Count ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Running;
        }

        public void AddChild(IBehaviourTreeNode child)
        {
            Children.Add(child);
        }
    }
}