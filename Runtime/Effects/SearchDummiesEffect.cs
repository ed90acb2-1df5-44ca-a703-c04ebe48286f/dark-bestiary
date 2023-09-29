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
    public class SearchDummiesEffect : Effect
    {
        private readonly SearchDummiesEffectData m_Data;
        private readonly IEffectRepository m_EffectRepository;

        public SearchDummiesEffect(SearchDummiesEffectData data, List<ValidatorWithPurpose> validators,
            IEffectRepository effectRepository) : base(data, validators)
        {
            m_Data = data;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new SearchDummiesEffect(m_Data, Validators, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var entities = BoardNavigator.Instance
                .WithinCircle(target, m_Data.Range)
                .SelectMany(cell => cell.GameObjectsInside)
                .Where(entity => entity.IsDummy() && entity.IsAlive() &&
                                 Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, entity))
                .OrderBy(entity => (entity.transform.position - caster.transform.position).sqrMagnitude)
                .ToList();

            if (m_Data.Limit > 0)
            {
                entities = entities.Take(m_Data.Limit).ToList();
            }

            foreach (var entity in entities)
            {
                m_EffectRepository.Find(m_Data.EffectId).Apply(caster, entity);
            }

            TriggerFinished();
        }
    }
}