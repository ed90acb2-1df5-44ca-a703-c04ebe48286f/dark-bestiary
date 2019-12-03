using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Messaging
{
    public struct EntityDamagedEventData
    {
        public GameObject Attacker { get; }
        public GameObject Victim { get; }
        public Damage Damage { get; }

        public EntityDamagedEventData(GameObject attacker, GameObject victim, Damage damage)
        {
            Attacker = attacker;
            Victim = victim;
            Damage = damage;
        }
    }
}