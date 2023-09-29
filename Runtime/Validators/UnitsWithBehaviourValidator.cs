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
        private readonly BehaviourCountValidatorData m_Data;

        public UnitsWithBehaviourValidator(BehaviourCountValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var unitCount = Board.Instance.Cells
                .Select(c => c.OccupiedBy)
                .Count(e => e != null &&
                            e.IsAlive() &&
                            e.GetComponent<BehavioursComponent>().GetStackCount(m_Data.BehaviourId) > 0);

            return Comparator.Compare(unitCount, m_Data.Value, m_Data.Comparator);
        }
    }
}