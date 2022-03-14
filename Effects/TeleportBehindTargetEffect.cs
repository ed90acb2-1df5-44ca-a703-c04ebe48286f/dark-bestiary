using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class TeleportBehindTargetEffect : Effect
    {
        private readonly EmptyEffectData data;

        public TeleportBehindTargetEffect(EmptyEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new TeleportBehindTargetEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var cell = BoardNavigator.Instance.BehindTheBackOfOccupying(target.transform.position);

            if (cell == null || cell.IsOccupied)
            {
                cell = BoardNavigator.Instance.WithinCircle(target.transform.position, 5)
                    .Where(c => c.IsWalkable && !c.IsOccupied)
                    .OrderBy(c => (c.transform.position - target.transform.position).sqrMagnitude)
                    .FirstOrDefault();
            }

            if (cell == null)
            {
                TriggerFinished();
                return;
            }

            caster.transform.position = cell.transform.position;
            caster.GetComponent<ActorComponent>().Model.LookAt(target.transform.position);
            TriggerFinished();
        }
    }
}