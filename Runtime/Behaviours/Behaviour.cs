using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public abstract class Behaviour
    {
        public event Action<Behaviour> Applied;
        public event Action<Behaviour> Removed;
        public event Action<Behaviour> StackCountChanged;
        public event Action<Behaviour> RemainingDurationChanged;

        public int Id { get; }
        public string Icon { get; }
        public string Label { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public int Duration { get; }
        public int Period { get; }
        public bool CasterIsBearer { get; }
        public Rarity Rarity { get; }
        public BehaviourFlags Flags { get; }
        public ReApplyBehaviourFlags ReApplyFlags { get; }
        public StatusFlags StatusFlags { get; }
        public StatusFlags StatusImmunity { get; }
        public BehaviourEventSubject EventSubject { get; }
        public List<AttachmentInfo> Attachments { get; }
        public GameObject Caster { get; set; }
        public GameObject Target { get; set; }
        public bool CanBeRemovedOnCast { get; set; }

        public GameObject OriginalCaster { get; private set; }
        public bool IsApplied { get; private set; }

        public bool IsDispellable => Flags.HasFlag(BehaviourFlags.Dispellable);
        public bool IsUnique => Flags.HasFlag(BehaviourFlags.Unique);
        public bool IsOneshot => Flags.HasFlag(BehaviourFlags.Oneshot);
        public bool IsHarmful => Flags.HasFlag(BehaviourFlags.Negative);
        public bool IsHidden => Flags.HasFlag(BehaviourFlags.Hidden);
        public bool IsAscension => Flags.HasFlag(BehaviourFlags.Ascension);
        public bool IsEpisodeAffix => Flags.HasFlag(BehaviourFlags.EpisodeAffix);
        public bool IsMonsterAffix => Flags.HasFlag(BehaviourFlags.MonsterAffix);
        public bool IsMonsterModifier => Flags.HasFlag(BehaviourFlags.MonsterModifier);
        public bool IsFood => Flags.HasFlag(BehaviourFlags.Food);
        public bool IsIgnoresImmunity => Flags.HasFlag(BehaviourFlags.IgnoreImmunity);

        public int RemainingDuration
        {
            get => m_RemainingDuration;
            set
            {
                m_RemainingDuration = value;
                RemainingDurationChanged?.Invoke(this);
            }
        }

        public int StackCountMax { get; }

        public int StackCount
        {
            get => m_StackCount;
            set
            {
                var previous = m_StackCount;

                m_StackCount = Mathf.Clamp(value, 1, Mathf.Max(StackCountMax, 1));
                OnStackCountChanged(this, m_StackCount - previous);

                if (!IsApplied)
                {
                    return;
                }

                StackCountChanged?.Invoke(this);
            }
        }

        // Note: to prevent circular dependencies
        protected static readonly IEffectRepository s_EffectRepository = Container.Instance.Resolve<IEffectRepository>();
        protected static readonly IRarityRepository s_RarityRepository = Container.Instance.Resolve<IRarityRepository>();

        protected readonly List<ValidatorWithPurpose> Validators;

        private readonly BehaviourData m_Data;
        private readonly Effect m_OnApplyEffect;
        private readonly Effect m_OnExpireEffect;
        private readonly Effect m_OnRemoveEffect;

        private int m_Counter;
        private int m_RemainingDuration;
        private int m_StackCount = 1;

        protected Behaviour(BehaviourData data, List<ValidatorWithPurpose> validators)
        {
            Id = data.Id;
            Icon = data.Icon;
            Label = data.Label;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Period = Math.Max(1, data.Period);
            Duration = data.Duration;
            RemainingDuration = 0;
            Flags = data.Flags;
            ReApplyFlags = data.ReApplyFlags;
            StatusFlags = data.StatusFlags;
            StatusImmunity = data.StatusImmunity;
            Attachments = data.Attachments;
            EventSubject = data.EventSubject;
            StackCountMax = data.StackCountMax;
            CasterIsBearer = data.CasterIsBearer;
            Rarity = s_RarityRepository.Find(data.RarityId);

            m_Data = data;
            Validators = validators;

            m_OnApplyEffect = s_EffectRepository.Find(data.OnApplyEffectId);
            m_OnExpireEffect = s_EffectRepository.Find(data.OnExpireEffectId);
            m_OnRemoveEffect = s_EffectRepository.Find(data.OnRemoveEffectId);
        }

        public void Apply(GameObject caster, GameObject target)
        {
            if (IsApplied)
            {
                return;
            }

            OriginalCaster = caster;
            Caster = CasterIsBearer ? target : caster;
            Target = target;
            RemainingDuration = Duration;

            if (!Validators.ByPurpose(ValidatorPurpose.Apply).Validate(Caster, Target))
            {
                return;
            }

            OnApply(caster, target);
            MaybeApplyOnApplyEffect(caster, target);
            IsApplied = true;

            var actor = target.GetComponent<ActorComponent>();
            actor.CreateAttachments(this, Attachments);

            if (!Mathf.Approximately(1, m_Data.Scale))
            {
                actor.Model.ChangeScale(m_Data.Scale);
            }

            if (!Mathf.Approximately(1, m_Data.Transparency))
            {
                actor.Model.ChangeTransparency(m_Data.Transparency);
            }

            if (!string.IsNullOrEmpty(m_Data.Tint))
            {
                actor.Model.ChangeColor(m_Data.Tint.ToColor());
            }

            Applied?.Invoke(this);
        }

        public void Tick()
        {
            m_Counter++;

            if (m_Counter < Period)
            {
                return;
            }

            m_Counter = 0;

            OnTick(Caster, Target);
        }

        public void MaybeExpire()
        {
            if (Duration <= 0)
            {
                return;
            }

            RemainingDuration -= Period;

            if (RemainingDuration > 0)
            {
                return;
            }

            MaybeApplyOnExpireEffect(Caster, Target);
            OnExpire(Caster, Target);
            Remove(StackCount);
        }

        public void Remove(int stack = 1)
        {
            if (!IsApplied)
            {
                return;
            }

            if (StackCount - stack > 0)
            {
                StackCount -= stack;
                return;
            }

            IsApplied = false;

            OnRemove(Caster, Target);

            var actor = Target.GetComponent<ActorComponent>();

            if (!Mathf.Approximately(1, m_Data.Scale))
            {
                actor.Model.ResetScale();
            }

            if (!Mathf.Approximately(1, m_Data.Transparency))
            {
                actor.Model.ChangeTransparency(1);
            }

            if (!string.IsNullOrEmpty(m_Data.Tint))
            {
                actor.Model.ResetColor();
            }

            actor.DestroyAttachments(this);

            Removed?.Invoke(this);

            MaybeApplyOnRemovedEffect(Caster, Target);
            OnRemoved(Caster, Target);
        }

        public override bool Equals(object target)
        {
            return (target as Behaviour)?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public string GetName(GameObject entity)
        {
            return Name;
        }

        public string GetDuration(GameObject entity)
        {
            return Duration.ToString();
        }

        public bool IsPreventsMovement()
        {
            return IsPreventsActions() || StatusFlags.HasFlag(StatusFlags.Immobilization);
        }

        public bool IsPreventsActions()
        {
            return StatusFlags.HasFlag(StatusFlags.Stun) ||
                   StatusFlags.HasFlag(StatusFlags.Polymorph) ||
                   StatusFlags.HasFlag(StatusFlags.Confusion) ||
                   StatusFlags.HasFlag(StatusFlags.Sleep);
        }

        public void RefreshDuration(Behaviour behaviour)
        {
            RemainingDuration = Duration;
        }

        public void RefreshEffect(Behaviour behaviour)
        {
            OnRefreshEffect(behaviour);
        }

        public void StackEffect(Behaviour behaviour)
        {
            OnStackEffect(behaviour);
        }

        public void StackDuration(Behaviour behaviour)
        {
            RemainingDuration += behaviour.Duration;
        }

        protected virtual void OnApply(GameObject caster, GameObject target)
        {
        }

        protected virtual void OnRefreshEffect(Behaviour behaviour)
        {
        }

        protected virtual void OnStackEffect(Behaviour behaviour)
        {
        }

        protected virtual void OnStackCountChanged(Behaviour behaviour, int delta)
        {
        }

        protected virtual void OnRemove(GameObject source, GameObject target)
        {
        }

        protected virtual void OnRemoved(GameObject source, GameObject target)
        {
        }

        protected virtual void OnTick(GameObject caster, GameObject target)
        {
        }

        protected virtual void OnExpire(GameObject caster, GameObject target)
        {
        }

        private void MaybeApplyOnApplyEffect(GameObject caster, GameObject target)
        {
            MaybeApplyEffectClone(caster, target, m_OnApplyEffect);
        }

        private void MaybeApplyOnExpireEffect(GameObject caster, GameObject target)
        {
            MaybeApplyEffectClone(caster, target, m_OnExpireEffect);
        }

        private void MaybeApplyOnRemovedEffect(GameObject caster, GameObject target)
        {
            MaybeApplyEffectClone(caster, target, m_OnRemoveEffect);
        }

        private void MaybeApplyEffectClone(GameObject caster, GameObject target, Effect effect)
        {
            if (effect == null)
            {
                return;
            }

            var clone = effect.Clone();
            clone.Origin = target;
            clone.StackCount = StackCount;
            clone.Apply(caster, target);
        }
    }
}