using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public abstract class FormulaBasedEffect : Effect
    {
        private readonly string m_Formula;

        protected FormulaBasedEffect(FormulaBasedEffectData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Formula = data.Formula;
        }

        protected float EvaluateFormula(GameObject caster, GameObject target)
        {
            return Formula.Evaluate(m_Formula, caster, target, Skill);
        }
    }
}