using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetBehaviourCountValidator : Validator
    {
        private readonly BehaviourCountValidatorData m_Data;

        public TargetBehaviourCountValidator(BehaviourCountValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            var behavioursComponent = targetAsGameObject.GetComponent<BehavioursComponent>();

            if (behavioursComponent == null)
            {
                return false;
            }

            return Comparator.Compare(
                behavioursComponent.Behaviours.Where(b => b.Id == m_Data.BehaviourId).Sum(b => b.StackCount),
                m_Data.Value,
                m_Data.Comparator
            );
        }
    }
}