using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetUnitHaveFlagsValidator : Validator
    {
        private readonly UnitFlagsValidatorData data;

        public TargetUnitHaveFlagsValidator(UnitFlagsValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var gameObject = target as GameObject;

            if (gameObject == null)
            {
                return false;
            }

            var flags = gameObject.GetComponent<UnitComponent>().Flags;

            return this.data.Flags == UnitFlags.None || (this.data.Flags & flags) > 0;
        }
    }
}