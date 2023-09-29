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
    public class AuraBehaviour : Behaviour
    {
        private readonly List<GameObject> m_AffectedEntities = new();
        private readonly AuraBehaviourData m_Data;
        private readonly IBehaviourRepository m_BehaviourRepository;

        public AuraBehaviour(AuraBehaviourData data, IBehaviourRepository behaviourRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_BehaviourRepository = behaviourRepository;
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

            foreach (var affectedEntity in m_AffectedEntities)
            {
                if (affectedEntity == null)
                {
                    continue;
                }

                affectedEntity.GetComponent<BehavioursComponent>().RemoveStack(m_Data.BehaviourId);
            }

            m_AffectedEntities.Clear();
        }

        protected override void OnStackCountChanged(Behaviour behaviour, int delta)
        {
            foreach (var affectedEntity in m_AffectedEntities.ToList())
            {
                if (affectedEntity == null)
                {
                    continue;
                }

                var behaviours = affectedEntity.GetComponent<BehavioursComponent>();
                var stackCount = behaviours.GetStackCount(m_Data.BehaviourId);

                behaviours.SetStackCount(m_Data.BehaviourId, stackCount + delta);
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

                foreach (var affectedEntity in m_AffectedEntities.ToList())
                {
                    if (affectedEntity.GetComponent<BehavioursComponent>().Behaviours.Any(b => b.Id == m_Data.BehaviourId))
                    {
                        continue;
                    }

                    m_AffectedEntities.Remove(affectedEntity);
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
            var entitiesInRange = BoardNavigator.Instance.WithinCircle(Target.transform.position, m_Data.Range)
                .ToEntities()
                .Where(entity => Validators.ByPurpose(ValidatorPurpose.Other).Validate(Target, entity))
                .ToList();

            foreach (var entityInRange in entitiesInRange)
            {
                if (m_AffectedEntities.Contains(entityInRange))
                {
                    continue;
                }

                var behaviour = m_BehaviourRepository.FindOrFail(m_Data.BehaviourId);
                behaviour.StackCount = StackCount;

                entityInRange.GetComponent<BehavioursComponent>().ApplyAllStacks(behaviour, Target);

                m_AffectedEntities.Add(entityInRange);
            }

            foreach (var affectedEntity in m_AffectedEntities.ToList())
            {
                if (entitiesInRange.Contains(affectedEntity))
                {
                    continue;
                }

                if (affectedEntity != null)
                {
                    affectedEntity.GetComponent<BehavioursComponent>().RemoveStack(m_Data.BehaviourId, StackCount);
                }

                m_AffectedEntities.Remove(affectedEntity);
            }
        }
    }
}