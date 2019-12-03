using DarkBestiary.Components;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary
{
    public class StopParticleEmissionOnDeath : MonoBehaviour
    {
        private void Start()
        {
            var health = GetComponentInParent<HealthComponent>();

            if (health == null)
            {
                return;
            }

            health.Died += OnDeath;
        }

        private void OnDeath(EntityDiedEventData data)
        {
            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop();
            }
        }
    }
}