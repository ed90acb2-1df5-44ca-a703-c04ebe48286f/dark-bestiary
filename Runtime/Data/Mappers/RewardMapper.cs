using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Rewards;

namespace DarkBestiary.Data.Mappers
{
    public class RewardMapper : Mapper<RewardData, Reward>
    {
        private static readonly Dictionary<string, Type> s_Mapping = new();

        static RewardMapper()
        {
            Assembly.GetAssembly(typeof(Reward))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Reward)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => s_Mapping.Add(type.Name, type));
        }

        public override RewardData ToData(Reward entity)
        {
            throw new NotImplementedException();
        }

        public override Reward ToEntity(RewardData data)
        {
            if (!s_Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown reward type " + data.Type);
            }

            return Container.Instance.Instantiate(s_Mapping[data.Type], new[] {data}) as Reward;
        }
    }
}