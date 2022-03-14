using System;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class HealthComponent : Component
    {
        public static event Payload<EntityDamagedEventData> AnyEntityDamaged;
        public static event Payload<EntityHealedEventData> AnyEntityHealed;
        public static event Payload<EntityDiedEventData> AnyEntityDied;
        public static event Payload<GameObject> AnyEntityInvulnerable;

        public event Payload<EntityDiedEventData> Died;
        public event Payload<HealthComponent> HealthChanged;
        public event Payload<HealthComponent> ShieldChanged;
        public event Payload<EntityDamagedEventData> Damaged;
        public event Payload<EntityHealedEventData> Healed;

        public float HealthMax => (int) (this.healthProperty?.Value() ?? 0);
        public float HealthAndShield => (int) (Health + Shield);
        public float HealthAndShieldMax => (int) (HealthMax + Shield);
        public float HealthFraction => Health / HealthMax;
        public bool IsAlive { get; private set; } = true;
        public bool IsInvulnerable { get; set; }

        public float Shield => (int) this.behaviours.Behaviours
            .OfType<ShieldBehaviour>().Sum(behaviour => behaviour.Amount);

        public float Health
        {
            get => (int) this.health;
            set
            {
                this.health = Mathf.Clamp(value, 0, HealthMax);
                HealthChanged?.Invoke(this);
            }
        }

        public float MissingHealth => HealthMax - Health;

        public float LastOverheal { get; private set; }

        private float health;
        private PropertiesComponent properties;
        private BehavioursComponent behaviours;
        private DefenseComponent defense;
        private Property healthProperty;

        protected override void OnInitialize()
        {
            this.behaviours = GetComponent<BehavioursComponent>();
            this.defense = GetComponent<DefenseComponent>();

            this.properties = GetComponent<PropertiesComponent>();

            this.healthProperty = this.properties.Get(PropertyType.Health);
            this.healthProperty.Changed += OnHealthPropertyChanged;

            Health = this.healthProperty.Value();

            if (Health < 1)
            {
                Health = 1;
            }

            ShieldBehaviour.AnyShieldChanged += AnyShieldChanged;
            CombatEncounter.AnyCombatRoundStarted += OnAnyCombatRoundStarted;
        }

        protected override void OnTerminate()
        {
            this.healthProperty.Changed -= OnHealthPropertyChanged;

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

            var regeneration = this.properties.Get(PropertyType.HealthRegeneration).Value();

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

            damage = this.defense.Modify(attacker, damage);

            if (this.behaviours.IsInvulnerable)
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

            if (Health - damage.Amount < 1 && this.behaviours.IsImmortal)
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

            return this.behaviours.Behaviours.OfType<SpiritLinkBehaviour>().FirstOrDefault()?
                       .ReduceAndDistribute(attacker, damage) ?? damage;
        }

        private Damage Absorb(Damage damage)
        {
            if (damage.Type == DamageType.Chaos || damage.Type == DamageType.Health)
            {
                return damage;
            }

            var absorbed = 0f;

            foreach (var shield in this.behaviours.Behaviours.OfType<ShieldBehaviour>().ToList())
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
            increased *= 1 + this.properties.Get(PropertyType.IncomingHealingIncrease).Value();

            if (!healing.IsVampirism() && !healing.IsRegeneration() && RNG.Test(this.properties.Get(PropertyType.HealingCriticalChance).Value()))
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