using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class MonoBehaviourFactory<T> : IFactory<T> where T : MonoBehaviour
    {
        private readonly T prefab;
        private readonly Transform transform;

        public MonoBehaviourFactory(T prefab, Transform transform)
        {
            this.prefab = prefab;
            this.transform = transform;
        }

        public T Create()
        {
            return Object.Instantiate(this.prefab, this.transform);
        }
    }
}