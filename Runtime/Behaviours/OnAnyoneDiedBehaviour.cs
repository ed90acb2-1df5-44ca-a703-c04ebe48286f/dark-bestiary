using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnAnyoneDiedBehaviour : Behaviour
    {
        private readonly OnKillBehaviourData m_Data;
        private readonly Effect m_Effect;

        public OnAnyoneDiedBehaviour(OnKillBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            HealthComponent.AnyEntityDied += OnEntityDied;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            HealthComponent.AnyEntityDied -= OnEntityDied;
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.IsDummy())
            {
                return;
            }

            if (m_Data.DamageFlags != DamageFlags.None && (data.Damage.Flags & m_Data.DamageFlags) == 0 ||
                m_Data.DamageInfoFlags != DamageInfoFlags.None && (data.Damage.InfoFlags & m_Data.DamageInfoFlags) == 0)
            {
                return;
            }

            if (!Validators.ByPurpose(ValidatorPurpose.Other).Validate(Target, data.Victim))
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;
            clone.Apply(Caster, m_Data.EventSubject == BehaviourEventSubject.Me ? Target : data.Victim);
        }
    }
}