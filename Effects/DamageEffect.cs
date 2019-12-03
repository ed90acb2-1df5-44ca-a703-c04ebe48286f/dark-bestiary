using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class DamageEffect : FormulaBasedEffect
    {
        protected readonly DamageEffectData Data;
        protected readonly IEffectRepository EffectRepository;

        public DamageEffect(DamageEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.Data = data;
            this.EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new DamageEffect(this.Data, this.Validators, this.EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var amount = Modify(GetAmountVariance(caster, target), caster, target) * StackCount;

            if (this.Data.Effects.Count > 0)
            {
                amount += this.EffectRepository.Find(this.Data.Effects)
                    .OfType<DamageEffect>()
                    .Aggregate(0f, (current, effect) => current + effect.GetAmountVariance(caster, target));
            }

            var damageType = GetDamageType();

            if (MaybeHealInstead(caster, target, amount, damageType))
            {
                TriggerFinished();
                return;
            }

            var damage = target.GetComponent<HealthComponent>().Damage(caster,
                new Damage(amount, damageType, GetWeaponSound(), this.Data.DamageFlags, this.Data.DamageInfoFlags)
            );

            if (damage.IsDodged() || damage.IsInvulnerable())
            {
                IsFailed = true;
                TriggerFinished();
                return;
            }

            MaybeApplyVampirism(caster, damage);
            MaybeApplyOnCritEffect(caster, target, damage);

            TriggerFinished();
        }

        private bool MaybeHealInstead(GameObject caster, GameObject target, float amount, DamageType damageType)
        {
            if (target.GetComponent<BehavioursComponent>().IsUndead && damageType == DamageType.Poison)
            {
                target.GetComponent<HealthComponent>().Heal(caster, new Healing(amount));
                return true;
            }

            return false;
        }

        private void MaybeApplyVampirism(GameObject caster, Damage damage)
        {
            if (damage.Amount < 1 || Mathf.Approximately(this.Data.Vampirism, 0))
            {
                return;
            }

            var healing = new Healing(damage * this.Data.Vampirism, HealingFlags.Vampirism);

            caster.GetComponent<HealthComponent>().Heal(caster, healing);
        }

        private void MaybeApplyOnCritEffect(GameObject caster, GameObject target, Damage damage)
        {
            if (!damage.IsCritical() || this.Data.OnCritEffectId == 0)
            {
                return;
            }

            var effect = this.EffectRepository.Find(this.Data.OnCritEffectId);
            effect.Skill = Skill;
            effect.Origin = Origin;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }

        protected virtual DamageType GetDamageType()
        {
            return this.Data.DamageType;
        }

        private WeaponSound GetWeaponSound()
        {
            if (this.Data.WeaponSound != WeaponSound.Weapon)
            {
                return this.Data.WeaponSound;
            }

            var weapon = Skill.Caster.GetComponent<EquipmentComponent>().GetPrimaryOrSecondaryWeapon();

            if (weapon == null || weapon.IsEmpty)
            {
                return WeaponSound.None;
            }

            switch (weapon.Type.Type)
            {
                case ItemTypeType.OneHandedAxe:
                case ItemTypeType.TwoHandedAxe:
                    return WeaponSound.Axe;

                case ItemTypeType.OneHandedMace:
                case ItemTypeType.TwoHandedMace:
                case ItemTypeType.MagicStaff:
                case ItemTypeType.CombatStaff:
                case ItemTypeType.Wand:
                    return WeaponSound.Mace;

                case ItemTypeType.OneHandedSword:
                case ItemTypeType.Claws:
                case ItemTypeType.Scythe:
                case ItemTypeType.Sickle:
                case ItemTypeType.TwoHandedSword:
                    return WeaponSound.Sword;

                case ItemTypeType.Bow:
                case ItemTypeType.Crossbow:
                    return WeaponSound.Arrow;

                case ItemTypeType.Knuckles:
                    return WeaponSound.Fist;

                case ItemTypeType.Dagger:
                case ItemTypeType.Katar:
                case ItemTypeType.TwoHandedSpear:
                case ItemTypeType.OneHandedSpear:
                    return WeaponSound.Dagger;

                case ItemTypeType.Shield:
                    return WeaponSound.Shield;
            }

            return WeaponSound.None;
        }

        public float GetAmountVariance(GameObject caster, GameObject target)
        {
            var damage = GetAmount(caster, target);

            if (this.Data.Variance > 0)
            {
                var min = (int) (Math.Max(1, damage * (1 - this.Data.Variance / 2)));
                var max = (int) (damage * (1 + this.Data.Variance / 2));

                damage = RNG.Range(min, max);
            }

            return damage;
        }

        protected virtual float Modify(float amount, GameObject caster, GameObject target)
        {
            return amount;
        }

        private float GetAmount(GameObject caster, GameObject target)
        {
            var damage = this.Data.Base + EvaluateFormula(caster, target);

            if (Skill?.IsOffhandWeapon() == true)
            {
                damage *= 0.5f + caster.GetComponent<PropertiesComponent>().Get(PropertyType.OffhandDamageIncrease).Value();
            }

            return damage;
        }

        private string Wrap(string value)
        {
            var wrapped = value;

            switch (GetDamageType())
            {
                case DamageType.Crushing:
                case DamageType.Slashing:
                case DamageType.Piercing:
                    wrapped = $"<color=#C79C6E>{value}</color>";
                    break;
                case DamageType.Fire:
                    wrapped =  $"<color=#FF7D0A>{value}</color>";
                    break;
                case DamageType.Cold:
                    wrapped =  $"<color=#A5F2F3>{value}</color>";
                    break;
                case DamageType.Holy:
                    wrapped =  $"<color=#FFFF00>{value}</color>";
                    break;
                case DamageType.Shadow:
                    wrapped =  $"<color=#8270CA>{value}</color>";
                    break;
                case DamageType.Arcane:
                    wrapped =  $"<color=#B251D1>{value}</color>";
                    break;
                case DamageType.Lightning:
                    wrapped =  $"<color=#0892D0>{value}</color>";
                    break;
                case DamageType.Poison:
                    wrapped =  $"<color=#ABD473>{value}</color>";
                    break;
                case DamageType.Chaos:
                    wrapped =  $"<color=#C41F3B>{value}</color>";
                    break;
                case DamageType.Health:
                    wrapped =  $"<color=#FF0000>{value}</color>";
                    break;
            }

            if (SettingsManager.Instance.DisplayFormulasInTooltips)
            {
                wrapped += $" <color=#888888>({this.Data.Formula})</color>";
            }

            return wrapped;
        }

        public string GetAmountString(GameObject entity)
        {
            var amount = GetAmount(entity, null);

            var multiplier = entity.GetComponent<OffenseComponent>().GetDamageIncreaseMultiplier(
                new Damage(amount, GetDamageType(), this.Data.DamageFlags, DamageInfoFlags.None));

            amount *= multiplier;

            if (this.Data.Variance <= 0)
            {
                return Wrap(((int) amount).ToString());
            }

            var min = (int) (Math.Max(1, amount * (1.0f - this.Data.Variance / 2)));
            var max = (int) (amount * (1.0f + this.Data.Variance / 2));

            return Wrap($"{min}-{max}");
        }
    }
}