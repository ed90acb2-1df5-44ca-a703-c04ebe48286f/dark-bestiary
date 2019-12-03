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
        private readonly SuckInEffectData data;
        private readonly List<Validator> validators;

        public SuckInEffect(SuckInEffectData data, List<Validator> validators) : base(data, new List<Validator>())
        {
            this.data = data;
            this.validators = validators;
        }

        protected override Effect New()
        {
            return new SuckInEffect(this.data, this.validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var cells = BoardNavigator.Instance.WithinCircle(target, this.data.Radius)
                .Where(cell => cell.IsWalkable)
                .OrderBy(cell => (cell.transform.position - target).sqrMagnitude)
                .ToList();

            var entities = cells.ToEntities()
                .Where(entity => this.validators.All(validator => validator.Validate(caster, entity)))
                .Where(entity => !entity.IsImmovable())
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
                Timer.Instance.StartCoroutine(Move(destination.Key, destination.Value, this.data.Duration));
            }

            Timer.Instance.Wait(this.data.Duration, TriggerFinished);
        }

        private IEnumerator Move(GameObject entity, BoardCell destination, float duration)
        {
            var actor = entity.GetComponent<ActorComponent>();

            if (!string.IsNullOrEmpty(this.data.Animation))
            {
                actor.Model.LookAt(destination.transform.position);
                actor.PlayAnimation(this.data.Animation);
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

            destination.IsReserved = false;

            if (!string.IsNullOrEmpty(this.data.Animation))
            {
                actor.PlayAnimation("idle");
            }
        }
    }
}