using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetUnitHaveNotFlagsValidator : Validator
    {
        private readonly UnitFlagsValidatorData m_Data;

        public TargetUnitHaveNotFlagsValidator(UnitFlagsValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var gameObject = target as GameObject;

            if (gameObject == null)
            {
                return false;
            }

            var flags = gameObject.GetComponent<UnitComponent>().Flags;

            return m_Data.Flags == UnitFlags.None || (m_Data.Flags & flags) == 0;
        }
    }
}