using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Items;
using DarkBestiary.Validators;
using DarkBestiary.Visuals;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class CreateBeamEffect : Effect
    {
        private readonly CreateBeamEffectData m_Data;
        private readonly string m_Prefab;

        public CreateBeamEffect(CreateBeamEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Prefab = data.Path;
        }

        protected override Effect New()
        {
            return new CreateBeamEffect(m_Data, Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.GetComponent<ActorComponent>()
                .Model.GetAttachmentPoint(AttachmentPoint.Chest).transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (!(Object.Instantiate(Resources.Load(m_Prefab, typeof(ILightning))) is ILightning beam))
            {
                Debug.LogError($"No beam found at {m_Prefab}");
                TriggerFinished();
                return;
            }

            beam.Initialize(
                caster.Equals(Origin)
                    ? caster.GetComponent<ActorComponent>()
                        .Model.GetAttachmentPoint(AttachmentPoint.RightHand).transform.position
                    : OriginPosition(), target);

            beam.FadeOut(0.25f, 0.5f);
            beam.Destroy(0.75f);

            TriggerFinished();
        }
    }
}