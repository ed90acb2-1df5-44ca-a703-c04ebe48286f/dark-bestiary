using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Events
{
    public struct EntityHealedEventData
    {
        public GameObject Source { get; }
        public GameObject Target { get; }
        public Healing Healing { get; }

        public EntityHealedEventData(GameObject source, GameObject target, Healing healing)
        {
            Source = source;
            Target = target;
            Healing = healing;
        }
    }
}