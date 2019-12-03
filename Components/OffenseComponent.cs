using System;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class OffenseComponent : Component
    {
        private PropertiesComponent properties;
        private BehavioursComponent behaviours;
        private HealthComponent health;

        protected override void OnInitialize()
        {
            this.properties = gameObject.GetComponent<PropertiesComponent>();
            this.behaviours = gameObject.GetComponent<BehavioursComponent>();
            this.health = gameObject.GetComponent<HealthComponent>();

            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
        }

        protected override void OnTerminate()
        {
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
        }

        public Damage Modify(GameObject target, Damage damage)
        {
            if (this.properties == null || damage.Type == DamageType.Health)
            {
                return damage;
            }

            foreach (var behaviour in this.behaviours.OffensiveDamageBehaviours())
            {
                damage = behaviour.Modify(target, gameObject, damage);
            }

            damage *= GetDamageIncreaseMultiplier(damage);

            return RollDiceCritical(damage);
        }

        public float GetDamageIncreaseMultiplier(Damage damage)
        {
            if (damage.Type == DamageType.Health)
            {
                return 1;
            }

            var multiplier = 1 + this.properties.Get(PropertyType.DamageIncrease).Value();

            if (damage.Flags.HasFlag(DamageFlags.Melee))
            {
                multiplier += this.properties.Get(PropertyType.MeleeDamageIncrease).Value();
            }

            if (damage.Flags.HasFlag(DamageFlags.Ranged))
            {
                multiplier += this.properties.Get(PropertyType.RangedDamageIncrease).Value();
            }

            if (damage.Flags.HasFlag(DamageFlags.Magic))
            {
                multiplier += this.properties.Get(PropertyType.MagicalDamageIncrease).Value();
            }

            if (damage.Type == DamageType.Piercing || damage.Type == DamageType.Crushing ||
                damage.Type == DamageType.Slashing)
            {
                multiplier += this.properties.Get(PropertyType.PhysicalDamageIncrease).Value();
            }

            switch (damage.Type)
            {
                case DamageType.Piercing:
                    multiplier += this.properties.Get(PropertyType.PiercingDamageIncrease).Value();
                    break;
                case DamageType.Slashing:
                    multiplier += this.properties.Get(PropertyType.SlashingDamageIncrease).Value();
                    break;
                case DamageType.Crushing:
                    multiplier += this.properties.Get(PropertyType.CrushingDamageIncrease).Value();
                    break;
                case DamageType.Fire:
                    multiplier += this.properties.Get(PropertyType.FireDamageIncrease).Value();
                    break;
                case DamageType.Cold:
                    multiplier += this.properties.Get(PropertyType.ColdDamageIncrease).Value();
                    break;
                case DamageType.Shadow:
                    multiplier += this.properties.Get(PropertyType.ShadowDamageIncrease).Value();
                    break;
                case DamageType.Holy:
                    multiplier += this.properties.Get(PropertyType.HolyDamageIncrease).Value();
                    break;
                case DamageType.Arcane:
                    multiplier += this.properties.Get(PropertyType.ArcaneDamageIncrease).Value();
                    break;
                case DamageType.Lightning:
                    multiplier += this.properties.Get(PropertyType.LightningDamageIncrease).Value();
                    break;
                case DamageType.Poison:
                    multiplier += this.properties.Get(PropertyType.PoisonDamageIncrease).Value();
                    break;
            }

            return multiplier;
        }

        private Damage RollDiceCritical(Damage damage)
        {
            if (damage.Flags.HasFlag(DamageFlags.CantBeCritical))
            {
                return damage;
            }

            if (RNG.Test(this.properties.Get(PropertyType.CriticalHitChance).Value()))
            {
                damage.InfoFlags |= DamageInfoFlags.Critical;
                return damage * this.properties.Get(PropertyType.CriticalHitDamage).Value();
            }

            return damage;
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if ((data.Damage < 1 && data.Damage.Absorbed > 0) || data.Attacker != gameObject || data.Attacker == data.Victim ||
                data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Reflected))
            {
                return;
            }

            var vampirism = this.properties.Get(PropertyType.Vampirism).Value();

            if (Math.Abs(vampirism) < 0.01)
            {
                return;
            }

            // Note: Shield Explode deals damage then removes shields, to prevent "overheal"
            // shield removal we need to wait a little bit.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                this.health.Heal(gameObject, new Healing(data.Damage * vampirism, HealingFlags.Vampirism));
            });
        }
    }
}