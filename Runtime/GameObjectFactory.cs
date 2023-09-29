using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class GameObjectFactory : IFactory<GameObject>
    {
        private readonly GameObject m_Prefab;
        private readonly Transform m_Transform;

        public GameObjectFactory(GameObject prefab, Transform transform)
        {
            m_Prefab = prefab;
            m_Transform = transform;
        }

        public GameObject Create()
        {
            return Object.Instantiate(m_Prefab, m_Transform);
        }
    }
}