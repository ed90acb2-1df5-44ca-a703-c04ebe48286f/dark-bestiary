using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Components
{
    public class BehavioursComponent : Component
    {
        public static event Payload<Behaviour> AnyBehaviourApplied;
        public static event Payload<Behaviour> AnyBehaviourRemoved;
        public static event Payload<GameObject, Behaviour> AnyBehaviourImmune;

        public event Payload<Behaviour> BehaviourApplied;
        public event Payload<Behaviour> BehaviourRemoved;

        public List<Behaviour> Behaviours { get; private set; } = new List<Behaviour>();

        public bool IsUncontrollable => IsStunned || IsPolymorphed || IsConfused || IsSleeping;

        public bool IsSlowed => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Slow));
        public bool IsSwift => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Swiftness));
        public bool IsStunned => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Stun));
        public bool IsPolymorphed => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Polymorph));
        public bool IsPoisoned => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Poison));
        public bool IsDisarmed => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Disarm));
        public bool IsSilenced => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Silence));
        public bool IsWeakened => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Weakness));
        public bool IsAdrenaline => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Adrenaline));
        public bool IsInvisible => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Invisibility));
        public bool IsImmobilized => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Immobilization));
        public bool IsInvulnerable => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Invulnerability));
        public bool IsImmortal => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Immortal));
        public bool IsFreecasting => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Freecasting));
        public bool IsUndead => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Undead));
        public bool IsConfused => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Confusion));
        public bool IsSleeping => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.Sleep));
        public bool IsMindControlled => Behaviours.Any(behaviour => behaviour.StatusFlags.HasFlag(StatusFlags.MindControl));
        public bool IsCaged => Behaviours.Any(behaviour => behaviour is CageBehaviour);

        protected override void OnInitialize()
        {
            Skill.AnySkillUsed += OnAnySkillUsed;
            Scenario.AnyScenarioExit += OnScenarioAnyScenarioExit;
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged += OnAnyEntityDamaged;
            CombatEncounter.AnyCombatTeamTurnStarted += OnAnyCombatTeamTurnStarted;
            Episode.AnyEpisodeCompleted += OnAnyEpisodeComplete;

            GetComponent<HealthComponent>().Died += OnDied;
        }

        protected override void OnTerminate()
        {
            Skill.AnySkillUsed -= OnAnySkillUsed;
            Scenario.AnyScenarioExit -= OnScenarioAnyScenarioExit;
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
            HealthComponent.AnyEntityDamaged -= OnAnyEntityDamaged;
            CombatEncounter.AnyCombatTeamTurnStarted -= OnAnyCombatTeamTurnStarted;
            Episode.AnyEpisodeCompleted -= OnAnyEpisodeComplete;

            GetComponent<HealthComponent>().Died -= OnDied;

            RemoveBehaviours();
        }

        private void OnAnyCombatTeamTurnStarted(CombatEncounter combat)
        {
            if (combat.ActingTeamId != gameObject.GetComponent<UnitComponent>().TeamId)
            {
                return;
            }

            foreach (var behaviour in Behaviours.OrderBy(b => b.IsHarmful).ToList())
            {
                behaviour.MaybeExpire();

                if (behaviour.IsApplied)
                {
                    behaviour.Tick();
                }
            }
        }

        public bool IsBehaviourSetApplied(IEnumerable<Behaviour> behaviours)
        {
            return behaviours.All(b => Behaviours.Any(bb => bb.Id == b.Id));
        }

        public void Apply(Behaviour behaviour, GameObject caster)
        {
            if (!behaviour.IsIgnoresImmunity &&
                Behaviours.Any(element => (element.StatusImmunity & behaviour.StatusFlags) > 0) ||
                CombatEncounter.Active == null && behaviour.IsPreventsMovement())
            {
                AnyBehaviourImmune?.Invoke(gameObject, behaviour);
                return;
            }

            BeforeBehaviourApplied(behaviour);

            var applied = Behaviours.FirstOrDefault(b => b.Id == behaviour.Id);

            if (applied != null)
            {
                ReApply(applied, behaviour);
                return;
            }

            Behaviours.Add(behaviour);
            Behaviours = Behaviours.OrderBy(b => b.RemainingDuration).ToList();

            behaviour.Removed += OnBehaviourRemoved;
            behaviour.Apply(caster, gameObject);

            AnyBehaviourApplied?.Invoke(behaviour);
            BehaviourApplied?.Invoke(behaviour);
        }

        public void RemoveAllStacks(List<Behaviour> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                RemoveAllStacks(behaviour.Id);
            }
        }

        public void RemoveAllStacks(int behaviourId)
        {
            var behaviour = Behaviours.FirstOrDefault(b => b.Id == behaviourId);

            if (behaviour == null)
            {
                return;
            }

            RemoveStack(behaviour, behaviour.StackCount);
        }

        public IEnumerable<DamageBehaviour> DefensiveDamageBehaviours()
        {
            return Behaviours.OfType<DamageBehaviour>()
                .Where(behaviour => behaviour.Flags.HasFlag(BehaviourFlags.Defensive))
                .OrderBy(behaviour => behaviour.ModifierType);
        }

        public IEnumerable<DamageBehaviour> OffensiveDamageBehaviours()
        {
            return Behaviours.OfType<DamageBehaviour>()
                .Where(behaviour => behaviour.Flags.HasFlag(BehaviourFlags.Offensive))
                .OrderBy(behaviour => behaviour.ModifierType);
        }

        public int GetStackCount(int behaviourId)
        {
            return Behaviours
                .Where(b => b.Id == behaviourId)
                .Select(b => b.StackCount)
                .DefaultIfEmpty(0)
                .Sum();
        }

        public void SetStackCount(int behaviourId, int stack)
        {
            var behaviour = Behaviours.FirstOrDefault(b => b.Id == behaviourId);

            if (behaviour == null)
            {
                return;
            }

            behaviour.StackCount = stack;
        }

        public void RemoveStack(Behaviour behaviour, int stack = 1)
        {
            Behaviours.FirstOrDefault(b => b.Equals(behaviour))?.Remove(stack);
        }

        public void RemoveStack(int behaviourId, int stack = 1)
        {
            Behaviours.FirstOrDefault(behaviour => behaviour.Id == behaviourId)?.Remove(stack);
        }

        public StatusFlags GetStatusFlags()
        {
            var flags = StatusFlags.None;

            foreach (var behaviour in Behaviours)
            {
                flags |= behaviour.StatusFlags;
            }

            return flags;
        }

        public void SyncWith(BehavioursComponent behaviours)
        {
            foreach (var behaviour in behaviours.Behaviours)
            {
                if (GetStackCount(behaviour.Id) == 0)
                {
                    Apply(behaviour, behaviour.Caster);
                }

                Behaviours.First(b => b.Id == behaviour.Id).RemainingDuration = behaviour.RemainingDuration;
            }
        }

        private static void ReApply(Behaviour applied, Behaviour behaviour)
        {
            if (applied.ReApplyFlags.HasFlag(ReApplyBehaviourFlags.RefreshDuration))
            {
                applied.RefreshDuration(behaviour);
            }

            if (applied.ReApplyFlags.HasFlag(ReApplyBehaviourFlags.RefreshEffect))
            {
                applied.RefreshEffect(behaviour);
            }

            if (applied.ReApplyFlags.HasFlag(ReApplyBehaviourFlags.StackDuration))
            {
                applied.StackDuration(behaviour);
            }

            if (applied.ReApplyFlags.HasFlag(ReApplyBehaviourFlags.StackEffect))
            {
                applied.StackEffect(behaviour);
            }

            if (applied.StackCountMax > 0 && applied.StackCount < applied.StackCountMax)
            {
                applied.StackCount = Math.Min(applied.StackCount + behaviour.StackCount, applied.StackCountMax);
            }
        }

        private void RemoveBehaviours(Func<Behaviour, bool> predicate = null)
        {
            Behaviours
                .Where(behaviour => predicate?.Invoke(behaviour) ?? true)
                .ToList()
                .ForEach(behaviour => behaviour.Remove(behaviour.StackCount));
        }

        private void RemoveTemporaryBehaviours(Func<Behaviour, bool> predicate = null)
        {
            RemoveBehaviours(behaviour => (predicate?.Invoke(behaviour) ?? true) && behaviour.Duration >= 1 || behaviour.Flags.HasFlag(BehaviourFlags.Temporary));
        }

        private void BeforeBehaviourApplied(Behaviour applied)
        {
            if (!applied.IsPreventsActions())
            {
                return;
            }

            foreach (var behaviour in Behaviours.Where(behaviour => behaviour.Flags.HasFlag(BehaviourFlags.BreaksOnCrowdControl)).ToList())
            {
                RemoveAllStacks(behaviour.Id);
            }
        }

        private void OnAnyEpisodeComplete(Episode episode)
        {
            foreach (var behaviour in Behaviours.Where(behaviour => behaviour.IsPreventsMovement()).ToList())
            {
                RemoveStack(behaviour, behaviour.StackCount);
            }
        }

        private void OnAnyEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Victim == gameObject)
            {
                OnTakeDamage(data);
            }
            else if (data.Attacker == gameObject)
            {
                OnDealDamage(data);
            }
        }

        private void OnTakeDamage(EntityDamagedEventData data)
        {
            if (data.Damage.Amount < 1)
            {
                return;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                foreach (var behaviour in Behaviours.Where(
                    behaviour => behaviour.Flags.HasFlag(BehaviourFlags.BreaksOnTakeDamage)).ToList())
                {
                    RemoveStack(behaviour, behaviour.StackCount);
                }
            });
        }

        private void OnDealDamage(EntityDamagedEventData data)
        {
            if (data.Damage.Amount < 1)
            {
                return;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                foreach (var behaviour in Behaviours.Where(
                    behaviour => behaviour.Flags.HasFlag(BehaviourFlags.BreaksOnDealDamage)).ToList())
                {
                    RemoveStack(behaviour, behaviour.StackCount);
                }

                if (!data.Damage.Flags.HasFlag(DamageFlags.DOT))
                {
                    foreach (var behaviour in Behaviours.Where(
                        behaviour => behaviour.Flags.HasFlag(BehaviourFlags.BreaksOnDealDirectDamage)).ToList())
                    {
                        RemoveStack(behaviour, behaviour.StackCount);
                    }
                }
            });
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            foreach (var behaviour in Behaviours.Where(behaviour =>
                behaviour.Flags.HasFlag(BehaviourFlags.BreaksOnCasterDeath) &&
                behaviour.Caster == data.Victim).ToList())
            {
                RemoveStack(behaviour, behaviour.StackCount);
            }
        }

        private void OnAnySkillUsed(SkillUseEventData data)
        {
            var behaviours = Behaviours.Where(b => b.Flags.HasFlag(BehaviourFlags.BreaksOnCast) &&
                                                   b.Caster == data.Caster).ToList();

            foreach (var behaviour in behaviours)
            {
                if (!behaviour.CanBeRemovedOnCast)
                {
                    continue;
                }

                RemoveStack(behaviour, behaviour.StackCount);
            }

            // Skills triggering "AnySkillUsed" event right after behaviour is applied.

            Timer.Instance.WaitForEndOfFrame(() =>
            {
                foreach (var behaviour in behaviours)
                {
                    if (behaviour.CanBeRemovedOnCast)
                    {
                        continue;
                    }

                    behaviour.CanBeRemovedOnCast = true;
                }
            });
        }

        private void OnDied(EntityDiedEventData data)
        {
            if (gameObject.IsCharacter())
            {
                RemoveTemporaryBehaviours(behaviour => !behaviour.Flags.HasFlag(BehaviourFlags.DoNotRemoveOnDeath));
            }
            else
            {
                RemoveBehaviours(behaviour => !behaviour.Flags.HasFlag(BehaviourFlags.DoNotRemoveOnDeath));
            }
        }

        private void OnScenarioAnyScenarioExit(Scenario scenario)
        {
            RemoveTemporaryBehaviours();
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            Behaviours.Remove(behaviour);
            behaviour.Removed -= OnBehaviourRemoved;

            BehaviourRemoved?.Invoke(behaviour);
            AnyBehaviourRemoved?.Invoke(behaviour);
        }
    }
}