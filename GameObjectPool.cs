using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class GameObjectPool : MemoryPool<GameObject>
    {
        private readonly List<GameObject> items = new List<GameObject>();

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
            foreach (var item in this.items.ToList())
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
            this.items.Remove(item);
        }

        protected override void OnSpawned(GameObject item)
        {
            item.SetActive(true);
            item.transform.SetAsLastSibling();
            this.items.Add(item);
        }
    }
}