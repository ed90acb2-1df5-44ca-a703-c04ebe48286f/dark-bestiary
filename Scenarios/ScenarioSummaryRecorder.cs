using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios.Encounters;

namespace DarkBestiary.Scenarios
{
    public class ScenarioSummaryRecorder
    {
        private int rounds;
        private float damageDealt;
        private float damageTaken;
        private float healing;

        public void Start()
        {
            this.rounds = 0;
            this.damageDealt = 0;
            this.damageTaken = 0;
            this.healing = 0;

            CombatEncounter.AnyCombatRoundStarted += OnRoundStarted;
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;
        }

        public void Stop()
        {
            CombatEncounter.AnyCombatRoundStarted -= OnRoundStarted;
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
            HealthComponent.AnyEntityHealed -= OnEntityHealed;
        }

        public ScenarioSummary GetResult()
        {
            return new ScenarioSummary(this.rounds, this.damageDealt, this.damageTaken, this.healing);
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            if (data.Healer.IsOwnedByPlayer())
            {
                this.healing += data.Healing;
            }
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Attacker.IsOwnedByPlayer())
            {
                this.damageDealt += data.Damage;
            }
            else
            {
                this.damageTaken += data.Damage;
            }
        }

        private void OnRoundStarted(CombatEncounter combat)
        {
            this.rounds++;
        }
    }
}