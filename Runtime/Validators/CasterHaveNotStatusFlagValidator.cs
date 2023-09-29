using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CasterHaveNotStatusFlagValidator : Validator
    {
        private readonly StatusFlagsValidatorData m_Data;

        public CasterHaveNotStatusFlagValidator(StatusFlagsValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var flags = caster.GetComponent<BehavioursComponent>().GetStatusFlags();

            return m_Data.Flags == StatusFlags.None || (m_Data.Flags & flags) == 0;
        }
    }
}