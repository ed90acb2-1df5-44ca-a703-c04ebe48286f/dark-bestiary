using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class SearchCorpsesEffect : Effect
    {
        private readonly Effect effect;
        private readonly SearchAreaEffectData data;
        private readonly List<ValidatorWithPurpose> validators;
        private readonly IEffectRepository effectRepository;

        public SearchCorpsesEffect(SearchAreaEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            this.data = data;
            this.validators = validators;
            this.effectRepository = effectRepository;
            this.effect = effectRepository.Find(data.EffectId);
        }

        protected override Effect New()
        {
            return new SearchCorpsesEffect(this.data, this.validators, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var origin = OriginPosition();

            // TODO: Separate shapes from lines/cones
            List<BoardCell> cells;

            switch (this.data.Shape)
            {
                case Shape.Circle:
                    cells = BoardNavigator.Instance.WithinCircle(target, this.data.Radius);
                    break;
                case Shape.Cross:
                    cells = BoardNavigator.Instance.WithinCross(target, this.data.Radius);
                    break;
                default:
                    cells = BoardNavigator.Instance.WithinShape(origin, target, this.data.Shape, this.data.Radius);
                    break;
            }

            var corpses = cells
                .OrderBy(c => (c.transform.position - target).sqrMagnitude)
                .Select(c => c.GameObjectsInside.Corpses().FirstOrDefault())
                .NotNull()
                .ToList();

            for (var index = 0; index < corpses.Count; index++)
            {
                if (this.data.Limit != -1 && index >= this.data.Limit)
                {
                    break;
                }

                var clone = this.effect.Clone();
                clone.Skill = Skill;
                clone.DamageMultiplier = DamageMultiplier;
                clone.Origin = Origin;
                clone.StackCount = StackCount;
                clone.Apply(caster, corpses[index].transform.position);

                corpses[index].Consume();
            }

            TriggerFinished();
        }
    }
}