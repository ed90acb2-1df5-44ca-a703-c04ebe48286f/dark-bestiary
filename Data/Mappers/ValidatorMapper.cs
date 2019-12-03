using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Validators;

namespace DarkBestiary.Data.Mappers
{
    public class ValidatorMapper : Mapper<ValidatorData, Validator>
    {
        private static readonly Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        static ValidatorMapper()
        {
            Assembly.GetAssembly(typeof(Validator))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Validator)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => Mapping.Add(type.Name, type));
        }

        public override ValidatorData ToData(Validator target)
        {
            throw new NotImplementedException();
        }

        public override Validator ToEntity(ValidatorData data)
        {
            if (!Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown validator type " + data.Type);
            }

            return Container.Instance.Instantiate(Mapping[data.Type], new[] {data}) as Validator;
        }
    }
}