namespace DarkBestiary.AI
{
    public abstract class BehaviourTreeNode : IBehaviourTreeNode
    {
        public BehaviourTreeStatus LastStatus { get; private set; }

        protected abstract BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta);

        protected virtual void OnOpen(BehaviourTreeContext context)
        {
        }

        protected virtual void OnClose(BehaviourTreeContext context)
        {
        }

        public BehaviourTreeStatus Tick(BehaviourTreeContext context, float delta)
        {
            if (!context.OpenedNodes.Contains(this))
            {
                Open(context);
            }

            var status = OnTick(context, delta);

            if (status != BehaviourTreeStatus.Running)
            {
                Close(context);
            }

            LastStatus = status;

            return status;
        }

        private void Open(BehaviourTreeContext context)
        {
            context.OpenedNodes.Add(this);
            OnOpen(context);
        }

        private void Close(BehaviourTreeContext context)
        {
            context.OpenedNodes.Remove(this);
            OnClose(context);

            LastStatus = BehaviourTreeStatus.Waiting;
        }
    }
}