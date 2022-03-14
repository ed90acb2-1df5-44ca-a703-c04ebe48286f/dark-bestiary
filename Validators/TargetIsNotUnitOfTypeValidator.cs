using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetIsNotUnitOfTypeValidator : Validator
    {
        private readonly UnitValidatorData data;

        public TargetIsNotUnitOfTypeValidator(UnitValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject targetGameObject))
            {
                return false;
            }

            return targetGameObject.GetComponent<UnitComponent>().Id != this.data.UnitId;
        }
    }
}