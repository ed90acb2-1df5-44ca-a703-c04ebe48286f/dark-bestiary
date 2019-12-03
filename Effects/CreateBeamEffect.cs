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
        private readonly CreateBeamEffectData data;
        private readonly string prefab;

        public CreateBeamEffect(CreateBeamEffectData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
            this.prefab = data.Path;
        }

        protected override Effect New()
        {
            return new CreateBeamEffect(this.data, this.Validators);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.GetComponent<ActorComponent>()
                .Model.GetAttachmentPoint(AttachmentPoint.Chest).transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            if (!(Object.Instantiate(Resources.Load(this.prefab, typeof(ILightning))) is ILightning beam))
            {
                Debug.LogError($"No beam found at {this.prefab}");
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