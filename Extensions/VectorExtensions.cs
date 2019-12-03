using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class VectorExtensions
    {
        public static Quaternion With(this Quaternion original, float? x = null, float? y = null, float? z = null, float? w = null)
        {
            return new Quaternion(x ?? original.x, y ?? original.y, z ?? original.z, w ?? original.w);
        }

        public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
        }

        public static Vector3 DirectionTo(this Vector3 source, Vector3 destination)
        {
            return Vector3.Normalize(destination - source);
        }

        public static Vector3 WithOffsetTowardsPoint(this Vector3 source, Vector3 target, Vector3 offset)
        {
            var direction = (target - source).normalized;
            return source + new Vector3(offset.x * direction.x, offset.y * direction.y, offset.z * direction.z);
        }

        public static Vector3 WithOffsetTowardsDirection(this Vector3 source, Vector3 direction, Vector3 offset)
        {
            return source + new Vector3(offset.x * direction.x, offset.y * direction.y, offset.z * direction.z);
        }

        public static Vector3 Snapped(this Vector3 vector, float size)
        {
            var xCount = Mathf.RoundToInt(vector.x / size);
            var yCount = Mathf.RoundToInt(vector.y / size);

            return new Vector3(xCount * size, yCount * size, 0);
        }

        public static Vector3 Snapped(this Vector3 vector)
        {
            return vector.Snapped(Board.Instance.CellSize);
        }
    }
}