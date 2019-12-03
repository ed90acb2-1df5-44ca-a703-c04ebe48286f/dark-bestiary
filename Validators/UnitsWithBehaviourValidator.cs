using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class UnitsWithBehaviourValidator : Validator
    {
        private readonly BehaviourCountValidatorData data;

        public UnitsWithBehaviourValidator(BehaviourCountValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var unitCount = Board.Instance.Cells
                .Select(c => c.OccupiedBy)
                .Count(e => e != null &&
                            e.IsAlive() &&
                            e.GetComponent<BehavioursComponent>().GetStackCount(this.data.BehaviourId) > 0);

            return Comparator.Compare(unitCount, this.data.Value, this.data.Comparator);
        }
    }
}