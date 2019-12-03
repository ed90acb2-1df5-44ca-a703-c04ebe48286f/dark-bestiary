using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class ChangeOwnerBehaviour : Behaviour
    {
        public ChangeOwnerBehaviour(EmptyBehaviourData data, List<Validator> validators) : base(data, validators)
        {
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            var unit = target.GetComponent<UnitComponent>();
            unit.TeamId = caster.GetComponent<UnitComponent>().TeamId;
            unit.Owner = Owner.Neutral;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            target.GetComponent<UnitComponent>().RestorePreviousOwner();
        }
    }
}