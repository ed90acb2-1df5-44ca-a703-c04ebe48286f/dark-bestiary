using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class ObjectExtensions
    {
        public static Vector3 GetPosition(this object origin)
        {
            switch (origin)
            {
                case Transform transform:
                    return transform.position;
                case GameObject gameObject:
                    return gameObject.transform.position;
                default:
                    return (Vector3) origin;
            }
        }
    }
}