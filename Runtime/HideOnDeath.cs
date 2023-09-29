using DarkBestiary.Components;
using DarkBestiary.Events;
using UnityEngine;

namespace DarkBestiary
{
    public class HideOnDeath : MonoBehaviour
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

        private static void OnDeath(EntityDiedEventData data)
        {
            data.Victim.GetComponent<ActorComponent>().Hide();
        }
    }
}