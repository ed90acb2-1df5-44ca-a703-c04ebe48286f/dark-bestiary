namespace DarkBestiary.AI
{
    public interface IBehaviourTreeNode
    {
        BehaviourTreeStatus LastStatus { get; }

        BehaviourTreeStatus Tick(BehaviourTreeContext context, float delta);
    }
}
