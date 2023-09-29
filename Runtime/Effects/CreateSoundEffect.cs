using System.Collections.Generic;
using DarkBestiary.Audio;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class CreateSoundEffect : Effect
    {
        private readonly CreateSoundEffectData m_Data;
        private readonly FmodAudioEngine m_AudioEngine;

        public CreateSoundEffect(CreateSoundEffectData data, List<ValidatorWithPurpose> validators,
            FmodAudioEngine audioEngine) : base(data, validators)
        {
            m_Data = data;
            m_AudioEngine = audioEngine;
        }

        protected override Effect New()
        {
            return new CreateSoundEffect(m_Data, Validators, m_AudioEngine);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            m_AudioEngine.PlayOneShot(m_Data.Path, target);
            TriggerFinished();
        }
    }
}