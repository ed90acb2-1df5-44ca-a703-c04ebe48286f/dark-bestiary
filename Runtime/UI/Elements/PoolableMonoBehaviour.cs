using UnityEngine;
using Zenject;

namespace DarkBestiary.UI.Elements
{
    public class PoolableMonoBehaviour : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private IMemoryPool m_Pool;

        public void Despawn()
        {
            m_Pool.Despawn(this);
        }

        public void OnSpawned(IMemoryPool pool)
        {
            m_Pool = pool;
            OnSpawn();
        }

        public void OnDespawned()
        {
            OnDespawn();
        }

        protected virtual void OnSpawn()
        {
        }

        protected virtual void OnDespawn()
        {
        }
    }
}