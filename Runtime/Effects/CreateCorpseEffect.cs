using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class CreateCorpseEffect : Effect
    {
        private readonly CreateCorpseEffectData m_Data;

        public CreateCorpseEffect(CreateCorpseEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override Effect New()
        {
            return new CreateCorpseEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            CorpseSpawner.Instance.SpawnCorpse(target, Resources.Load<Corpse>(m_Data.Prefab));
            TriggerFinished();
        }
    }
}