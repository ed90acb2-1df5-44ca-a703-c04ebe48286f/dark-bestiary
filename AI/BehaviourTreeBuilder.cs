using System;
using System.Collections.Generic;

namespace DarkBestiary.AI
{
    public class BehaviourTreeBuilder
    {
        private readonly Stack<IBehaviourTreeNodeParent> parentStack = new Stack<IBehaviourTreeNodeParent>();

        private IBehaviourTreeNodeParent top;

        public BehaviourTreeBuilder AddParent(IBehaviourTreeNodeParent parent)
        {
            if (this.parentStack.Count > 0)
            {
                this.parentStack.Peek().AddChild(parent);
            }

            this.parentStack.Push(parent);

            return this;
        }

        public BehaviourTreeBuilder AddChild(IBehaviourTreeNode child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (this.parentStack.Count <= 0)
            {
                throw new ApplicationException("There is no parent node in tree.");
            }

            this.parentStack.Peek().AddChild(child);

            return this;
        }

        public BehaviourTreeBuilder End()
        {
            this.top = this.parentStack.Pop();

            return this;
        }

        public BehaviourTree Build()
        {
            if (this.top == null)
            {
                throw new ApplicationException("Can't create a behaviour tree with zero nodes");
            }

            return new BehaviourTree(this.top);
        }
    }
}