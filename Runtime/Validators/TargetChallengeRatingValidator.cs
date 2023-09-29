using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetChallengeRatingValidator : Validator
    {
        private readonly ValueValidatorData m_Data;

        public TargetChallengeRatingValidator(ValueValidatorData data)
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

            var unit = targetAsGameObject.GetComponent<UnitComponent>();

            if (unit == null)
            {
                return false;
            }

            return Comparator.Compare(unit.ChallengeRating, m_Data.Value, m_Data.Comparator);
        }
    }
}