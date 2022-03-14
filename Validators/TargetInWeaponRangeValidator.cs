using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetInWeaponRangeValidator : Validator
    {
        public TargetInWeaponRangeValidator(EmptyValidatorData data)
        {
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject gameObject))
            {
                return false;
            }

            var attack = caster.GetComponent<SpellbookComponent>().FirstWeaponSkill();

            if (attack == null)
            {
                return false;
            }

            return attack.IsTargetInRange(gameObject);
        }
    }
}