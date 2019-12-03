using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnStatusEffectRemovedBehaviour : Behaviour
    {
        private readonly OnStatusEffectRemovedBehaviourData data;
        private readonly Effect effect;

        public OnStatusEffectRemovedBehaviour(OnStatusEffectRemovedBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
            this.effect = Container.Instance.Resolve<IEffectRepository>().FindOrFail(data.EffectId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            BehavioursComponent.AnyBehaviourRemoved += OnAnyBehaviourRemoved;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            BehavioursComponent.AnyBehaviourRemoved -= OnAnyBehaviourRemoved;
        }

        private void OnAnyBehaviourRemoved(Behaviour behaviour)
        {
            if (behaviour.Target != Target)
            {
                return;
            }

            if ((behaviour.StatusFlags & this.data.StatusFlags) == 0)
            {
                return;
            }

            this.effect.Clone().Apply(Target, EventSubject == BehaviourEventSubject.Me ? Target : Caster);
        }
    }
}