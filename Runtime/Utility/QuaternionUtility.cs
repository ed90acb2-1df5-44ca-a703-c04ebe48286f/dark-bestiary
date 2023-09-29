using UnityEngine;

namespace DarkBestiary.Utility
{
    public static class QuaternionUtility
    {
        public static Quaternion LookRotation2D(Vector3 delta)
        {
            return Quaternion.AngleAxis(Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg, Vector3.forward);
        }
    }
}