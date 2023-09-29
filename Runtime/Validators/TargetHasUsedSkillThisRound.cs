using DarkBestiary.Components;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class TargetHasUsedSkillThisRound : Validator
    {
        public TargetHasUsedSkillThisRound(ValidatorData data)
        {
        }

        public override bool Validate(GameObject entity, object target)
        {
            var targetAsGameObject = target as GameObject;

            if (targetAsGameObject == null)
            {
                return false;
            }

            return targetAsGameObject.GetComponent<SpellbookComponent>().LastUsedSkillThisRound != null;
        }
    }
}