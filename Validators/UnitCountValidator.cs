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
        private readonly UnitCountValidatorData data;

        public UnitCountValidator(UnitCountValidatorData data)
        {
            this.data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var unitCount = Board.Instance.Cells.Select(c => c.OccupiedBy)
                .Count(e => e != null && e.IsAlive() && e.GetComponent<UnitComponent>().Id == this.data.UnitId);

            return Comparator.Compare(unitCount, this.data.Value, this.data.Comparator);
        }
    }
}