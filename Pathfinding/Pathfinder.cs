using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
using Pathfinding;
using UnityEngine;

namespace DarkBestiary.Pathfinding
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(AstarPath))]
    public class Pathfinder : Singleton<Pathfinder>, IPathfinder
    {
        public event Payload ScanCompleted;
        public event Payload GraphsUpdated;

        public BlockManager BlockManager { get; private set; }

        private AstarPath pathfinder;
        private Seeker seeker;

        private void Awake()
        {
            BlockManager = GetComponent<BlockManager>();

            this.seeker = GetComponent<Seeker>();
            this.pathfinder = GetComponent<AstarPath>();

            AstarPath.OnGraphsUpdated += OnGraphsUpdated;
        }

        private void OnGraphsUpdated(AstarPath script)
        {
            GraphsUpdated?.Invoke();
        }

        public void Scan()
        {
            this.pathfinder.Scan();
            ScanCompleted?.Invoke();
        }

        public int Distance(Vector3 origin, Vector3 destination)
        {
            var path = this.seeker.StartPath(origin, destination);
            path.nnConstraint = NNConstraint.None;
            path.BlockUntilCalculated();

            return path.path.Count < 2 || path.error ? 0 : path.path.Count;
        }

        public List<Vector3> FindPath(GameObject entity, Vector3 destination, bool calculatePartial)
        {
            var path = StartPath(entity, destination, calculatePartial);
            path.BlockUntilCalculated();

            return path.path.Count < 2 || path.error ? new List<Vector3>() : path.vectorPath.ToList();
        }

        public void FindPathAsync(GameObject entity, Vector3 destination, bool calculatePartial, Action<List<Vector3>> callback)
        {
            StartPath(entity, destination, calculatePartial, _ =>
            {
                // TODO: First path ignores SingleNodeBlocker stuff.
                StartPath(entity, destination, calculatePartial, path =>
                {
                    var points = path.error || path.path.Count < 2
                        ? new List<Vector3>() : path.vectorPath.ToList();

                    callback.Invoke(points);
                });
            });
        }

        public bool IsPointWalkable(Vector3 point)
        {
            return this.pathfinder.data.gridGraph.GetNearest(point, PathNNConstraint.Default).node?.Walkable ?? false;
        }

        public bool IsTargetReachable(GameObject entity, Vector3 destination)
        {
            return FindPath(entity, destination, false).Count > 0;
        }

        public Vector3? GetNearestReachable(GameObject entity, Vector3 destination)
        {
            var path = FindPath(entity, destination, true);

            if (path.Count == 0)
            {
                return null;
            }

            return path.Last();
        }

        private Path StartPath(GameObject entity, Vector3 destination, bool calculatePartial, OnPathDelegate callback = null)
        {
            var traversalProvider = CreateTraversalProvider(entity);

            var path = ABPath.Construct(entity.transform.position, destination, callback);
            path.calculatePartial = calculatePartial;
            path.traversalProvider = traversalProvider;
            path.nnConstraint = new TraversableNNConstraint(path, traversalProvider);

            AstarPath.StartPath(path);

            return path;
        }

        private ITraversalProvider CreateTraversalProvider(GameObject entity)
        {
            return new BlockManager.TraversalProvider(BlockManager, BlockManager.BlockMode.AllExceptSelector, new List<SingleNodeBlocker>
            {
                entity.GetComponent<SingleNodeBlocker>()
            });
        }
    }
}