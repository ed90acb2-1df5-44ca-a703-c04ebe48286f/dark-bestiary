using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Movers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class KnockbackEffect : Effect
    {
        private readonly KnockbackEffectData data;
        private readonly IEffectRepository effectRepository;
        private readonly BoardNavigator boardNavigator;

        private Mover mover;

        public KnockbackEffect(KnockbackEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository, BoardNavigator boardNavigator) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
            this.boardNavigator = boardNavigator;
        }

        protected override Effect New()
        {
            return new KnockbackEffect(this.data, this.Validators, this.effectRepository, this.boardNavigator);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            if (target.IsImmovable() || target.GetComponent<BehavioursComponent>().IsImmobilized || target.GetComponent<UnitComponent>().IsMovingViaScript)
            {
                TriggerFinished();
                return;
            }

            var direction = (target.transform.position - OriginPosition()).normalized;
            var distance = Board.Instance.CellSize * this.data.Distance;
            var destination = target.transform.position + direction * distance;

            var unitComponent = target.GetComponent<UnitComponent>();
            unitComponent.Flags |= UnitFlags.MovingViaScript;

            this.mover = Mover.Factory(this.data.Mover);
            this.mover.Stopped += OnMoverStopped;
            this.mover.FindAnyCollisionAndMove(target, destination, distance);
            this.mover.DestinationCell.IsReserved = true;
        }

        private void OnMoverStopped()
        {
            var unitComponent = this.mover.Entity.GetComponent<UnitComponent>();
            unitComponent.Flags &= ~UnitFlags.MovingViaScript;

            this.mover.DestinationCell.OnEnter(this.mover.Entity);
            this.mover.DestinationCell.IsReserved = false;
            this.mover.Entity.transform.position = this.mover.DestinationCell.transform.position.Snapped();

            if (this.mover.CollidesWithEntity)
            {
                this.effectRepository.Find(this.data.CollideWithEntityEffectId)?.Apply(Caster, Target);
            }
            else if (this.mover.CollidesWithEnvironment)
            {
                this.effectRepository.Find(this.data.CollideWithEnvironmentEffectId)?.Apply(Caster, Target);
            }
            else
            {
                this.effectRepository.Find(this.data.FinalEffectId)?.Apply(Caster, Target);
            }

            TriggerFinished();
        }
    }
}