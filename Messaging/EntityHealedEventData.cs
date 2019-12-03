using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Messaging
{
    public struct EntityHealedEventData
    {
        public GameObject Healer { get; }
        public GameObject Target { get; }
        public Healing Healing { get; }

        public EntityHealedEventData(GameObject healer, GameObject target, Healing healing)
        {
            Healer = healer;
            Target = target;
            Healing = healing;
        }
    }
}