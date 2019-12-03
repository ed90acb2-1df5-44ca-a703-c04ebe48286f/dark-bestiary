using System;
using DarkBestiary.Movers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DarkBestiary.Data.Mappers
{
    public class MissileMapper : Mapper<MissileData, Missile>
    {
        private static readonly GameObject Parent = new GameObject("Missiles");

        public override MissileData ToData(Missile missile)
        {
            throw new NotImplementedException();
        }

        public override Missile ToEntity(MissileData data)
        {
            var prefab = Resources.Load<GameObject>(data.Prefab);

            if (prefab == null)
            {
                Debug.LogError("Can't find prefab " + data.Prefab);
                return null;
            }

            var missile = new GameObject(prefab.name).AddComponent<Missile>();
            missile.transform.SetParent(Parent.transform);
            missile.gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            var collider = missile.gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.1f;
            collider.isTrigger = true;

            missile.Graphics = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, missile.transform);
            missile.name = data.Name;
            missile.ImpactPrefab = data.ImpactPrefab;
            missile.Mover = Mover.Factory(new MoverData(MoverType.Linear, 5.0f, 0, 0, false));

            return missile;
        }
    }
}