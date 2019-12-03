using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using UnityEngine;

namespace DarkBestiary.Validators
{
    public class CombineOrValidator : Validator
    {
        private readonly List<Validator> validators;

        public CombineOrValidator(CombineValidatorsData data, IValidatorRepository validatorRepository)
        {
            this.validators = validatorRepository.Find(data.Validators);
        }

        public override bool Validate(GameObject caster, object target)
        {
            return this.validators.Any(v => v.Validate(caster, target));
        }
    }
}