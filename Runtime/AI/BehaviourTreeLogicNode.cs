using DarkBestiary.Data;

namespace DarkBestiary.AI
{
    public abstract class BehaviourTreeLogicNode : BehaviourTreeNode
    {
        protected BehaviourTreePropertiesData Properties { get; }

        protected BehaviourTreeLogicNode(BehaviourTreePropertiesData properties)
        {
            Properties = properties;
        }
    }
}