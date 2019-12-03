using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnContactBehaviour : Behaviour
    {
        private readonly Effect effect;

        public OnContactBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<Validator> validators) : base(data, validators)
        {
            this.effect = effectRepository.FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BoardCell.AnyEntityEnterCell += OnAnyEntityEnterCell;
            DragEffect.AnyDragEffectFinished += OnAnyDragEffectFinished;

            var cell = BoardNavigator.Instance.NearestCell(target.transform.position);

            if (cell.IsOccupied)
            {
                OnAnyEntityEnterCell(cell.OccupiedBy, cell);
            }
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BoardCell.AnyEntityEnterCell -= OnAnyEntityEnterCell;
            DragEffect.AnyDragEffectFinished -= OnAnyDragEffectFinished;
        }

        private void OnAnyDragEffectFinished(DragEffect effect)
        {
            OnAnyEntityEnterCell(effect.Caster, BoardNavigator.Instance.NearestCell(effect.Caster.transform.position));
        }

        private void OnAnyEntityEnterCell(GameObject entity, BoardCell cell)
        {
            if (!Target.IsAlive())
            {
                return;
            }

            if (Target == entity || !cell.GameObjectsInside.Contains(Target) || !entity.IsUnit() || entity.IsDummy())
            {
                return;
            }

            MaybeApplyEffect(entity);
        }

        private void MaybeApplyEffect(GameObject entity)
        {
            if (entity.IsAirborne() || this.Validators.Any(v => !v.Validate(Caster, entity)))
            {
                return;
            }

            this.effect.Clone().Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : entity);
        }
    }
}