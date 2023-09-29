using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHealthFractionValidator : Validator
    {
        private readonly TargetHealthFractionValidatorData m_Data;

        public TargetHealthFractionValidator(TargetHealthFractionValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            var health = targetAsGameObject.GetComponent<HealthComponent>();

            return health != null && Comparator.Compare(health.HealthFraction, m_Data.Fraction, m_Data.Comparator);
        }
    }
}