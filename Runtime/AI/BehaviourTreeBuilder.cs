using System;
using System.Collections.Generic;

namespace DarkBestiary.AI
{
    public class BehaviourTreeBuilder
    {
        private readonly Stack<IBehaviourTreeNodeParent> m_ParentStack = new();

        private IBehaviourTreeNodeParent m_Top;

        public BehaviourTreeBuilder AddParent(IBehaviourTreeNodeParent parent)
        {
            if (m_ParentStack.Count > 0)
            {
                m_ParentStack.Peek().AddChild(parent);
            }

            m_ParentStack.Push(parent);

            return this;
        }

        public BehaviourTreeBuilder AddChild(IBehaviourTreeNode child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (m_ParentStack.Count <= 0)
            {
                throw new ApplicationException("There is no parent node in tree.");
            }

            m_ParentStack.Peek().AddChild(child);

            return this;
        }

        public BehaviourTreeBuilder End()
        {
            m_Top = m_ParentStack.Pop();

            return this;
        }

        public BehaviourTree Build()
        {
            if (m_Top == null)
            {
                throw new ApplicationException("Can't create a behaviour tree with zero nodes");
            }

            return new BehaviourTree(m_Top);
        }
    }
}