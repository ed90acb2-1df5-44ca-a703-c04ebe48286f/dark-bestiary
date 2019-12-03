using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHaveNotStatusFlagValidator : Validator
    {
        private readonly StatusFlagsValidatorData data;

        public TargetHaveNotStatusFlagValidator(StatusFlagsValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (!(target is GameObject gameObject))
            {
                return false;
            }

            var flags = gameObject.GetComponent<BehavioursComponent>().GetStatusFlags();

            return this.data.Flags == StatusFlags.None || (this.data.Flags & flags) == 0;
        }
    }
}