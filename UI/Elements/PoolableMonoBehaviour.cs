using UnityEngine;
using Zenject;

namespace DarkBestiary.UI.Elements
{
    public class PoolableMonoBehaviour : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private IMemoryPool pool;

        public void Despawn()
        {
            this.pool.Despawn(this);
        }

        public void OnSpawned(IMemoryPool pool)
        {
            this.pool = pool;
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