using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class ChangeModelBehaviour : Behaviour
    {
        private readonly ChangeModelBehaviourData data;

        public ChangeModelBehaviour(ChangeModelBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            target.GetComponent<ActorComponent>().ChangeModel(this.data.Model);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<ActorComponent>().RestoreModel();
        }
    }
}