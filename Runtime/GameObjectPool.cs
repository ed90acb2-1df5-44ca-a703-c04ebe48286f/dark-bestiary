using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class GameObjectPool : MemoryPool<GameObject>
    {
        private readonly List<GameObject> m_Items = new();

        public static GameObjectPool Factory(GameObject prefab, Transform transform)
        {
            return DarkBestiary.Container.Instance.Instantiate<GameObjectPool>(
                new object[]
                {
                    new MemoryPoolSettings {InitialSize = 10},
                    new GameObjectFactory(prefab, transform),
                });
        }

        public void DespawnAll()
        {
            foreach (var item in m_Items.ToList())
            {
                Despawn(item);
            }
        }

        protected override void OnCreated(GameObject item)
        {
            item.SetActive(false);
        }

        protected override void OnDespawned(GameObject item)
        {
            item.SetActive(false);
            m_Items.Remove(item);
        }

        protected override void OnSpawned(GameObject item)
        {
            item.SetActive(true);
            item.transform.SetAsLastSibling();
            m_Items.Add(item);
        }
    }
}