using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class MonoBehaviourFactory<T> : IFactory<T> where T : MonoBehaviour
    {
        private readonly T prefab;
        private readonly Transform transform;
        private readonly Stack<T> precreated;

        public MonoBehaviourFactory(T prefab, Transform transform, IEnumerable<T> precreated)
        {
            this.prefab = prefab;
            this.transform = transform;
            this.precreated = new Stack<T>(precreated);
        }

        public T Create()
        {
            if (this.precreated.Count > 0)
            {
                return this.precreated.Pop();
            }

            return Object.Instantiate(this.prefab, this.transform);
        }
    }
}