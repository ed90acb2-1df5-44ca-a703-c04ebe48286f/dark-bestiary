using System.Collections.Generic;
using System.Linq;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class MonoBehaviourPool<T> : MemoryPool<T> where T : PoolableMonoBehaviour
    {
        public static MonoBehaviourPool<T> Factory(T prefab, Transform container, int size = 10)
        {
            return DarkBestiary.Container.Instance.Instantiate<MonoBehaviourPool<T>>(
                new object[]
                {
                    new MemoryPoolSettings {InitialSize = size},
                    new MonoBehaviourFactory<T>(prefab, container),
                });
        }

        public IReadOnlyCollection<T> Items => this.items;

        private readonly List<T> items = new List<T>();

        public new void Clear()
        {
            DespawnAll();
            base.Clear();
        }

        public void DespawnAll()
        {
            foreach (var item in this.items.ToList())
            {
                Despawn(item);
            }
        }

        protected override void OnCreated(T item)
        {
            item.gameObject.SetActive(false);
        }

        protected override void OnDespawned(T item)
        {
            item.gameObject.SetActive(false);
            item.OnDespawned();
            this.items.Remove(item);
        }

        protected override void OnSpawned(T item)
        {
            item.gameObject.SetActive(true);
            item.transform.SetAsLastSibling();
            item.OnSpawned(this);

            this.items.Add(item);
        }
    }
}