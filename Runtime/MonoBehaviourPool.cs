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
            return Factory(prefab, container, new T[0], size);
        }

        public static MonoBehaviourPool<T> Factory(T prefab, Transform container, IEnumerable<T> precreated, int size = 10)
        {
            return DarkBestiary.Container.Instance.Instantiate<MonoBehaviourPool<T>>(
                new object[]
                {
                    new MemoryPoolSettings {InitialSize = size},
                    new MonoBehaviourFactory<T>(prefab, container, precreated),
                }
            );
        }

        public IReadOnlyCollection<T> ActiveItems => m_ActiveItems;

        private readonly List<T> m_ActiveItems = new();

        public new void Clear()
        {
            DespawnAll();
            base.Clear();
        }

        public void DespawnAll()
        {
            foreach (var item in m_ActiveItems.ToList())
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
            m_ActiveItems.Remove(item);
        }

        protected override void OnSpawned(T item)
        {
            item.gameObject.SetActive(true);
            item.transform.SetAsLastSibling();
            item.OnSpawned(this);

            m_ActiveItems.Add(item);
        }
    }
}