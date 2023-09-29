using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Scenarios
{
    public class DeathRecapRecord
    {
        public Damage Damage { get; }
        public Healing Healing { get; }
        public bool IsDamage { get; }
        public bool IsHealing { get; }
        public GameObject Source { get; }

        public DeathRecapRecord(Damage damage, GameObject source)
        {
            Damage = damage;
            Source = source;
            IsDamage = true;
        }

        public DeathRecapRecord(Healing healing, GameObject source)
        {
            Healing = healing;
            Source = source;
            IsHealing = true;
        }
    }
}