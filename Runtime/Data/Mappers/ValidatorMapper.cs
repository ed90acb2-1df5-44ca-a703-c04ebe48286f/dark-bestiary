using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Validators;

namespace DarkBestiary.Data.Mappers
{
    public class ValidatorMapper : Mapper<ValidatorData, Validator>
    {
        private static readonly Dictionary<string, Type> s_Mapping = new();

        static ValidatorMapper()
        {
            Assembly.GetAssembly(typeof(Validator))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Validator)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => s_Mapping.Add(type.Name, type));
        }

        public override ValidatorData ToData(Validator target)
        {
            throw new NotImplementedException();
        }

        public override Validator ToEntity(ValidatorData data)
        {
            if (!s_Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown validator type " + data.Type);
            }

            return Container.Instance.Instantiate(s_Mapping[data.Type], new[] {data}) as Validator;
        }
    }
}