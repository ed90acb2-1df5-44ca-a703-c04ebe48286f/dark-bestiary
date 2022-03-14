using System;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.Values;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DarkBestiary.Components
{
    public class DefenseComponent : Component
    {
        public event Payload<DefenseComponent> AttackDodged;
        public event Payload<DefenseComponent> AttackBlocked;

        private PropertiesComponent properties;
        private BehavioursComponent behaviours;

        protected override void OnInitialize()
        {
            this.properties = gameObject.GetComponent<PropertiesComponent>();
            this.behaviours = gameObject.GetComponent<BehavioursComponent>();

            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
        }

        protected override void OnTerminate()
        {
            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
        }

        public Damage Modify(GameObject source, Damage damage)
        {
            if (this.properties == null || damage.Type == DamageType.Chaos || damage.Type == DamageType.Health || damage.IsTrue())
            {
                return damage;
            }

            var multiplier = GetDamageMultiplier(source, damage);

            foreach (var behaviour in this.behaviours.DefensiveDamageBehaviours())
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
                    multiplier = this.properties.Get(PropertyType.IncomingCrushingDamageReduction).Value();
                    break;
                case DamageType.Slashing:
                    multiplier = this.properties.Get(PropertyType.IncomingSlashingDamageReduction).Value();
                    break;
                case DamageType.Piercing:
                    multiplier = this.properties.Get(PropertyType.IncomingPiercingDamageReduction).Value();
                    break;
                case DamageType.Fire:
                    multiplier = this.properties.Get(PropertyType.IncomingFireDamageReduction).Value();
                    break;
                case DamageType.Cold:
                    multiplier = this.properties.Get(PropertyType.IncomingColdDamageReduction).Value();
                    break;
                case DamageType.Holy:
                    multiplier = this.properties.Get(PropertyType.IncomingHolyDamageReduction).Value();
                    break;
                case DamageType.Shadow:
                    multiplier = this.properties.Get(PropertyType.IncomingShadowDamageReduction).Value();
                    break;
                case DamageType.Arcane:
                    multiplier = this.properties.Get(PropertyType.IncomingArcaneDamageReduction).Value();
                    break;
                case DamageType.Poison:
                    multiplier = this.properties.Get(PropertyType.IncomingPoisonDamageReduction).Value();
                    break;
                case DamageType.Lightning:
                    multiplier = this.properties.Get(PropertyType.IncomingLightningDamageReduction).Value();
                    break;
            }

            if (damage.IsPhysicalType())
            {
                multiplier += this.properties.Get(PropertyType.IncomingPhysicalDamageReduction).Value();
            }

            if (damage.IsMagicalType())
            {
                multiplier += this.properties.Get(PropertyType.IncomingMagicalDamageReduction).Value();
            }

            multiplier += this.properties.Get(PropertyType.IncomingDamageReduction).Value();

            return Mathf.Clamp01(multiplier);
        }

        private float GetResistanceMultiplier(GameObject source, Damage damage)
        {
            float multiplier;

            switch (damage.Type)
            {
                case DamageType.Crushing:
                    multiplier = this.properties.Get(PropertyType.CrushingResistance).Value();
                    break;
                case DamageType.Slashing:
                    multiplier = this.properties.Get(PropertyType.SlashingResistance).Value();
                    break;
                case DamageType.Piercing:
                    multiplier = this.properties.Get(PropertyType.PiercingResistance).Value();
                    break;
                case DamageType.Fire:
                    multiplier = this.properties.Get(PropertyType.FireResistance).Value();
                    break;
                case DamageType.Cold:
                    multiplier = this.properties.Get(PropertyType.ColdResistance).Value();
                    break;
                case DamageType.Holy:
                    multiplier = this.properties.Get(PropertyType.HolyResistance).Value();
                    break;
                case DamageType.Shadow:
                    multiplier = this.properties.Get(PropertyType.ShadowResistance).Value();
                    break;
                case DamageType.Arcane:
                    multiplier = this.properties.Get(PropertyType.ArcaneResistance).Value();
                    break;
                case DamageType.Poison:
                    multiplier = this.properties.Get(PropertyType.PoisonResistance).Value();
                    break;
                case DamageType.Lightning:
                    multiplier = this.properties.Get(PropertyType.LightningResistance).Value();
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
            if (damage.Flags.HasFlag(DamageFlags.CantBeDodged) || this.behaviours.IsUncontrollable)
            {
                return damage;
            }

            var dodgeChance = this.properties.Get(PropertyType.Dodge).Value();

            if (damage.IsMagicalType())
            {
                dodgeChance *= 0.5f;
            }

            if (!RNG.Test(dodgeChance))
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
            if (damage.Flags.HasFlag(DamageFlags.CantBeBlocked) || this.behaviours.IsUncontrollable)
            {
                return damage;
            }

            var chance = this.properties.Get(PropertyType.BlockChance).Value();

            if (damage.IsMagicalType())
            {
                chance *= 0.5f;
            }

            if (!RNG.Test(chance))
            {
                return damage;
            }

            damage.InfoFlags |= DamageInfoFlags.Blocked;

            AttackBlocked?.Invoke(this);

            return damage * (1 - this.properties.Get(PropertyType.BlockAmount).Value());
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            MaybeThorns(data);
            MaybeReflect(data);
        }

        private void MaybeThorns(EntityDamagedEventData data)
        {
            if (!CanBeReflected(data.Victim, data.Attacker, data.Damage))
            {
                return;
            }

            var thorns = this.properties.Get(PropertyType.Thorns).Value();

            if ((data.Victim.transform.position - data.Attacker.transform.position).magnitude > Board.Instance.CellSize * 2)
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

                data.Attacker.GetComponent<HealthComponent>().Damage(gameObject, thornsDamage);
            });
        }

        private void MaybeReflect(EntityDamagedEventData data)
        {
            if (!CanBeReflected(data.Victim, data.Attacker, data.Damage))
            {
                return;
            }

            var reflection = this.properties.Get(PropertyType.DamageReflection).Value();

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
                data.Attacker.GetComponent<HealthComponent>()
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
                   !damage.IsDOT();
        }
    }
}