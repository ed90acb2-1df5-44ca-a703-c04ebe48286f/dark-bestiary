using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetIsNotUnitOfTypeValidator : Validator
    {
        private readonly UnitValidatorData m_Data;

        public TargetIsNotUnitOfTypeValidator(UnitValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject targetGameObject))
            {
                return false;
            }

            return targetGameObject.GetComponent<UnitComponent>().Id != m_Data.UnitId;
        }
    }
}