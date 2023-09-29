using System.Collections.Generic;
using Pathfinding;

namespace DarkBestiary.Pathfinding
{
    public class BlockManagerTraversalProvider : ITraversalProvider
    {
        private readonly BlockManager m_BlockManager;
        private readonly List<SingleNodeBlocker> m_Selector;

        public BlockManagerTraversalProvider(BlockManager blockManager, List<SingleNodeBlocker> selector)
        {
            if (blockManager == null)
            {
                throw new System.ArgumentNullException(nameof(blockManager));
            }

            m_BlockManager = blockManager;
            m_Selector = selector ?? throw new System.ArgumentNullException(nameof(selector));
        }

        public virtual bool CanTraverse(Path path, GraphNode node)
        {
            if (!DefaultITraversalProvider.CanTraverse(path, node))
            {
                return false;
            }

            return !m_BlockManager.NodeContainsAnyExcept(node, m_Selector);
        }

        public virtual uint GetTraversalCost(Path path, GraphNode node)
        {
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }
}