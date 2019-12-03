using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnExitCellBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnExitCellBehaviour(EffectBehaviourData data, List<Validator> validators) : base(data, validators)
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
            if (!Target.Equals(entity))
            {
                return;
            }

            if (EventSubject == BehaviourEventSubject.Me)
            {
                this.effect.Clone().Apply(Caster, Target);
            }
            else
            {
                this.effect.Clone().Apply(Caster, cell.transform.position);
            }
        }
    }
}