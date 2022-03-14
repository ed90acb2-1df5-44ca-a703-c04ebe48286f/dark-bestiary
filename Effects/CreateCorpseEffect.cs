using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class CreateCorpseEffect : Effect
    {
        private readonly CreateCorpseEffectData data;

        public CreateCorpseEffect(CreateCorpseEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override Effect New()
        {
            return new CreateCorpseEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            CorpseSpawner.Instance.SpawnCorpse(target, Resources.Load<Corpse>(this.data.Prefab));
            TriggerFinished();
        }
    }
}