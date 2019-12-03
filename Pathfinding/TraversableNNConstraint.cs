using Pathfinding;

namespace DarkBestiary.Pathfinding
{
    public class TraversableNNConstraint : NNConstraint
    {
        private readonly Path path;
        private readonly ITraversalProvider traversalProvider;

        public TraversableNNConstraint(Path path, ITraversalProvider traversalProvider)
        {
            this.path = path;
            this.traversalProvider = traversalProvider;
        }

        public override bool Suitable(GraphNode node)
        {
            return this.traversalProvider.CanTraverse(this.path, node) && base.Suitable(node);
        }
    }
}