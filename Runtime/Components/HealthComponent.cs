using System;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Events;
using DarkBestiary.Properties;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class HealthComponent : Component
    {
        public static event Action<EntityDamagedEventData> AnyEntityDamaged;
        public static event Action<EntityHealedEventData> AnyEntityHealed;
        public static event Action<EntityDiedEventData> AnyEntityDied;
        public static event Action<GameObject> AnyEntityInvulnerable;

        public event Action<EntityDiedEventData> Died;
        public event Action<HealthComponent> HealthChanged;
        public event Action<HealthComponent> ShieldChanged;
        public event Action<EntityDamagedEventData> Damaged;
        public event Action<EntityHealedEventData> Healed;

        public float HealthMax => (int) (m_HealthProperty?.Value() ?? 0);
        public float HealthAndShield => (int) (Health + Shield);
        public float HealthAndShieldMax => (int) (HealthMax + Shield);
        public float HealthFraction => Health / HealthMax;
        public bool IsAlive { get; private set; } = true;
        public bool IsInvulnerable { get; set; }

        public float Shield => (int) m_Behaviours.Behaviours
            .OfType<ShieldBehaviour>().Sum(behaviour => behaviour.Amount);

        public float Health
        {
            get => (int) m_Health;
            set
            {
                m_Health = Mathf.Clamp(value, 0, HealthMax);
                HealthChanged?.Invoke(this);
            }
        }

        public float MissingHealth => HealthMax - Health;

        public float LastOverheal { get; private set; }

        private float m_Health;
        private PropertiesComponent m_Properties;
        private BehavioursComponent m_Behaviours;
        private DefenseComponent m_Defense;
        private Property m_HealthProperty;

        protected override void OnInitialize()
        {
            m_Behaviours = GetComponent<BehavioursComponent>();
            m_Defense = GetComponent<DefenseComponent>();

            m_Properties = GetComponent<PropertiesComponent>();

            m_HealthProperty = m_Properties.Get(PropertyType.Health);
            m_HealthProperty.Changed += OnHealthPropertyChanged;

            Health = m_HealthProperty.Value();

            if (Health < 1)
            {
                Health = 1;
            }

            ShieldBehaviour.AnyShieldChanged += AnyShieldChanged;
            CombatEncounter.AnyCombatRoundStarted += OnAnyCombatRoundStarted;
        }

        protected override void OnTerminate()
        {
            m_HealthProperty.Changed -= OnHealthPropertyChanged;

            ShieldBehaviour.AnyShieldChanged -= AnyShieldChanged;
            CombatEncounter.AnyCombatRoundStarted -= OnAnyCombatRoundStarted;
        }

        private void AnyShieldChanged(ShieldBehaviour behaviour)
        {
            if (behaviour.Target != gameObject)
            {
                return;
            }

            ShieldChanged?.Invoke(this);
        }

        private void OnAnyCombatRoundStarted(CombatEncounter combat)
        {
            if (!IsAlive)
            {
                return;
            }

            var regeneration = m_Properties.Get(PropertyType.HealthRegeneration).Value();

            if (Math.Abs(regeneration) < 0.01f)
            {
                return;
            }

            Heal(gameObject, new Healing(regeneration, HealFlags.Regeneration, null));
        }

        private void OnHealthPropertyChanged(Property property)
        {
            Health = Mathf.Clamp(Health, 0, HealthMax);
        }

        public void Kill(GameObject killer)
        {
            Kill(killer, new Damage(0, DamageType.Health, WeaponSound.None, DamageFlags.None, DamageInfoFlags.None, null));
        }

        public void Kill(GameObject killer, Damage damage)
        {
            if (!IsAlive)
            {
                return;
            }

            Health = 0;
            IsAlive = false;
            var eventData = new EntityDiedEventData(gameObject, killer, damage);
            AnyEntityDied?.Invoke(eventData);
            Died?.Invoke(eventData);
        }

        public void Restore()
        {
            Health = HealthMax;
        }

        public void Revive()
        {
            Health = HealthMax;
            IsAlive = true;
        }

        public Damage Damage(GameObject attacker, Damage damage)
        {
            if (!IsAlive || CombatEncounter.Active == null || !gameObject.activeSelf || IsInvulnerable)
            {
                damage.InfoFlags |= DamageInfoFlags.Invulnerable;
                damage.Amount = 0;
                return damage;
            }

            if (!damage.IsCleave())
            {
                damage = attacker.GetComponent<OffenseComponent>().Modify(gameObject, damage);
            }

            damage = m_Defense.Modify(attacker, damage);

            if (m_Behaviours.IsInvulnerable)
            {
                damage.InfoFlags |= DamageInfoFlags.Invulnerable;
                damage.Amount = 0;

                AnyEntityInvulnerable?.Invoke(gameObject);
            }
            else
            {
                damage = Absorb(damage);
                damage = MaybeSpiritLink(attacker, damage);
            }

            damage.Amount = Mathf.Min(Mathf.Ceil(damage.Amount), Health);

            if (Health - damage.Amount < 1 && m_Behaviours.IsImmortal)
            {
                Health = 1;
            }
            else
            {
                Health -= damage.Amount;
            }

            var eventData = new EntityDamagedEventData(attacker, gameObject, damage);
            Damaged?.Invoke(eventData);
            AnyEntityDamaged?.Invoke(eventData);

            MaybeKill(attacker, damage);

            return damage;
        }

        private void MaybeKill(GameObject attacker, Damage damage)
        {
            if (Health >= 1)
            {
                return;
            }

            Kill(attacker, damage);
        }

        private Damage MaybeSpiritLink(GameObject attacker, Damage damage)
        {
            if (damage.Amount < 1 ||
                damage.InfoFlags.HasFlag(DamageInfoFlags.SpiritLink) ||
                damage.InfoFlags.HasFlag(DamageInfoFlags.Dodged) ||
                damage.InfoFlags.HasFlag(DamageInfoFlags.Blocked))
            {
                return damage;
            }

            return m_Behaviours.Behaviours.OfType<SpiritLinkBehaviour>().FirstOrDefault()?
                       .ReduceAndDistribute(attacker, damage) ?? damage;
        }

        private Damage Absorb(Damage damage)
        {
            if (damage.Type == DamageType.Chaos || damage.Type == DamageType.Health)
            {
                return damage;
            }

            var absorbed = 0f;

            foreach (var shield in m_Behaviours.Behaviours.OfType<ShieldBehaviour>().ToList())
            {
                absorbed += shield.Absorb(ref damage);

                if (Math.Abs(damage) < Mathf.Epsilon)
                {
                    break;
                }
            }

            damage.Absorbed = absorbed;

            return damage;
        }

        public void Heal(GameObject source, Healing healing)
        {
            if (healing.Amount < 1)
            {
                return;
            }

            var increased = healing;
            increased *= 1 + m_Properties.Get(PropertyType.IncomingHealingIncrease).Value();

            if (!healing.IsVampirism() && !healing.IsRegeneration() && Rng.Test(m_Properties.Get(PropertyType.HealingCriticalChance).Value()))
            {
                increased *= 2;
            }

            var overheal = Mathf.Max(0, increased.Amount - (HealthMax - Health));

            if (healing.IsVampirism() || healing.IsRegeneration())
            {
                overheal /= 2.0f;
            }

            LastOverheal = overheal;

            Health += increased.Amount;

            var eventData = new EntityHealedEventData(source, gameObject, increased);
            Healed?.Invoke(eventData);
            AnyEntityHealed?.Invoke(eventData);
        }

        public void SyncHealthFraction(HealthComponent health)
        {
            Health = HealthMax * health.HealthFraction;
        }
    }
}