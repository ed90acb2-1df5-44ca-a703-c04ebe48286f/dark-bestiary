using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.Utility;
using DarkBestiary.Validators;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DarkBestiary.Effects
{
    public abstract class Effect
    {
        public event Action<Effect> Finished;

        public int Id => m_Data.Id;
        public string Name => m_Data.Name;
        public object Origin { get; set; }
        public float DamageMultiplier { get; set; } = 1;
        public Skill Skill { get; set; }
        public bool IsFailed { get; protected set; }

        public int StackCount
        {
            get => m_StackCount;
            set => m_StackCount = Mathf.Clamp(value, 1, m_Data.StackCountMax > 0 ? m_Data.StackCountMax : int.MaxValue);
        }

        public GameObject Caster { get; protected set; }
        public object OriginalTarget { get; private set; }
        public object Target { get; protected set; }
        public bool IsFinished { get; private set; }

        protected readonly List<ValidatorWithPurpose> Validators;

        private static int s_SessionCounter;

        private readonly EffectData m_Data;

        private int m_StackCount = 1;
        private bool m_IsApplied;

        protected Effect(EffectData data, List<ValidatorWithPurpose> validators)
        {
            m_Data = data;
            Validators = validators;
        }

        public Effect Clone()
        {
            var effect = New();
            effect.Skill = Skill;
            effect.DamageMultiplier = DamageMultiplier;

            return effect;
        }

        protected Effect Inherit(Effect effect)
        {
            effect.Skill = Skill;
            effect.Origin = Origin;
            effect.StackCount = StackCount;
            effect.DamageMultiplier = DamageMultiplier;

            return effect;
        }

        protected abstract Effect New();

        public string GetChanceString(GameObject entity)
        {
            return $"{m_Data.Chance:P}";
        }

        public void Apply(GameObject caster, object target)
        {
            if (m_IsApplied)
            {
                throw new Exception($"Trying to re-apply effect: {m_Data.Name}");
            }

            m_IsApplied = true;

            OriginalTarget = target;
            Caster = DetermineCaster(caster);
            Target = DetermineTarget(Caster, target);
            Origin = Origin ?? Caster;

            if (!Rng.Test(m_Data.StackChance ? m_Data.Chance * StackCount : m_Data.Chance))
            {
                TriggerFinished();
                return;
            }

            if (!Validators.ByPurpose(ValidatorPurpose.Apply).Validate(Caster, Target))
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
            if (string.IsNullOrEmpty(m_Data.Animation))
            {
                return;
            }

            var animation = Skill?.DetermineAnimation(m_Data.Animation) ?? m_Data.Animation;

            Caster.GetComponent<ActorComponent>().PlayAnimation(animation);
        }

        private void PlaySound()
        {
            if (string.IsNullOrEmpty(m_Data.Sound))
            {
                return;
            }

            AudioManager.Instance.PlayOneShot(m_Data.Sound);
        }

        protected object DetermineTarget(GameObject caster, object target)
        {
            switch (m_Data.Target)
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
            if (!m_Data.CasterIsOwner)
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
            foreach (var attachment in m_Data.Attachments)
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