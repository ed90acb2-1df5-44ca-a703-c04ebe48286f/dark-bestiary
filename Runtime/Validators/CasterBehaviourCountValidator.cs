using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CasterBehaviourCountValidator : Validator
    {
        private readonly BehaviourCountValidatorData m_Data;

        public CasterBehaviourCountValidator(BehaviourCountValidatorData data)
        {
            m_Data = data;
        }

        public override bool Validate(GameObject caster, object target)
        {
            var behaviours = caster.GetComponent<BehavioursComponent>();

            return Comparator.Compare(
                behaviours.Behaviours.Where(b => b.Id == m_Data.BehaviourId).Sum(b => b.StackCount),
                m_Data.Value,
                m_Data.Comparator
            );
        }
    }
}