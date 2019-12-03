using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkBestiary.Pathfinding
{
    public interface IPathfinder
    {
        Vector3? GetNearestReachable(GameObject entity, Vector3 destination);

        List<Vector3> FindPath(GameObject entity, Vector3 destination, bool calculatePartial);

        int Distance(Vector3 origin, Vector3 destination);

        void FindPathAsync(GameObject entity, Vector3 destination, bool calculatePartial, Action<List<Vector3>> callback);

        bool IsPointWalkable(Vector3 point);

        bool IsTargetReachable(GameObject entity, Vector3 destination);
    }
}