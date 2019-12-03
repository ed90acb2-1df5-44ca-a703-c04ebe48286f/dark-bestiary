using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Rewards;

namespace DarkBestiary.Data.Mappers
{
    public class RewardMapper : Mapper<RewardData, Reward>
    {
        private static readonly Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        static RewardMapper()
        {
            Assembly.GetAssembly(typeof(Reward))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Reward)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => Mapping.Add(type.Name, type));
        }

        public override RewardData ToData(Reward entity)
        {
            throw new System.NotImplementedException();
        }

        public override Reward ToEntity(RewardData data)
        {
            if (!Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown reward type " + data.Type);
            }

            return Container.Instance.Instantiate(Mapping[data.Type], new[] {data}) as Reward;
        }
    }
}