using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetActionPointsValidator : Validator
    {
        private readonly ValueValidatorData data;

        public TargetActionPointsValidator(ValueValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject gameObject))
            {
                return false;
            }

            var actionPoints = gameObject.GetComponent<ResourcesComponent>().Get(ResourceType.ActionPoint).Amount;

            return Comparator.Compare(actionPoints, this.data.Value, this.data.Comparator);
        }
    }
}