using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHaveStatusFlagValidator : Validator
    {
        private readonly StatusFlagsValidatorData m_Data;

        public TargetHaveStatusFlagValidator(StatusFlagsValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject gameObject))
            {
                return false;
            }

            var flags = gameObject.GetComponent<BehavioursComponent>().GetStatusFlags();

            return m_Data.Flags == StatusFlags.None || (m_Data.Flags & flags) > 0;
        }
    }
}