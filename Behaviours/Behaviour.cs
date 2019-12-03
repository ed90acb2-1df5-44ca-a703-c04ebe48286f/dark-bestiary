using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public abstract class Behaviour
    {
        public event Payload<Behaviour> Applied;
        public event Payload<Behaviour> Removed;
        public event Payload<Behaviour> StackCountChanged;
        public event Payload<Behaviour> RemainingDurationChanged;

        public int Id { get; }
        public string Icon { get; }
        public string Label { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public int Duration { get; }
        public int Period { get; }
        public bool CasterIsBearer { get; }
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
        public bool IsTicked { get; private set; }

        public bool IsHarmful => Flags.HasFlag(BehaviourFlags.Negative);
        public bool IsHidden => Flags.HasFlag(BehaviourFlags.Hidden);
        public bool IsEpisodeAffix => Flags.HasFlag(BehaviourFlags.EpisodeAffix);
        public bool IsMonsterAffix => Flags.HasFlag(BehaviourFlags.MonsterAffix);
        public bool IsMonsterModifier => Flags.HasFlag(BehaviourFlags.MonsterModifier);
        public bool IsFood => Flags.HasFlag(BehaviourFlags.Food);
        public bool IsIgnoresImmunity => Flags.HasFlag(BehaviourFlags.IgnoreImmunity);

        public int RemainingDuration
        {
            get => this.remainingDuration;
            set
            {
                this.remainingDuration = value;
                RemainingDurationChanged?.Invoke(this);
            }
        }

        public int StackCountMax { get; }

        public int StackCount
        {
            get => this.stackCount;
            set
            {
                this.stackCount = value;
                StackCountChanged?.Invoke(this);
                OnStackCountChanged(this);
            }
        }

        protected readonly List<Validator> Validators;

        private readonly BehaviourData data;

        private int counter;
        private int remainingDuration;
        private int stackCount = 1;

        protected Behaviour(BehaviourData data, List<Validator> validators)
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

            this.data = data;

            this.Validators = validators;
        }

        public void Tick()
        {
            this.counter++;

            if (this.counter < Period)
            {
                return;
            }

            this.counter = 0;

            OnTick(Caster, Target);

            IsTicked = true;
        }

        public void MaybeExpire()
        {
            if (Duration <= 0 || !IsTicked)
            {
                return;
            }

            RemainingDuration -= Period;

            if (RemainingDuration > 0)
            {
                return;
            }

            OnExpire(Caster, Target);
            Remove(StackCount);
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

            OnApply(caster, target);
            IsApplied = true;
            IsTicked = false;

            var actor = target.GetComponent<ActorComponent>();
            actor.CreateAttachments(this, Attachments);

            if (!Mathf.Approximately(1, this.data.Scale))
            {
                actor.Model.ChangeScale(this.data.Scale);
            }

            if (!string.IsNullOrEmpty(this.data.Tint))
            {
                actor.Model.ChangeColor(this.data.Tint.ToColor());
            }

            Applied?.Invoke(this);
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

            if (!Mathf.Approximately(1, this.data.Scale))
            {
                actor.Model.ResetScale();
            }

            if (!string.IsNullOrEmpty(this.data.Tint))
            {
                actor.Model.ResetColor();
            }

            actor.DestroyAttachments(this);

            Removed?.Invoke(this);

            OnRemoved(Caster, Target);
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

        protected virtual void OnStackCountChanged(Behaviour behaviour)
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
    }
}