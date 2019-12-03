using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;

namespace DarkBestiary.Effects
{
    public class WeaponDamageEffect : DamageEffect
    {
        public WeaponDamageEffect(DamageEffectData data, List<Validator> validators,
            IEffectRepository effectRepository) : base(data, validators, effectRepository)
        {
        }

        protected override Effect New()
        {
            return new WeaponDamageEffect(this.Data, this.Validators, this.EffectRepository);
        }

        protected override DamageType GetDamageType()
        {
            var weapon = Skill.Caster.GetComponent<EquipmentComponent>().GetPrimaryOrSecondaryWeapon();

            if (weapon == null || weapon.IsEmpty)
            {
                return DamageType.Crushing;
            }

            if (weapon.IsSlashingMeleeWeapon)
            {
                return DamageType.Slashing;
            }

            return weapon.IsPiercingMeleeWeapon ? DamageType.Piercing : DamageType.Crushing;
        }
    }
}