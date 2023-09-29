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
    public class OnEnterCellBehaviour : Behaviour
    {
        private readonly Effect m_Effect;

        public OnEnterCellBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Effect = effectRepository.FindOrFail(data.EffectId);
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
            if (!Target.Equals(entity) || Target.IsMovingViaScript())
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, Target);
        }
    }
}