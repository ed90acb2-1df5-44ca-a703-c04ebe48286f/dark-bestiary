using UnityEngine;
using Zenject;

namespace DarkBestiary
{
    public class GameObjectFactory : IFactory<GameObject>
    {
        private readonly GameObject prefab;
        private readonly Transform transform;

        public GameObjectFactory(GameObject prefab, Transform transform)
        {
            this.prefab = prefab;
            this.transform = transform;
        }

        public GameObject Create()
        {
            return Object.Instantiate(this.prefab, this.transform);
        }
    }
}