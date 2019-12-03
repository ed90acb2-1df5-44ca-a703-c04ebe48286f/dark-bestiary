using DarkBestiary.Data;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetInRangeValidator : Validator
    {
        private readonly InRangeValidatorData data;

        public TargetInRangeValidator(InRangeValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            return (caster.transform.position - target.GetPosition()).magnitude.InRange(this.data.Min, this.data.Max);
        }
    }
}