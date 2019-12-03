using System.Collections.Generic;
using Pathfinding;

namespace DarkBestiary.Pathfinding
{
    public class BlockManagerTraversalProvider : ITraversalProvider
    {
        private readonly BlockManager blockManager;
        private readonly List<SingleNodeBlocker> selector;

        public BlockManagerTraversalProvider(BlockManager blockManager, List<SingleNodeBlocker> selector)
        {
            if (blockManager == null)
            {
                throw new System.ArgumentNullException(nameof(blockManager));
            }

            this.blockManager = blockManager;
            this.selector = selector ?? throw new System.ArgumentNullException(nameof(selector));
        }

        public virtual bool CanTraverse(Path path, GraphNode node)
        {
            if (!DefaultITraversalProvider.CanTraverse(path, node))
            {
                return false;
            }

            return !this.blockManager.NodeContainsAnyExcept(node, this.selector);
        }

        public virtual uint GetTraversalCost(Path path, GraphNode node)
        {
            return DefaultITraversalProvider.GetTraversalCost(path, node);
        }
    }
}