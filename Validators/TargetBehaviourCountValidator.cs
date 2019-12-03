using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetBehaviourCountValidator : Validator
    {
        private readonly BehaviourCountValidatorData data;

        public TargetBehaviourCountValidator(BehaviourCountValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            var behavioursComponent = targetAsGameObject.GetComponent<BehavioursComponent>();

            if (behavioursComponent == null)
            {
                return false;
            }

            return Comparator.Compare(
                behavioursComponent.Behaviours.Where(b => b.Id == this.data.BehaviourId).Sum(b => b.StackCount),
                this.data.Value,
                this.data.Comparator
            );
        }
    }
}