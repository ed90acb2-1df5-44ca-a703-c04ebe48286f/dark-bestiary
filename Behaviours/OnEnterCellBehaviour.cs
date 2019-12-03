using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnEnterCellBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnEnterCellBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<Validator> validators) : base(data, validators)
        {
            this.effect = effectRepository.FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BoardCell.AnyEntityEnterCell += OnEntityEnterCell;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BoardCell.AnyEntityEnterCell -= OnEntityEnterCell;
        }

        private void OnEntityEnterCell(GameObject entity, BoardCell cell)
        {
            if (!Target.Equals(entity))
            {
                return;
            }

            this.effect.Clone().Apply(Caster, Target);
        }
    }
}