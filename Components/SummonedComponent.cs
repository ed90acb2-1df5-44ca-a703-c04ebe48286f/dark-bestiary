using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class SummonedComponent : Component
    {
        public static event Payload<SummonedComponent> AnySummonedComponentInitialized;

        public GameObject Master { get; private set; }
        public int RemainingLifetime { get; private set; }

        private int duration;
        private bool killOnMasterDeath;
        private bool killOnEpisodeComplete;

        public SummonedComponent Construct(GameObject master, int duration,
            bool killOnMasterDeath, bool killOnEpisodeComplete)
        {
            Master = master;
            RemainingLifetime = duration;

            this.duration = duration;
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

            AnySummonedComponentInitialized?.Invoke(this);
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
            Timer.Instance.WaitForFixedUpdate(() => GetComponent<HealthComponent>().Kill(Master));
        }

        private void OnMasterDied(EntityDiedEventData data)
        {
            Timer.Instance.WaitForFixedUpdate(() => GetComponent<HealthComponent>().Kill(Master));
        }

        private void OnAnyCombatTeamTurnStarted(CombatEncounter combat)
        {
            if (!gameObject.IsVisible() || combat.ActingTeamId != gameObject.GetComponent<UnitComponent>().TeamId)
            {
                return;
            }

            RemainingLifetime -= 1;

            if (RemainingLifetime > 0)
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