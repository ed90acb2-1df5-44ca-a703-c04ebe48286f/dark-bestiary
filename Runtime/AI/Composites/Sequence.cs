using System.Collections.Generic;

namespace DarkBestiary.AI.Composites
{
    public class Sequence : BehaviourTreeNode, IBehaviourTreeNodeParent
    {
        public List<IBehaviourTreeNode> Children { get; } = new();

        public void AddChild(IBehaviourTreeNode child)
        {
            Children.Add(child);
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var runningChildIndex = context.RunningChildIndex.GetValueOrDefault(this, 0);

            for (var i = runningChildIndex; i <  Children.Count; i++)
            {
                var childStatus = Children[i].Tick(context, delta);

                if (childStatus != BehaviourTreeStatus.Success)
                {
                    if (childStatus == BehaviourTreeStatus.Running)
                    {
                        context.RunningChildIndex[this] = i;
                    }

                    return childStatus;
                }
            }

            return BehaviourTreeStatus.Success;
        }

        protected override void OnClose(BehaviourTreeContext context)
        {
            context.RunningChildIndex[this] = 0;
        }
    }
}