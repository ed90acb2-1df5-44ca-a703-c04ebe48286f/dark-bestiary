using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CasterHaveNotStatusFlagValidator : Validator
    {
        private readonly StatusFlagsValidatorData data;

        public CasterHaveNotStatusFlagValidator(StatusFlagsValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var flags = caster.GetComponent<BehavioursComponent>().GetStatusFlags();

            return this.data.Flags == StatusFlags.None || (this.data.Flags & flags) == 0;
        }
    }
}