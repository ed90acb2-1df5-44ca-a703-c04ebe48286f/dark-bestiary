using System.Collections.Generic;

namespace DarkBestiary.AI
{
    public interface IBehaviourTreeNodeParent : IBehaviourTreeNode
    {
        List<IBehaviourTreeNode> Children { get; }

        void AddChild(IBehaviourTreeNode child);
    }
}
