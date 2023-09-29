using UnityEngine;

namespace DarkBestiary.Values
{
    public struct Area
    {
        public Vector3 Point { get; }
        public float Radius { get; }

        public Area(Vector3 point, float radius)
        {
            Point = point;
            Radius = radius;
        }
    }
}