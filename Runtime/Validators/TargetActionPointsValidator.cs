using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetActionPointsValidator : Validator
    {
        private readonly ValueValidatorData m_Data;

        public TargetActionPointsValidator(ValueValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject gameObject))
            {
                return false;
            }

            var actionPoints = gameObject.GetComponent<ResourcesComponent>().Get(ResourceType.ActionPoint).Amount;

            return Comparator.Compare(actionPoints, m_Data.Value, m_Data.Comparator);
        }
    }
}