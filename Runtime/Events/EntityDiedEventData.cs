using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Events
{
    public struct EntityDiedEventData
    {
        public GameObject Victim { get; }
        public GameObject Killer { get; }
        public Damage Damage { get; }

        public EntityDiedEventData(GameObject victim, GameObject killer, Damage damage)
        {
            Victim = victim;
            Killer = killer;
            Damage = damage;
        }
    }
}