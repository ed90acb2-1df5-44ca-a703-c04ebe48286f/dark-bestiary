using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Achievements;
using DarkBestiary.Achievements.Conditions;

namespace DarkBestiary.Data.Mappers
{
    public class AchievementMapper : Mapper<AchievementData, Achievement>
    {
        private static readonly Dictionary<string, Type> s_AchievementMapping = new();
        private static readonly Dictionary<string, Type> s_ConditionMapping = new();

        static AchievementMapper()
        {
            Assembly.GetAssembly(typeof(Achievement))
                    .GetTypes()
                    .Where(type => type.IsClass && type.IsSubclassOf(typeof(Achievement)) && !type.IsAbstract)
                    .ToList()
                    .ForEach(type => s_AchievementMapping.Add(type.Name, type));

            Assembly.GetAssembly(typeof(AchievementCondition))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(AchievementCondition)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => s_ConditionMapping.Add(type.Name, type));
        }

        public override AchievementData ToData(Achievement target)
        {
            throw new NotImplementedException();
        }

        public override Achievement ToEntity(AchievementData data)
        {
            if (!s_AchievementMapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown achievement type " + data.Type);
            }

            return Container.Instance.Instantiate(
                s_AchievementMapping[data.Type], new object[] {data, GetConditions(data)}) as Achievement;
        }

        private List<AchievementCondition> GetConditions(AchievementData data)
        {
            var conditions = new List<AchievementCondition>();

            foreach (var conditionData in data.Conditions)
            {
                if (!s_ConditionMapping.ContainsKey(conditionData.Type))
                {
                    throw new Exception("Unknown achievement condition type " + data.Type);
                }

                conditions.Add(
                    Container.Instance.Instantiate(
                        s_ConditionMapping[conditionData.Type], new[] {conditionData}) as AchievementCondition);
            }

            return conditions;
        }
    }
}