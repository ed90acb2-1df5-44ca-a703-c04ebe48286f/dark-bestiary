using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CombineAndValidator : Validator
    {
        private readonly List<Validator> m_Validators;

        public CombineAndValidator(CombineValidatorsData data, IValidatorRepository validatorRepository)
        {
            m_Validators = validatorRepository.Find(data.Validators);
        }

        public override bool Validate(GameObject caster, object target)
        {
            return m_Validators.All(v => v.Validate(caster, target));
        }
    }
}