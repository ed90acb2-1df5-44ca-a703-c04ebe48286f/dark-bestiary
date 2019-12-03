using DarkBestiary.Components;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class WeaponSoundManager : MonoBehaviour
    {
        private void Start()
        {
            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            var armorSound = data.Victim.GetComponent<UnitComponent>().ArmorSound;

            if (armorSound == ArmorSound.None ||
                data.Damage.WeaponSound == WeaponSound.None ||
                data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Reflected) ||
                data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Cleave))
            {
                return;
            }

            AudioManager.Instance.PlayOneShot($"event:/SFX/Weapon/{data.Damage.WeaponSound}_On_{armorSound}");
        }
    }
}