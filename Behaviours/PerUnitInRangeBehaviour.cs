using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerUnitInRangeBehaviour : Behaviour
    {
        private readonly AuraBehaviourData data;
        private readonly Behaviour behaviour;

        private BehavioursComponent behaviours;

        public PerUnitInRangeBehaviour(AuraBehaviourData data, IBehaviourRepository behaviourRepository,
            List<Validator> validators) : base(data, validators)
        {
            this.data = data;
            this.behaviour = behaviourRepository.FindOrFail(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BoardCell.AnyCellOccupied += OnAnyCellOccupied;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            target.GetComponent<UnitComponent>().OwnerChanged += OnOwnerChanged;

            this.behaviours = target.GetComponent<BehavioursComponent>();

            Detect();
        }

        protected override void OnRemove(GameObject caster, GameObject target)
        {
            BoardCell.AnyCellOccupied -= OnAnyCellOccupied;
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            target.GetComponent<UnitComponent>().OwnerChanged -= OnOwnerChanged;

            if (this.behaviour.IsApplied)
            {
                this.behaviours.RemoveAllStacks(this.behaviour.Id);
            }
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            Detect();
        }

        private void OnOwnerChanged(UnitComponent unit)
        {
            Detect();
        }

        private void OnAnyCellOccupied(BoardCell cell)
        {
            if (!Target.IsAlive())
            {
                return;
            }

            Detect();
        }

        private void Detect()
        {
            var entitiesInRange = BoardNavigator.Instance.WithinCircle(Target.transform.position, this.data.Range)
                .ToEntities()
                .Where(entity => entity != Target && entity.IsAlive() && this.Validators.All(validator => validator.Validate(Target, entity)))
                .ToList();

            if (entitiesInRange.Count == 0)
            {
                this.behaviours.RemoveAllStacks(this.behaviour.Id);
                return;
            }

            if (!this.behaviour.IsApplied)
            {
                this.behaviours.Apply(this.behaviour, Target);
            }

            this.behaviours.SetStackCount(this.behaviour.Id, entitiesInRange.Count);
        }
    }
}