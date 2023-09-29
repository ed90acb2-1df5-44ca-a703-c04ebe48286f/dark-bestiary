using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class DamageMeterViewController : ViewController<IDamageMeterView>
    {
        public event Action DamageMetersUpdated;

        public readonly List<DamageMeter> DamageMeters = new();

        public DamageMeterViewController(IDamageMeterView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.Construct(this);

            Scenario.AnyScenarioStarted += OnAnyScenarioStarted;
            CombatEncounter.AnyCombatTurnStarted += OnEntityTurnStarted;
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;
            HealthComponent.AnyEntityDied += OnEntityDied;
        }

        protected override void OnTerminate()
        {
            Scenario.AnyScenarioStarted -= OnAnyScenarioStarted;
            CombatEncounter.AnyCombatTurnStarted -= OnEntityTurnStarted;
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
            HealthComponent.AnyEntityHealed -= OnEntityHealed;
            HealthComponent.AnyEntityDied -= OnEntityDied;
        }

        public void Reset()
        {
            DamageMeters.Clear();
            DamageMetersUpdated?.Invoke();
        }

        private void OnAnyScenarioStarted(Scenario _)
        {
            Reset();
        }

        private void OnEntityTurnStarted(GameObject subject)
        {
            FindOrCreateDamageMeter(subject).AddTurns(1);
            DamageMetersUpdated?.Invoke();
        }

        private void OnEntityDamaged(EntityDamagedEventData payload)
        {
            FindOrCreateDamageMeter(payload.Source).AddDamageDone(payload.Damage.Amount);
            FindOrCreateDamageMeter(payload.Target).AddDamageTaken(payload.Damage.Amount);
            DamageMetersUpdated?.Invoke();
        }

        private void OnEntityHealed(EntityHealedEventData payload)
        {
            FindOrCreateDamageMeter(payload.Source).AddHealingDone(payload.Healing.Amount);
            FindOrCreateDamageMeter(payload.Target).AddHealingTaken(payload.Healing.Amount);
            DamageMetersUpdated?.Invoke();
        }

        private void OnEntityDied(EntityDiedEventData payload)
        {
            FindOrCreateDamageMeter(payload.Victim).IsAlive = false;
        }

        private DamageMeter FindOrCreateDamageMeter(GameObject subject)
        {
            var damageMeter = DamageMeters.FirstOrDefault(x => x.Id == subject.GetHashCode());

            if (damageMeter == null)
            {
                damageMeter = new DamageMeter(subject.GetHashCode(), subject.GetComponent<UnitComponent>().Name, subject.IsAllyOfPlayer());
                DamageMeters.Add(damageMeter);
            }

            return damageMeter;
        }
    }
}