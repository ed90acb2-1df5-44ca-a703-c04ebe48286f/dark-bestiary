using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHasNoSummonedComponent : Validator
    {
        public TargetHasNoSummonedComponent(EmptyValidatorData data)
        {
        }

        public override bool Validate(GameObject caster, object target)
        {
            var entity = target as GameObject;

            return entity?.GetComponent<SummonedComponent>() == null;
        }
    }
}