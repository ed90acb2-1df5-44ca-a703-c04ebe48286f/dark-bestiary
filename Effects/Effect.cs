using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.Utility;
using DarkBestiary.Validators;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DarkBestiary.Effects
{
    public abstract class Effect
    {
        public event Payload<Effect> Finished;

        public int Id => this.data.Id;
        public string Name => this.data.Name;
        public object Origin { get; set; }
        public Skill Skill { get; set; }
        public bool IsFailed { get; protected set; }

        public int StackCount
        {
            get => this.stackCount;
            set => this.stackCount = Math.Max(1, value);
        }

        public GameObject Caster { get; protected set; }
        public object OriginalTarget { get; private set; }
        public object Target { get; protected set; }
        public bool IsFinished { get; private set; }

        protected readonly List<Validator> Validators;

        private static int sessionCounter;

        private readonly EffectData data;

        private int stackCount = 1;
        private bool isApplied;

        protected Effect(EffectData data, List<Validator> validators)
        {
            this.data = data;
            this.Validators = validators;
        }

        public Effect Clone()
        {
            var effect = New();
            effect.Skill = Skill;

            return effect;
        }

        protected Effect Inherit(Effect effect)
        {
            effect.Skill = Skill;
            effect.Origin = Origin;
            effect.StackCount = StackCount;

            return effect;
        }

        protected abstract Effect New();

        public string GetChanceString(GameObject entity)
        {
            return $"{this.data.Chance:P}";
        }

        public void Apply(GameObject caster, object target)
        {
            if (this.isApplied)
            {
                throw new Exception($"Trying to re-apply effect: {this.data.Name}");
            }

            this.isApplied = true;

            OriginalTarget = target;
            Caster = DetermineCaster(caster);
            Target = DetermineTarget(Caster, target);
            Origin = Origin ?? Caster;

            if (!RNG.Test(this.data.Chance))
            {
                TriggerFinished();
                return;
            }

            if (this.Validators.Any(validator => !validator.Validate(Caster, Target)))
            {
                TriggerFinished();
                return;
            }

            CreateAttachments();
            PlayAnimation();
            PlaySound();

            if (Target is GameObject)
            {
                Apply(Caster, (GameObject) Target);
            }
            else
            {
                Apply(Caster, (Vector3) Target);
            }
        }

        private void PlayAnimation()
        {
            if (string.IsNullOrEmpty(this.data.Animation))
            {
                return;
            }

            var animation = Skill?.DetermineAnimation(this.data.Animation) ?? this.data.Animation;

            Caster.GetComponent<ActorComponent>().PlayAnimation(animation);
        }

        private void PlaySound()
        {
            if (string.IsNullOrEmpty(this.data.Sound))
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(this.data.Sound);
        }

        protected object DetermineTarget(GameObject caster, object target)
        {
            switch (this.data.Target)
            {
                case EffectTargetType.Target:
                    return (GameObject) target;
                case EffectTargetType.Caster:
                    return caster;
                case EffectTargetType.CasterPoint:
                    return caster.transform.position;
                case EffectTargetType.TargetPoint:
                    return target.GetPosition();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected GameObject DetermineCaster(GameObject original)
        {
            if (!this.data.CasterIsOwner)
            {
                return original;
            }

            var caster = original;

            while (true)
            {
                var summoned = caster.GetComponent<SummonedComponent>();

                if (summoned == null)
                {
                    break;
                }

                caster = summoned.Master;
            }

            return caster;
        }

        protected virtual void Apply(GameObject caster, GameObject target)
        {
            throw new NotImplementedException();
        }

        protected virtual void Apply(GameObject caster, Vector3 target)
        {
            throw new NotImplementedException();
        }

        protected Vector3 OriginPosition()
        {
            var entity = Origin as GameObject;

            if (entity != null)
            {
                return entity.transform.position;
            }

            if (Origin is Vector3 vector3)
            {
                return vector3;
            }

            return Caster.transform.position;
        }

        protected void TriggerFinished()
        {
            IsFinished = true;
            Finished?.Invoke(this);
        }

        protected void CreateAttachments()
        {
            foreach (var attachment in this.data.Attachments)
            {
                CreateAttachment(
                    attachment.Target == CasterOrTarget.Target ? Caster.transform.position : Target.GetPosition(),
                    attachment.Target == CasterOrTarget.Target ? Target : Caster,
                    attachment
                );
            }
        }

        private void CreateAttachment(Vector3 origin, object target, AttachmentInfo attachment)
        {
            var prefab = Resources.Load<GameObject>(attachment.Prefab);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {attachment.Prefab} not found.");
                return;
            }

            var rotation = prefab.transform.rotation;

            if (attachment.Rotate)
            {
                if (attachment.RotateSameAsCaster)
                {
                    rotation = Caster.GetComponent<ActorComponent>().Model.transform.rotation;
                }
                else
                {
                    rotation = QuaternionUtility.LookRotation2D(
                        attachment.FlipRotation ? target.GetPosition() - origin : origin - target.GetPosition());
                }
            }

            var position = target.GetPosition();

            if (target is GameObject targetGameObject)
            {
                var attachmentPointTransform = targetGameObject.GetComponent<ActorComponent>().Model
                    .GetAttachmentPoint(attachment.Point);

                position = attachmentPointTransform.position;
            }

            Object.Instantiate(prefab, position, rotation).DestroyAsVisualEffect();
        }
    }
}