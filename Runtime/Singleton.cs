using UnityEngine;

namespace DarkBestiary
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                if (s_Instance != null)
                {
                    return s_Instance;
                }

                s_Instance = (T) FindObjectOfType(typeof(T));

                if (s_Instance != null)
                {
                    return s_Instance;
                }

                Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                return null;
            }
        }
    }
}