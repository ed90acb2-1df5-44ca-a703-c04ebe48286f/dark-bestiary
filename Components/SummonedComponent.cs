using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class SummonedComponent : Component
    {
        public GameObject Master { get; private set; }

        private int counter;
        private int duration;
        private bool isTicked;
        private bool killOnMasterDeath;
        private bool killOnEpisodeComplete;

        public SummonedComponent Construct(GameObject master, int duration,
            bool killOnMasterDeath, bool killOnEpisodeComplete)
        {
            Master = master;

            this.duration = duration;
            this.counter = duration;
            this.killOnMasterDeath = killOnMasterDeath;
            this.killOnEpisodeComplete = killOnEpisodeComplete;

            return this;
        }

        protected override void OnInitialize()
        {
            if (this.duration > 0)
            {
                CombatEncounter.AnyCombatTeamTurnStarted += OnAnyCombatTeamTurnStarted;
            }

            if (this.killOnMasterDeath)
            {
                Master.GetComponent<HealthComponent>().Died += OnMasterDied;
            }

            if (this.killOnEpisodeComplete)
            {
                Episode.AnyEpisodeCompleted += OnAnyEpisodeCompleted;
            }
        }

        protected override void OnTerminate()
        {
            if (this.duration > 0)
            {
                CombatEncounter.AnyCombatTeamTurnStarted -= OnAnyCombatTeamTurnStarted;
            }

            if (this.killOnMasterDeath)
            {
                Master.GetComponent<HealthComponent>().Died -= OnMasterDied;
            }

            if (this.killOnEpisodeComplete)
            {
                Episode.AnyEpisodeCompleted -= OnAnyEpisodeCompleted;
            }
        }

        private void OnAnyEpisodeCompleted(Episode episode)
        {
            GetComponent<HealthComponent>().Kill(Master);
        }

        private void OnMasterDied(EntityDiedEventData data)
        {
            GetComponent<HealthComponent>().Kill(Master);
        }

        private void OnAnyCombatTeamTurnStarted(CombatEncounter combat)
        {
            if (!gameObject.IsVisible() || combat.ActingTeamId != gameObject.GetComponent<UnitComponent>().TeamId)
            {
                return;
            }

            if (!this.isTicked)
            {
                this.isTicked = true;
                return;
            }

            this.counter -= 1;

            if (this.counter > 0)
            {
                return;
            }

            var health = GetComponent<HealthComponent>();

            if (health.IsAlive)
            {
                health.Kill(gameObject);
            }
        }
    }
}