using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Behaviours;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;

namespace DarkBestiary.Data.Mappers
{
    public class BehaviourMapper : Mapper<BehaviourData, Behaviour>
    {
        private static readonly Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        static BehaviourMapper()
        {
            Assembly.GetAssembly(typeof(Behaviour))
                    .GetTypes()
                    .Where(type => type.IsClass && type.IsSubclassOf(typeof(Behaviour)) && !type.IsAbstract)
                    .ToList()
                    .ForEach(type => Mapping.Add(type.Name, type));
        }

        private readonly IValidatorRepository validatorRepository;

        public BehaviourMapper(IValidatorRepository validatorRepository)
        {
            this.validatorRepository = validatorRepository;
        }

        public override BehaviourData ToData(Behaviour target)
        {
            throw new NotImplementedException();
        }

        public override Behaviour ToEntity(BehaviourData data)
        {
            if (!Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown behaviour type " + data.Type);
            }

            return Container.Instance.Instantiate(
                Mapping[data.Type],
                new object[]
                {
                    data,
                    CreateValidators(data)
                }) as Behaviour;
        }

        private List<ValidatorWithPurpose> CreateValidators(BehaviourData behaviour)
        {
            return behaviour.Validators
                .Select(validator =>
                    new ValidatorWithPurpose(
                        this.validatorRepository.Find(validator.ValidatorId), validator.ValidatorPurpose))
                .ToList();
        }
    }
}