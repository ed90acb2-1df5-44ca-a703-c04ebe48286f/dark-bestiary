using Pathfinding;

namespace DarkBestiary.Pathfinding
{
    public class TraversableNnConstraint : NNConstraint
    {
        private readonly Path m_Path;
        private readonly ITraversalProvider m_TraversalProvider;

        public TraversableNnConstraint(Path path, ITraversalProvider traversalProvider)
        {
            m_Path = path;
            m_TraversalProvider = traversalProvider;
        }

        public override bool Suitable(GraphNode node)
        {
            return m_TraversalProvider.CanTraverse(m_Path, node) && base.Suitable(node);
        }
    }
}