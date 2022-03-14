using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CasterHaveStatusFlagValidator : Validator
    {
        private readonly StatusFlagsValidatorData data;

        public CasterHaveStatusFlagValidator(StatusFlagsValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var flags = caster.GetComponent<BehavioursComponent>().GetStatusFlags();

            var temp = this.data.Flags & flags;

            return this.data.Flags == StatusFlags.None || (this.data.Flags & flags) > 0;
        }
    }
}