using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class PerUnitInRangeBehaviour : Behaviour
    {
        private readonly AuraBehaviourData m_Data;
        private readonly Behaviour m_Behaviour;

        private BehavioursComponent m_Behaviours;

        public PerUnitInRangeBehaviour(AuraBehaviourData data, IBehaviourRepository behaviourRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Behaviour = behaviourRepository.FindOrFail(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BoardCell.AnyCellOccupied += OnAnyCellOccupied;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            target.GetComponent<UnitComponent>().OwnerChanged += OnOwnerChanged;

            m_Behaviours = target.GetComponent<BehavioursComponent>();

            Detect();
        }

        protected override void OnRemove(GameObject caster, GameObject target)
        {
            BoardCell.AnyCellOccupied -= OnAnyCellOccupied;
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            target.GetComponent<UnitComponent>().OwnerChanged -= OnOwnerChanged;

            if (m_Behaviour.IsApplied)
            {
                m_Behaviours.RemoveAllStacks(m_Behaviour.Id);
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
            var entitiesInRange = BoardNavigator.Instance.WithinCircle(Target.transform.position, m_Data.Range)
                .ToEntities()
                .Where(entity => entity != Target &&
                                 entity.IsAlive() &&
                                 Validators.ByPurpose(ValidatorPurpose.Other).Validate(Target, entity))
                .ToList();

            if (entitiesInRange.Count == 0)
            {
                m_Behaviours.RemoveAllStacks(m_Behaviour.Id);
                return;
            }

            if (!m_Behaviour.IsApplied)
            {
                m_Behaviours.ApplyAllStacks(m_Behaviour, Target);
            }

            m_Behaviours.SetStackCount(m_Behaviour.Id, entitiesInRange.Count);
        }
    }
}