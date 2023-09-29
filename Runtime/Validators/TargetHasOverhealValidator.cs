using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHasOverhealValidator : Validator
    {
        public TargetHasOverhealValidator(ValidatorData data)
        {
        }

        public override bool Validate(GameObject caster, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            return targetAsGameObject.GetComponent<HealthComponent>().LastOverheal >= 1;
        }
    }
}