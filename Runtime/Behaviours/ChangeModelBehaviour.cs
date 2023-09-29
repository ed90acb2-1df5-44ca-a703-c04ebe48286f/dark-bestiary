using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class ChangeModelBehaviour : Behaviour
    {
        private readonly ChangeModelBehaviourData m_Data;

        public ChangeModelBehaviour(ChangeModelBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            target.GetComponent<ActorComponent>().ChangeModel(m_Data.Model);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<ActorComponent>().RestoreModel();
        }
    }
}