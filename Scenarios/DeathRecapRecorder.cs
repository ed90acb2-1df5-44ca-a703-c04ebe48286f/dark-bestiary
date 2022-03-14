using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;

namespace DarkBestiary.Scenarios
{
    public class DeathRecapRecorder
    {
        public IReadOnlyList<DeathRecapRecord> Records => this.records;
        private readonly List<DeathRecapRecord> records = new List<DeathRecapRecord>();

        public bool HasDied { get; private set; }

        public void Start()
        {
            this.records.Clear();

            HasDied = false;

            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
            HealthComponent.AnyEntityHealed += OnAnyEntityHealed;
        }

        public void Stop()
        {
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
            HealthComponent.AnyEntityHealed -= OnAnyEntityHealed;
        }

        private void OnAnyEntityDied(EntityDiedEventData payload)
        {
            HasDied = payload.Victim.IsCharacter();
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData payload)
        {
            if (!payload.Victim.IsCharacter())
            {
                return;
            }

            this.records.Add(new DeathRecapRecord(payload.Damage, payload.Attacker));
        }

        private void OnAnyEntityHealed(EntityHealedEventData payload)
        {
            if (!payload.Target.IsCharacter())
            {
                return;
            }

            this.records.Add(new DeathRecapRecord(payload.Healing, payload.Healer));
        }
    }
}