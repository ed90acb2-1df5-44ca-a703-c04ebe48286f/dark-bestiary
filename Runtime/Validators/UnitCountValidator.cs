using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class UnitCountValidator : Validator
    {
        private readonly UnitCountValidatorData m_Data;

        public UnitCountValidator(UnitCountValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var unitCount = Board.Instance.Cells.Select(c => c.OccupiedBy)
                .Count(e => e != null && e.IsAlive() && e.GetComponent<UnitComponent>().Id == m_Data.UnitId);

            return Comparator.Compare(unitCount, m_Data.Value, m_Data.Comparator);
        }
    }
}