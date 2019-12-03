using System.Collections.Generic;
using System.Linq;
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

        private BoardCell destination;
        private Mover mover;

        public KnockbackEffect(KnockbackEffectData data, List<Validator> validators,
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
            if (target.IsImmovable() || target.GetComponent<BehavioursComponent>().IsImmobilized)
            {
                TriggerFinished();
                return;
            }

            var direction = (target.transform.position - OriginPosition()).normalized;

            this.destination = this.boardNavigator.WithinLine(
                    target.transform.position, direction, this.data.Distance)
                .Where(cell => !cell.IsReserved)
                .OrderByDescending(cell => (target.transform.position - cell.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (this.destination == null)
            {
                TriggerFinished();
                return;
            }

            this.destination.IsReserved = true;

            this.mover = Mover.Factory(this.data.Mover);
            this.mover.Finished += OnMoverFinished;
            this.mover.CollidingWithEntity += OnMoverCollidingWithEntity;
            this.mover.CollidingWithEnvironment += OnMoverCollidingWithEnvironment;
            this.mover.Start(target, this.destination.transform.position);
        }

        private void OnMoverCollidingWithEntity(GameObject entity)
        {
            this.effectRepository.Find(this.data.CollideWithEntityEffectId)?.Apply(Caster, Target);
            this.mover.Stop();
        }

        private void OnMoverCollidingWithEnvironment()
        {
            this.effectRepository.Find(this.data.CollideWithEnvironmentEffectId)?.Apply(Caster, Target);
            this.mover.Stop();
        }

        private void OnMoverFinished()
        {
            this.destination.IsReserved = false;

            this.effectRepository.Find(this.data.FinalEffectId)?.Apply(Caster, Target);

            this.mover.Entity.transform.position = this.mover.Entity.transform.position.Snapped();

            TriggerFinished();
        }
    }
}