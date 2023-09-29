using System;
using DarkBestiary.Events;
using DarkBestiary.GameBoard;
using DarkBestiary.Properties;
using DarkBestiary.Values;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DarkBestiary.Components
{
    public class DefenseComponent : Component
    {
        public event Action<DefenseComponent> AttackDodged;
        public event Action<DefenseComponent> AttackBlocked;

        private PropertiesComponent m_Properties;
        private BehavioursComponent m_Behaviours;

        protected override void OnInitialize()
        {
            m_Properties = gameObject.GetComponent<PropertiesComponent>();
            m_Behaviours = gameObject.GetComponent<BehavioursComponent>();

            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        protected override void OnTerminate()
        {
            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
        }

        public Damage Modify(GameObject source, Damage damage)
        {
            if (m_Properties == null || damage.Type == DamageType.Chaos || damage.Type == DamageType.Health || damage.IsTrue())
            {
                return damage;
            }

            var multiplier = GetDamageMultiplier(source, damage);

            foreach (var behaviour in m_Behaviours.DefensiveDamageBehaviours())
            {
                multiplier += behaviour.GetDamageMultiplier(gameObject, source, ref damage);
            }

            damage -= damage * Mathf.Clamp01(multiplier);
            damage *= GetResistanceMultiplier(source, damage);
            damage = RollDiceDodge(source, damage);

            return damage.IsDodged() ? damage : RollDiceBlock(damage);
        }

        private float GetDamageMultiplier(GameObject source, Damage damage)
        {
            var multiplier = 0f;

            switch (damage.Type)
            {
                case DamageType.Crushing:
                    multiplier = m_Properties.Get(PropertyType.IncomingCrushingDamageReduction).Value();
                    break;
                case DamageType.Slashing:
                    multiplier = m_Properties.Get(PropertyType.IncomingSlashingDamageReduction).Value();
                    break;
                case DamageType.Piercing:
                    multiplier = m_Properties.Get(PropertyType.IncomingPiercingDamageReduction).Value();
                    break;
                case DamageType.Fire:
                    multiplier = m_Properties.Get(PropertyType.IncomingFireDamageReduction).Value();
                    break;
                case DamageType.Cold:
                    multiplier = m_Properties.Get(PropertyType.IncomingColdDamageReduction).Value();
                    break;
                case DamageType.Holy:
                    multiplier = m_Properties.Get(PropertyType.IncomingHolyDamageReduction).Value();
                    break;
                case DamageType.Shadow:
                    multiplier = m_Properties.Get(PropertyType.IncomingShadowDamageReduction).Value();
                    break;
                case DamageType.Arcane:
                    multiplier = m_Properties.Get(PropertyType.IncomingArcaneDamageReduction).Value();
                    break;
                case DamageType.Poison:
                    multiplier = m_Properties.Get(PropertyType.IncomingPoisonDamageReduction).Value();
                    break;
                case DamageType.Lightning:
                    multiplier = m_Properties.Get(PropertyType.IncomingLightningDamageReduction).Value();
                    break;
            }

            if (damage.IsPhysicalType())
            {
                multiplier += m_Properties.Get(PropertyType.IncomingPhysicalDamageReduction).Value();
            }

            if (damage.IsMagicalType())
            {
                multiplier += m_Properties.Get(PropertyType.IncomingMagicalDamageReduction).Value();
            }

            multiplier += m_Properties.Get(PropertyType.IncomingDamageReduction).Value();

            return Mathf.Clamp01(multiplier);
        }

        private float GetResistanceMultiplier(GameObject source, Damage damage)
        {
            float multiplier;

            switch (damage.Type)
            {
                case DamageType.Crushing:
                    multiplier = m_Properties.Get(PropertyType.CrushingResistance).Value();
                    break;
                case DamageType.Slashing:
                    multiplier = m_Properties.Get(PropertyType.SlashingResistance).Value();
                    break;
                case DamageType.Piercing:
                    multiplier = m_Properties.Get(PropertyType.PiercingResistance).Value();
                    break;
                case DamageType.Fire:
                    multiplier = m_Properties.Get(PropertyType.FireResistance).Value();
                    break;
                case DamageType.Cold:
                    multiplier = m_Properties.Get(PropertyType.ColdResistance).Value();
                    break;
                case DamageType.Holy:
                    multiplier = m_Properties.Get(PropertyType.HolyResistance).Value();
                    break;
                case DamageType.Shadow:
                    multiplier = m_Properties.Get(PropertyType.ShadowResistance).Value();
                    break;
                case DamageType.Arcane:
                    multiplier = m_Properties.Get(PropertyType.ArcaneResistance).Value();
                    break;
                case DamageType.Poison:
                    multiplier = m_Properties.Get(PropertyType.PoisonResistance).Value();
                    break;
                case DamageType.Lightning:
                    multiplier = m_Properties.Get(PropertyType.LightningResistance).Value();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(damage.Type), damage.Type, null);
            }

            if (damage.IsPhysicalType())
            {
                multiplier -= multiplier * source.GetComponent<PropertiesComponent>()
                                  .Get(PropertyType.ArmorPenetration)
                                  .Value();
            }

            if (damage.IsMagicalType())
            {
                multiplier -= multiplier * source.GetComponent<PropertiesComponent>()
                                  .Get(PropertyType.MagicPenetration)
                                  .Value();
            }

            return Mathf.Clamp(1.0f - multiplier, 0, 1);
        }

        private Damage RollDiceDodge(GameObject attacker, Damage damage)
        {
            if (damage.Flags.HasFlag(DamageFlags.CantBeDodged) || m_Behaviours.IsUncontrollable)
            {
                return damage;
            }

            var dodgeChance = m_Properties.Get(PropertyType.Dodge).Value();

            if (damage.IsMagicalType())
            {
                dodgeChance *= 0.5f;
            }

            if (!Rng.Test(dodgeChance))
            {
                return damage;
            }

            damage.InfoFlags |= DamageInfoFlags.Dodged;
            damage.Amount = 0;

            AttackDodged?.Invoke(this);

            return damage;
        }

        private Damage RollDiceBlock(Damage damage)
        {
            if (damage.Flags.HasFlag(DamageFlags.CantBeBlocked) || m_Behaviours.IsUncontrollable)
            {
                return damage;
            }

            var chance = m_Properties.Get(PropertyType.BlockChance).Value();

            if (damage.IsMagicalType())
            {
                chance *= 0.5f;
            }

            if (!Rng.Test(chance))
            {
                return damage;
            }

            damage.InfoFlags |= DamageInfoFlags.Blocked;

            AttackBlocked?.Invoke(this);

            return damage * (1 - m_Properties.Get(PropertyType.BlockAmount).Value());
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            MaybeThorns(data);
            MaybeReflect(data);
        }

        private void MaybeThorns(EntityDamagedEventData data)
        {
            if (!CanBeReflected(data.Target, data.Source, data.Damage))
            {
                return;
            }

            var thorns = m_Properties.Get(PropertyType.Thorns).Value();

            if ((data.Target.transform.position - data.Source.transform.position).magnitude > Board.Instance.CellSize * 2)
            {
                thorns /= 2;
            }

            if (thorns < 1)
            {
                return;
            }

            Timer.Instance.Wait(0.5f, () =>
            {
                var thornsDamage = new Damage(
                    thorns,
                    DamageType.Piercing,
                    WeaponSound.None,
                    DamageFlags.CantBeCritical | DamageFlags.CantBeBlocked | DamageFlags.CantBeDodged,
                    DamageInfoFlags.Reflected | DamageInfoFlags.Thorns,
                    null
                );

                data.Source.GetComponent<HealthComponent>().Damage(gameObject, thornsDamage);
            });
        }

        private void MaybeReflect(EntityDamagedEventData data)
        {
            if (!CanBeReflected(data.Target, data.Source, data.Damage))
            {
                return;
            }

            var reflection = m_Properties.Get(PropertyType.DamageReflection).Value();

            if (reflection < 0.01)
            {
                return;
            }

            var flags = data.Damage.Flags | DamageFlags.CantBeCritical | DamageFlags.CantBeBlocked |
                        DamageFlags.CantBeDodged;

            var reflected = new Damage(
                data.Damage.Amount, data.Damage.Type, data.Damage.WeaponSound, flags, DamageInfoFlags.Reflected, data.Damage.Skill);

            if (reflected < 1)
            {
                return;
            }

            Timer.Instance.Wait(0.5f, () =>
            {
                data.Source.GetComponent<HealthComponent>()
                    .Damage(gameObject, reflected * reflection);
            });
        }

        private bool CanBeReflected(Object victim, Object attacker, Damage damage)
        {
            return victim == gameObject &&
                   attacker != victim &&
                   damage.Type != DamageType.Health &&
                   !damage.IsTrue() &&
                   !damage.IsDodged() &&
                   !damage.IsReflected() &&
                   !damage.IsDot();
        }
    }
}