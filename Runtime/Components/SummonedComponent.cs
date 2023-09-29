using System;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class SummonedComponent : Component
    {
        public static event Action<SummonedComponent> AnySummonedComponentInitialized;

        public GameObject Master { get; private set; }
        public int RemainingLifetime { get; private set; }

        private int m_Duration;
        private bool m_KillOnMasterDeath;
        private bool m_KillOnEpisodeComplete;

        public SummonedComponent Construct(GameObject master, int duration,
            bool killOnMasterDeath, bool killOnEpisodeComplete)
        {
            Master = master;
            RemainingLifetime = duration;

            m_Duration = duration;
            m_KillOnMasterDeath = killOnMasterDeath;
            m_KillOnEpisodeComplete = killOnEpisodeComplete;

            return this;
        }

        protected override void OnInitialize()
        {
            if (m_Duration > 0)
            {
                CombatEncounter.AnyCombatTeamTurnStarted += OnAnyCombatTeamTurnStarted;
            }

            if (m_KillOnMasterDeath)
            {
                Master.GetComponent<HealthComponent>().Died += OnMasterDied;
            }

            if (m_KillOnEpisodeComplete)
            {
                Episode.AnyEpisodeCompleted += OnAnyEpisodeCompleted;
            }

            AnySummonedComponentInitialized?.Invoke(this);
        }

        protected override void OnTerminate()
        {
            if (m_Duration > 0)
            {
                CombatEncounter.AnyCombatTeamTurnStarted -= OnAnyCombatTeamTurnStarted;
            }

            if (m_KillOnMasterDeath)
            {
                Master.GetComponent<HealthComponent>().Died -= OnMasterDied;
            }

            if (m_KillOnEpisodeComplete)
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