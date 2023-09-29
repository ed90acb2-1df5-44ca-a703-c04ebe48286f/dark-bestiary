using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class MonoBehaviourFactory<T> : IFactory<T> where T : MonoBehaviour
    {
        private readonly T m_Prefab;
        private readonly Transform m_Transform;
        private readonly Stack<T> m_Precreated;

        public MonoBehaviourFactory(T prefab, Transform transform, IEnumerable<T> precreated)
        {
            m_Prefab = prefab;
            m_Transform = transform;
            m_Precreated = new Stack<T>(precreated);
        }

        public T Create()
        {
            if (m_Precreated.Count > 0)
            {
                return m_Precreated.Pop();
            }

            return Object.Instantiate(m_Prefab, m_Transform);
        }
    }
}