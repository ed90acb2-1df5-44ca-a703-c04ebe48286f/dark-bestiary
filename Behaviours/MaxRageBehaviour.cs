using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class MaxRageBehaviour : Behaviour
    {
        private readonly Behaviour behaviour;

        public MaxRageBehaviour(MaxRageBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.behaviour = Container.Instance.Resolve<IBehaviourRepository>().Find(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Target.GetComponent<ResourcesComponent>().RageChanged += OnRageChanged;
        }

        protected override void OnRemove(GameObject source, GameObject target)
        {
            Target.GetComponent<ResourcesComponent>().RageChanged -= OnRageChanged;
            this.behaviour.Remove();
        }

        private void OnRageChanged(Resource resource)
        {
            if (Math.Abs(resource.Amount - resource.MaxAmount) < Mathf.Epsilon)
            {
                if (this.behaviour.IsApplied)
                {
                    return;
                }

                Target.GetComponent<BehavioursComponent>().ApplyStack(this.behaviour, Caster);
            }
            else
            {
                if (!this.behaviour.IsApplied)
                {
                    return;
                }

                Target.GetComponent<BehavioursComponent>().RemoveStack(this.behaviour);
            }
        }
    }
}