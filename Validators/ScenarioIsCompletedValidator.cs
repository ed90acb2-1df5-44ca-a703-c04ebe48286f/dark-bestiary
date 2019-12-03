using DarkBestiary.Data;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class ScenarioIsCompletedValidator : Validator
    {
        private readonly ScenarioValidatorData data;
        private readonly CharacterManager characterManager;

        public ScenarioIsCompletedValidator(ScenarioValidatorData data, CharacterManager characterManager)
        {
            this.data = data;
            this.characterManager = characterManager;
        }

        public override bool Validate(GameObject caster, object target)
        {
            return this.characterManager.Character.Data.CompletedScenarios.Contains(this.data.ScenarioId);
        }
    }
}