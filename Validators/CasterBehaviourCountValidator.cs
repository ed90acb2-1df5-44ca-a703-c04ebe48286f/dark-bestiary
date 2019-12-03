using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CasterBehaviourCountValidator : Validator
    {
        private readonly BehaviourCountValidatorData data;

        public CasterBehaviourCountValidator(BehaviourCountValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var behaviours = caster.GetComponent<BehavioursComponent>();

            return Comparator.Compare(
                behaviours.Behaviours.Where(b => b.Id == this.data.BehaviourId).Sum(b => b.StackCount),
                this.data.Value,
                this.data.Comparator
            );
        }
    }
}