using System.Collections.Generic;

namespace DarkBestiary.AI.Composites
{
    public class Tree : BehaviourTreeNode, IBehaviourTreeNodeParent
    {
        public List<IBehaviourTreeNode> Children { get; } = new List<IBehaviourTreeNode>();

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            foreach (var child in Children)
            {
                child.Tick(context, delta);
            }

            return BehaviourTreeStatus.Success;
        }

        public void AddChild(IBehaviourTreeNode child)
        {
            Children.Add(child);
        }
    }
}