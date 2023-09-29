using DarkBestiary.Data;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetInRangeValidator : Validator
    {
        private readonly InRangeValidatorData m_Data;

        public TargetInRangeValidator(InRangeValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            return (caster.transform.position - target.GetPosition()).magnitude.InRange(m_Data.Min, m_Data.Max);
        }
    }
}