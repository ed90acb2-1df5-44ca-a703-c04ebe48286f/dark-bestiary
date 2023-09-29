using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class SuckInEffect : Effect
    {
        private readonly SuckInEffectData m_Data;

        public SuckInEffect(SuckInEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new SuckInEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var cells = BoardNavigator.Instance.WithinCircle(target, m_Data.Radius)
                .Where(cell => cell.IsWalkable)
                .OrderBy(cell => (cell.transform.position - target).sqrMagnitude)
                .ToList();

            var entities = cells.ToEntities()
                .Where(entity => Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, entity))
                .Where(entity =>
                    {
                        var unit = entity.GetComponent<UnitComponent>();
                        return !unit.IsImmovable && !unit.IsMovingViaScript;
                    }
                )
                .ToList();

            if (entities.Count == 0)
            {
                TriggerFinished();
                return;
            }

            var destinations = new Dictionary<GameObject, BoardCell>();

            foreach (var entity in entities)
            {
                var destination = cells
                    .Where(cell => !cell.IsOccupied && !cell.IsReserved && cell.OccupiedBy != entity)
                    .OrderBy(cell => (cell.transform.position - target).sqrMagnitude)
                    .ThenBy(cell => (cell.transform.position - entity.transform.position).sqrMagnitude)
                    .FirstOrDefault();

                if (destination == null)
                {
                    continue;
                }

                destination.IsReserved = true;
                destinations.Add(entity, destination);
            }

            foreach (var destination in destinations)
            {
                Timer.Instance.StartCoroutine(Move(destination.Key, destination.Value, m_Data.Duration));
            }

            Timer.Instance.Wait(m_Data.Duration, TriggerFinished);
        }

        private IEnumerator Move(GameObject entity, BoardCell destination, float duration)
        {
            var actor = entity.GetComponent<ActorComponent>();
            var unit = entity.GetComponent<UnitComponent>();

            unit.Flags |= UnitFlags.MovingViaScript;

            if (!string.IsNullOrEmpty(m_Data.Animation))
            {
                actor.Model.LookAt(destination.transform.position);
                actor.PlayAnimation(m_Data.Animation);
            }

            var time = 0f;

            while (time < duration)
            {
                time = Mathf.Min(time + Time.deltaTime, duration);

                entity.transform.position = Vector3.Lerp(
                    entity.transform.position, destination.transform.position, Mathf.Pow(time / duration, 1.5f));

                if ((entity.transform.position - destination.transform.position).magnitude < 0.05f)
                {
                    time = duration;
                }

                yield return null;
            }

            unit.Flags &= ~UnitFlags.MovingViaScript;

            entity.transform.position = destination.transform.position;

            destination.OnEnter(entity);
            destination.IsReserved = false;

            if (!string.IsNullOrEmpty(m_Data.Animation))
            {
                actor.PlayAnimation("idle");
            }
        }
    }
}