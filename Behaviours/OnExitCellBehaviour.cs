using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnExitCellBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnExitCellBehaviour(EffectBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BoardCell.AnyEntityExitCell += OnEntityExitCell;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BoardCell.AnyEntityExitCell -= OnEntityExitCell;
        }

        private void OnEntityExitCell(GameObject entity, BoardCell cell)
        {
            if (!Target.Equals(entity) || Target.IsMovingViaScript())
            {
                return;
            }

            var clone = this.effect.Clone();
            clone.StackCount = StackCount;

            if (EventSubject == BehaviourEventSubject.Me)
            {
                clone.Apply(Caster, Target);
            }
            else
            {
                clone.Apply(Caster, cell.transform.position);
            }
        }
    }
}