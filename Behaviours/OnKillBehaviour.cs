using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnKillBehaviour : Behaviour
    {
        private readonly OnKillBehaviourData data;
        private readonly Effect effect;

        public OnKillBehaviour(OnKillBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
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
            if (data.Victim.IsAllyOf(Target) || data.Killer != Target ||
                this.data.DamageFlags != DamageFlags.None && (data.Damage.Flags & this.data.DamageFlags) == 0 ||
                this.data.DamageInfoFlags != DamageInfoFlags.None && (data.Damage.InfoFlags & this.data.DamageInfoFlags) == 0)
            {
                return;
            }

            this.effect.Clone().Apply(Caster, Target);
        }
    }
}