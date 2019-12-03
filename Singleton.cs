using UnityEngine;

namespace DarkBestiary
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = (T) FindObjectOfType(typeof(T));

                if (instance != null)
                {
                    return instance;
                }

                Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                return null;
            }
        }
    }
}