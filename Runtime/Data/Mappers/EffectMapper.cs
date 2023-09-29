using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class EffectMapper : Mapper<EffectData, Effect>
    {
        private readonly IValidatorRepository m_ValidatorRepository;
        private static readonly Dictionary<string, Type> s_Mapping = new();

        static EffectMapper()
        {
            Assembly.GetAssembly(typeof(Effect))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => s_Mapping.Add(type.Name, type));
        }

        public EffectMapper(IValidatorRepository validatorRepository)
        {
            m_ValidatorRepository = validatorRepository;
        }

        public override EffectData ToData(Effect effect)
        {
            throw new NotImplementedException();
        }

        public override Effect ToEntity(EffectData data)
        {
            if (!s_Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown effect type " + data.Type);
            }

            try
            {
                return Container.Instance.Instantiate(
                    s_Mapping[data.Type],
                    new object[]
                    {
                        data,
                        CreateValidators(data),
                    }) as Effect;
                ;
            }
            catch (Exception exception)
            {
                Debug.LogError(data.Name + ": " + exception.Message);
                throw;
            }
        }

        private List<ValidatorWithPurpose> CreateValidators(EffectData effect)
        {
            return effect.Validators
                .Select(validator =>
                    new ValidatorWithPurpose(
                        m_ValidatorRepository.Find(validator.ValidatorId), validator.ValidatorPurpose))
                .ToList();
        }
    }
}