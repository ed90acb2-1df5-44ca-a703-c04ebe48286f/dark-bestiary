using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public abstract class FormulaBasedEffect : Effect
    {
        private readonly string formula;

        protected FormulaBasedEffect(FormulaBasedEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.formula = data.Formula;
        }

        protected float EvaluateFormula(GameObject caster, GameObject target)
        {
            return Formula.Evaluate(this.formula, caster, target, Skill);
        }
    }
}