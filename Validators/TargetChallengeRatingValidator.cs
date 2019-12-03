using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetChallengeRatingValidator : Validator
    {
        private readonly ValueValidatorData data;

        public TargetChallengeRatingValidator(ValueValidatorData data)
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

            var unit = targetAsGameObject.GetComponent<UnitComponent>();

            if (unit == null)
            {
                return false;
            }

            return Comparator.Compare(unit.ChallengeRating, this.data.Value, this.data.Comparator);
        }
    }
}