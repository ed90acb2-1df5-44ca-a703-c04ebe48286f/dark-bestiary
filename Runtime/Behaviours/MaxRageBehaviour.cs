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
        private readonly Behaviour m_Behaviour;

        public MaxRageBehaviour(MaxRageBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Behaviour = Container.Instance.Resolve<IBehaviourRepository>().Find(data.BehaviourId);
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Target.GetComponent<ResourcesComponent>().RageChanged += OnRageChanged;
        }

        protected override void OnRemove(GameObject source, GameObject target)
        {
            Target.GetComponent<ResourcesComponent>().RageChanged -= OnRageChanged;
            m_Behaviour.Remove();
        }

        private void OnRageChanged(Resource resource)
        {
            if (Math.Abs(resource.Amount - resource.MaxAmount) < Mathf.Epsilon)
            {
                if (m_Behaviour.IsApplied)
                {
                    return;
                }

                Target.GetComponent<BehavioursComponent>().ApplyStack(m_Behaviour, Caster);
            }
            else
            {
                if (!m_Behaviour.IsApplied)
                {
                    return;
                }

                Target.GetComponent<BehavioursComponent>().RemoveStack(m_Behaviour);
            }
        }
    }
}