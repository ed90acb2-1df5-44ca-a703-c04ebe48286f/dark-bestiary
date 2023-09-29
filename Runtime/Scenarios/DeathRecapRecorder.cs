using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;

namespace DarkBestiary.Scenarios
{
    public class DeathRecapRecorder
    {
        public IReadOnlyList<DeathRecapRecord> Records => m_Records;
        private readonly List<DeathRecapRecord> m_Records = new();

        public bool HasDied { get; private set; }

        public void Start()
        {
            m_Records.Clear();

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
            if (!payload.Target.IsCharacter())
            {
                return;
            }

            m_Records.Add(new DeathRecapRecord(payload.Damage, payload.Source));
        }

        private void OnAnyEntityHealed(EntityHealedEventData payload)
        {
            if (!payload.Target.IsCharacter())
            {
                return;
            }

            m_Records.Add(new DeathRecapRecord(payload.Healing, payload.Source));
        }
    }
}