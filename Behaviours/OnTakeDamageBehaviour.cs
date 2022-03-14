using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnTakeDamageBehaviour : Behaviour
    {
        private readonly OnTakeDamageBehaviourData data;
        private readonly Effect effect;

        public OnTakeDamageBehaviour(OnTakeDamageBehaviourData data,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
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
                data.Attacker == data.Victim)
            {
                return;
            }

            if (data.Attacker == Target ||
                this.data.DamageFlags != DamageFlags.None &&
                (data.Damage.Flags & this.data.DamageFlags) == 0 ||
                this.data.DamageInfoFlags != DamageInfoFlags.None &&
                (data.Damage.InfoFlags & this.data.DamageInfoFlags) == 0)
            {
                return;
            }

            if ((this.data.ExcludeDamageFlags & data.Damage.Flags) > 0 ||
                (this.data.ExcludeDamageInfoFlags & data.Damage.InfoFlags) > 0)
            {
                return;
            }

            if (this.data.DamageTypes.Count > 0 && !this.data.DamageTypes.Contains(data.Damage.Type))
            {
                return;
            }

            this.effect.Clone().Apply(Caster, EventSubject == BehaviourEventSubject.Me ? Target : data.Attacker);
        }
    }
}