using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
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

        public DamageEffect
        (
            DamageEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository
        ) : base(data, validators)
        {
            Data = data;
            EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new DamageEffect(Data, Validators, EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var amount = GetAmount(caster, target) * StackCount * DamageMultiplier;

            if (Data.Effects.Count > 0)
            {
                amount += EffectRepository.Find(Data.Effects)
                    .OfType<DamageEffect>()
                    .Aggregate(0f, (current, effect) => current + effect.GetAmount(caster, target));
            }

            var damageType = GetDamageType();

            if (MaybeHealInstead(caster, target, amount, damageType))
            {
                TriggerFinished();
                return;
            }

            var damage = new Damage(amount, damageType, GetWeaponSound(), Data.DamageFlags, Data.DamageInfoFlags, Skill);
            damage = target.GetComponent<HealthComponent>().Damage(caster, damage);

            if (target.IsEnemyOf(caster) && (damage.IsDodged() || damage.IsInvulnerable()))
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
            if (caster.GetComponent<BehavioursComponent>().IsCausticPoison)
            {
                return false;
            }

            if (target.GetComponent<BehavioursComponent>().IsUndead && damageType == DamageType.Poison)
            {
                target.GetComponent<HealthComponent>().Heal(caster, new Healing(amount, HealFlags.None, Skill));
                return true;
            }

            return false;
        }

        private void MaybeApplyVampirism(GameObject caster, Damage damage)
        {
            if (damage.Amount < 1 || Mathf.Approximately(Data.Vampirism, 0))
            {
                return;
            }

            var healing = new Healing(damage * Data.Vampirism, HealFlags.Vampirism, Skill);

            caster.GetComponent<HealthComponent>().Heal(caster, healing);
        }

        private void MaybeApplyOnCritEffect(GameObject caster, GameObject target, Damage damage)
        {
            if (!damage.IsCritical() || Data.OnCritEffectId == 0)
            {
                return;
            }

            var effect = EffectRepository.Find(Data.OnCritEffectId);
            effect.Skill = Skill;
            effect.Origin = Origin;
            effect.StackCount = StackCount;
            effect.Apply(caster, target);
        }

        protected virtual DamageType GetDamageType()
        {
            return Data.DamageType;
        }

        private WeaponSound GetWeaponSound()
        {
            if (Data.WeaponSound != WeaponSound.Weapon)
            {
                return Data.WeaponSound;
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

        public float GetAmount(GameObject caster, GameObject target)
        {
            var damage = Modify(GetAmountBase(caster, target), caster, target);

            if (Data.Variance > 0)
            {
                var min = (int) (Math.Max(1, damage * (1 - Data.Variance / 2)));
                var max = (int) (damage * (1 + Data.Variance / 2));

                damage = Rng.Range(min, max);
            }

            return damage;
        }

        private float GetAmountBase(GameObject caster, GameObject target)
        {
            var damage = Data.Base + EvaluateFormula(caster, target);

            if (Skill?.IsOffhandWeapon() == true)
            {
                damage *= 0.5f + caster.GetComponent<PropertiesComponent>().Get(PropertyType.OffhandDamageIncrease).Value();
            }

            if (Skill?.Weapon != null && Skill.Weapon.IsWeapon)
            {
                var rarityIndex = (int) Skill.Weapon.Rarity.Type;
                damage *= 1 + Mathf.Max(rarityIndex - 1, 0) * 0.1f;
            }

            return damage;
        }

        protected virtual float Modify(float amount, GameObject caster, GameObject target)
        {
            return amount;
        }

        public string GetAmountString(GameObject entity)
        {
            var amount = GetAmountBase(entity, null);

            var multiplier = entity.GetComponent<OffenseComponent>().GetDamageMultiplier(
                new Damage(amount, GetDamageType(), WeaponSound.None, Data.DamageFlags, DamageInfoFlags.None, null)
            );

            amount *= multiplier;

            if (Data.Variance <= 0 || amount < 1)
            {
                return Wrap(((int) amount).ToString());
            }

            var min = (int) (Math.Max(1, amount * (1.0f - Data.Variance / 2)));
            var max = (int) (amount * (1.0f + Data.Variance / 2));

            return Wrap($"{min.ToString()}-{max.ToString()}");
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
                    wrapped = $"<color=#FF7D0A>{value}</color>";
                    break;
                case DamageType.Cold:
                    wrapped = $"<color=#A5F2F3>{value}</color>";
                    break;
                case DamageType.Holy:
                    wrapped = $"<color=#FFFF00>{value}</color>";
                    break;
                case DamageType.Shadow:
                    wrapped = $"<color=#8270CA>{value}</color>";
                    break;
                case DamageType.Arcane:
                    wrapped = $"<color=#B251D1>{value}</color>";
                    break;
                case DamageType.Lightning:
                    wrapped = $"<color=#0892D0>{value}</color>";
                    break;
                case DamageType.Poison:
                    wrapped = $"<color=#ABD473>{value}</color>";
                    break;
                case DamageType.Chaos:
                    wrapped = $"<color=#C41F3B>{value}</color>";
                    break;
                case DamageType.Health:
                    wrapped = $"<color=#FF0000>{value}</color>";
                    break;
            }

            if (SettingsManager.Instance.DisplayFormulasInTooltips)
            {
                wrapped = $"{wrapped} <color=#888888>({Data.Formula})</color>";
            }

            return wrapped;
        }
    }
}