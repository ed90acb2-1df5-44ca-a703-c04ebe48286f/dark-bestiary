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
        private readonly Effect m_Effect;
        private readonly SearchAreaEffectData m_Data;
        private readonly List<ValidatorWithPurpose> m_Validators;
        private readonly IEffectRepository m_EffectRepository;

        public SearchCorpsesEffect(SearchAreaEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_Validators = validators;
            m_EffectRepository = effectRepository;
            m_Effect = effectRepository.Find(data.EffectId);
        }

        protected override Effect New()
        {
            return new SearchCorpsesEffect(m_Data, m_Validators, m_EffectRepository);
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

            switch (m_Data.Shape)
            {
                case Shape.Circle:
                    cells = BoardNavigator.Instance.WithinCircle(target, m_Data.Radius);
                    break;
                case Shape.Cross:
                    cells = BoardNavigator.Instance.WithinCross(target, m_Data.Radius);
                    break;
                default:
                    cells = BoardNavigator.Instance.WithinShape(origin, target, m_Data.Shape, m_Data.Radius);
                    break;
            }

            var corpses = cells
                .OrderBy(c => (c.transform.position - target).sqrMagnitude)
                .Select(c => c.GameObjectsInside.Corpses().FirstOrDefault())
                .NotNull()
                .ToList();

            for (var index = 0; index < corpses.Count; index++)
            {
                if (m_Data.Limit != -1 && index >= m_Data.Limit)
                {
                    break;
                }

                var clone = m_Effect.Clone();
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