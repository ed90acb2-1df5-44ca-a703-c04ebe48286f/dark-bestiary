using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Skills;
using DarkBestiary.Validators;

namespace DarkBestiary.Effects
{
    public class LaunchWeaponMissileEffect : LaunchMissileEffect
    {
        public LaunchWeaponMissileEffect(LaunchMissileEffectData data, List<ValidatorWithPurpose> validators,
            IMissileRepository missileRepository) : base(data, validators, missileRepository)
        {
        }

        protected override Effect New()
        {
            return new LaunchWeaponMissileEffect(Data, Validators, MissileRepository);
        }

        public override Missile GetMissile()
        {
            if (Skill.Type == SkillType.Weapon)
            {
                var weaponMissile = base.GetMissile();

                if (weaponMissile != null)
                {
                    return weaponMissile;
                }
            }

            var equipment = Skill.Caster.GetComponent<EquipmentComponent>();

            var weapon = equipment.GetPrimaryOrSecondaryWeapon();

            if (weapon == null || !weapon.IsRangedWeapon)
            {
                weapon = equipment.GetSecondaryOrPrimaryWeapon();

                if (weapon == null || !weapon.IsRangedWeapon)
                {
                    return base.GetMissile();
                }
            }

            var missile = weapon.WeaponSkillA?.GetMissile();

            return missile == null ? base.GetMissile() : missile;
        }
    }
}