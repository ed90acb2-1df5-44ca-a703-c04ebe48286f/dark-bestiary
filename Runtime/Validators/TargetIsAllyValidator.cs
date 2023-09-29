using DarkBestiary.Data;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetIsAllyValidator : Validator
    {
        public TargetIsAllyValidator(ValidatorData data)
        {
        }

        public override bool Validate(GameObject caster, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            return targetAsGameObject.IsAllyOf(caster);
        }
    }
}