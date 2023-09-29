using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetIsVisibleValidator : Validator
    {
        public TargetIsVisibleValidator(ValidatorData data)
        {
        }

        public override bool Validate(GameObject caster, object target)
        {
            if (target is GameObject gameObject)
            {
                return gameObject.GetComponent<ActorComponent>().IsVisible;
            }

            return false;
        }
    }
}