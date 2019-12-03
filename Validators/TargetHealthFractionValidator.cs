using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHealthFractionValidator : Validator
    {
        private readonly TargetHealthFractionValidatorData data;

        public TargetHealthFractionValidator(TargetHealthFractionValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            var health = targetAsGameObject.GetComponent<HealthComponent>();

            return health != null && Comparator.Compare(health.HealthFraction, this.data.Fraction, this.data.Comparator);
        }
    }
}