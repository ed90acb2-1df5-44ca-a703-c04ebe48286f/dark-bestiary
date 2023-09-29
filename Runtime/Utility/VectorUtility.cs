using UnityEngine;

namespace DarkBestiary.Utility
{
    public static class VectorUtility
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            float F(float x) => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, F(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
        {
            float F(float x) => -4 * height * x * x + 4 * height * x;

            var mid = Vector2.Lerp(start, end, t);

            return new Vector2(mid.x, F(t) + Mathf.Lerp(start.y, end.y, t));
        }
    }
}