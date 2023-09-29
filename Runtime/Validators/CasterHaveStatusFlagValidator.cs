using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CasterHaveStatusFlagValidator : Validator
    {
        private readonly StatusFlagsValidatorData m_Data;

        public CasterHaveStatusFlagValidator(StatusFlagsValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var flags = caster.GetComponent<BehavioursComponent>().GetStatusFlags();

            var temp = m_Data.Flags & flags;

            return m_Data.Flags == StatusFlags.None || (m_Data.Flags & flags) > 0;
        }
    }
}