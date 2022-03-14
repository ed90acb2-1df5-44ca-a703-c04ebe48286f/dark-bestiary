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
    public class AuraBehaviour : Behaviour
    {
        private readonly List<GameObject> affectedEntities = new List<GameObject>();
        private readonly AuraBehaviourData data;
        private readonly IBehaviourRepository behaviourRepository;

        public AuraBehaviour(AuraBehaviourData data, IBehaviourRepository behaviourRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.behaviourRepository = behaviourRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BoardCell.AnyCellOccupied += OnAnyCellOccupied;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            target.GetComponent<UnitComponent>().OwnerChanged += OnOwnerChanged;

            Detect();
        }

        protected override void OnRemove(GameObject caster, GameObject target)
        {
            BoardCell.AnyCellOccupied -= OnAnyCellOccupied;
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            target.GetComponent<UnitComponent>().OwnerChanged -= OnOwnerChanged;

            foreach (var affectedEntity in this.affectedEntities)
            {
                if (affectedEntity == null)
                {
                    continue;
                }

                affectedEntity.GetComponent<BehavioursComponent>().RemoveStack(this.data.BehaviourId);
            }

            this.affectedEntities.Clear();
        }

        protected override void OnStackCountChanged(Behaviour behaviour, int delta)
        {
            foreach (var affectedEntity in this.affectedEntities.ToList())
            {
                if (affectedEntity == null)
                {
                    continue;
                }

                var behaviours = affectedEntity.GetComponent<BehavioursComponent>();
                var stackCount = behaviours.GetStackCount(this.data.BehaviourId);

                behaviours.SetStackCount(this.data.BehaviourId, stackCount + delta);
            }
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            // Note: Workaround for situation, when other unit with same aura dies.

            if (data.Victim == Target)
            {
                return;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                if (!IsApplied)
                {
                    return;
                }

                foreach (var affectedEntity in this.affectedEntities.ToList())
                {
                    if (affectedEntity.GetComponent<BehavioursComponent>().Behaviours.Any(b => b.Id == this.data.BehaviourId))
                    {
                        continue;
                    }

                    this.affectedEntities.Remove(affectedEntity);
                }

                Detect();
            });
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
                .Where(entity => this.Validators.ByPurpose(ValidatorPurpose.Other).Validate(Target, entity))
                .ToList();

            foreach (var entityInRange in entitiesInRange)
            {
                if (this.affectedEntities.Contains(entityInRange))
                {
                    continue;
                }

                var behaviour = this.behaviourRepository.FindOrFail(this.data.BehaviourId);
                behaviour.StackCount = StackCount;

                entityInRange.GetComponent<BehavioursComponent>().ApplyAllStacks(behaviour, Target);

                this.affectedEntities.Add(entityInRange);
            }

            foreach (var affectedEntity in this.affectedEntities.ToList())
            {
                if (entitiesInRange.Contains(affectedEntity))
                {
                    continue;
                }

                if (affectedEntity != null)
                {
                    affectedEntity.GetComponent<BehavioursComponent>().RemoveStack(this.data.BehaviourId, StackCount);
                }

                this.affectedEntities.Remove(affectedEntity);
            }
        }
    }
}