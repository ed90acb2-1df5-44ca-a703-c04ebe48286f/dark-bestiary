using System.Collections.Generic;
using DarkBestiary.Audio;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class CreateSoundEffect : Effect
    {
        private readonly CreateSoundEffectData data;
        private readonly FmodAudioEngine audioEngine;

        public CreateSoundEffect(CreateSoundEffectData data, List<ValidatorWithPurpose> validators,
            FmodAudioEngine audioEngine) : base(data, validators)
        {
            this.data = data;
            this.audioEngine = audioEngine;
        }

        protected override Effect New()
        {
            return new CreateSoundEffect(this.data, this.Validators, this.audioEngine);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            this.audioEngine.PlayOneShot(this.data.Path, target);
            TriggerFinished();
        }
    }
}