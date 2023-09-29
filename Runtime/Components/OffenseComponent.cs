using System;
using DarkBestiary.Events;
using DarkBestiary.Properties;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class OffenseComponent : Component
    {
        private PropertiesComponent m_Properties;
        private BehavioursComponent m_Behaviours;
        private HealthComponent m_Health;

        protected override void OnInitialize()
        {
            m_Properties = gameObject.GetComponent<PropertiesComponent>();
            m_Behaviours = gameObject.GetComponent<BehavioursComponent>();
            m_Health = gameObject.GetComponent<HealthComponent>();

            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
        }

        protected override void OnTerminate()
        {
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
        }

        public Damage Modify(GameObject victim, Damage damage)
        {
            if (m_Properties == null || damage.Type == DamageType.Health || damage.IsTrue())
            {
                return damage;
            }

            var multiplier = GetDamageMultiplier(damage);

            foreach (var behaviour in m_Behaviours.OffensiveDamageBehaviours())
            {
                multiplier += behaviour.GetDamageMultiplier(victim, gameObject, ref damage);
            }

            damage *= multiplier;

            return RollDiceCritical(damage);
        }

        public float GetDamageMultiplier(Damage damage)
        {
            if (damage.Type == DamageType.Health || damage.IsTrue())
            {
                return 1;
            }

            var multiplier = 1 + m_Properties.Get(PropertyType.DamageIncrease).Value();

            if (damage.Flags.HasFlag(DamageFlags.Melee))
            {
                multiplier += m_Properties.Get(PropertyType.MeleeDamageIncrease).Value();
            }

            if (damage.Flags.HasFlag(DamageFlags.Ranged))
            {
                multiplier += m_Properties.Get(PropertyType.RangedDamageIncrease).Value();
            }

            if (damage.Flags.HasFlag(DamageFlags.Magic))
            {
                multiplier += m_Properties.Get(PropertyType.MagicalDamageIncrease).Value();
            }

            if (damage.Type == DamageType.Piercing || damage.Type == DamageType.Crushing ||
                damage.Type == DamageType.Slashing)
            {
                multiplier += m_Properties.Get(PropertyType.PhysicalDamageIncrease).Value();
            }

            switch (damage.Type)
            {
                case DamageType.Piercing:
                    multiplier += m_Properties.Get(PropertyType.PiercingDamageIncrease).Value();
                    break;
                case DamageType.Slashing:
                    multiplier += m_Properties.Get(PropertyType.SlashingDamageIncrease).Value();
                    break;
                case DamageType.Crushing:
                    multiplier += m_Properties.Get(PropertyType.CrushingDamageIncrease).Value();
                    break;
                case DamageType.Fire:
                    multiplier += m_Properties.Get(PropertyType.FireDamageIncrease).Value();
                    break;
                case DamageType.Cold:
                    multiplier += m_Properties.Get(PropertyType.ColdDamageIncrease).Value();
                    break;
                case DamageType.Shadow:
                    multiplier += m_Properties.Get(PropertyType.ShadowDamageIncrease).Value();
                    break;
                case DamageType.Holy:
                    multiplier += m_Properties.Get(PropertyType.HolyDamageIncrease).Value();
                    break;
                case DamageType.Arcane:
                    multiplier += m_Properties.Get(PropertyType.ArcaneDamageIncrease).Value();
                    break;
                case DamageType.Lightning:
                    multiplier += m_Properties.Get(PropertyType.LightningDamageIncrease).Value();
                    break;
                case DamageType.Poison:
                    multiplier += m_Properties.Get(PropertyType.PoisonDamageIncrease).Value();
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

            if (Rng.Test(m_Properties.Get(PropertyType.CriticalHitChance).Value()))
            {
                damage.InfoFlags |= DamageInfoFlags.Critical;
                return damage * m_Properties.Get(PropertyType.CriticalHitDamage).Value();
            }

            return damage;
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Damage < 1 && data.Damage.Absorbed > 0 || data.Source != gameObject || data.Source == data.Target ||
                data.Damage.InfoFlags.HasFlag(DamageInfoFlags.Reflected))
            {
                return;
            }

            var vampirism = m_Properties.Get(PropertyType.Vampirism).Value();

            if (Math.Abs(vampirism) < 0.01)
            {
                return;
            }

            if (data.Target.GetComponent<BehavioursComponent>().IsBleeding)
            {
                vampirism *= 1.5f;
            }

            // Note: "Shield Explode" deals damage first then removes shields, to prevent "overheal"
            // shield removal we need to wait a little bit.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                var amount = Mathf.Max(1, data.Damage * vampirism);

                m_Health.Heal(gameObject, new Healing(amount, HealFlags.Vampirism, data.Damage.Skill));
            });
        }
    }
}