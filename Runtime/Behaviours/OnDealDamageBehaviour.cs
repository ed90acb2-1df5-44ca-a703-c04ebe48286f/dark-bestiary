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
    public class OnDealDamageBehaviour : Behaviour
    {
        private readonly OnDealDamageBehaviourData m_Data;
        private readonly Effect m_Effect;

        public OnDealDamageBehaviour(OnDealDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            if (data.Damage.Amount < 1 && Mathf.Approximately(0, data.Damage.Absorbed) || data.Source == data.Target)
            {
                return;
            }

            if (data.Source != Target ||
                m_Data.DamageFlags != DamageFlags.None &&
                (data.Damage.Flags & m_Data.DamageFlags) == 0 ||
                m_Data.DamageInfoFlags != DamageInfoFlags.None &&
                (data.Damage.InfoFlags & m_Data.DamageInfoFlags) == 0)
            {
                return;
            }

            if ((m_Data.ExcludeDamageFlags & data.Damage.Flags) > 0 ||
                (m_Data.ExcludeDamageInfoFlags & data.Damage.InfoFlags) > 0)
            {
                return;
            }

            if (m_Data.DamageTypes.Count > 0 && !m_Data.DamageTypes.Contains(data.Damage.Type))
            {
                return;
            }

            if (!Validators.ByPurpose(ValidatorPurpose.Other).Validate(data.Source, data.Target))
            {
                return;
            }

            var clone = m_Effect.Clone();
            clone.StackCount = StackCount;

            // Note: if this effect will apply behaviour with BreaksOnTakeDamage flag, it will be removed immediately.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                // Wait for BehavioursComponent OnTakeDamage callback...
                Timer.Instance.WaitForFixedUpdate(() =>
                {
                    clone.Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Target);
                });
            });
        }
    }
}