using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Events
{
    public struct EntityDamagedEventData
    {
        public GameObject Source { get; }
        public GameObject Target { get; }
        public Damage Damage { get; }

        public EntityDamagedEventData(GameObject source, GameObject target, Damage damage)
        {
            Source = source;
            Target = target;
            Damage = damage;
        }
    }
}