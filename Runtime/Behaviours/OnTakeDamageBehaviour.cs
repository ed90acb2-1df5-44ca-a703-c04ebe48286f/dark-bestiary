using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Events;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnTakeDamageBehaviour : Behaviour
    {
        private readonly OnTakeDamageBehaviourData m_Data;
        private readonly Effect m_Effect;

        public OnTakeDamageBehaviour(OnTakeDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
            m_Effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            target.GetComponent<HealthComponent>().Damaged += OnDamaged;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<HealthComponent>().Damaged -= OnDamaged;
        }

        private void OnDamaged(EntityDamagedEventData data)
        {
            if (data.Damage.Amount < 1 && !data.Damage.IsDodged() &&
                Mathf.Approximately(0, data.Damage.Absorbed) && !data.Damage.IsInvulnerable() ||
                data.Source == data.Target)
            {
                return;
            }

            if (data.Source == Target ||
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

            m_Effect.Clone().Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Source);
        }
    }
}