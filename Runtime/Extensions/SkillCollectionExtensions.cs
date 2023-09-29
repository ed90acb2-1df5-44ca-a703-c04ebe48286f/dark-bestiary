using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Skills;

namespace DarkBestiary.Extensions
{
    public static class SkillCollectionExtensions
    {
        public static IEnumerable<SkillData> Learnable(this IEnumerable<SkillData> collection, Func<SkillData, bool> predicate)
        {
            return collection.Learnable().Where(predicate);
        }

        public static IEnumerable<SkillData> Learnable(this IEnumerable<SkillData> collection)
        {
            return collection.Where(s => Skill.IsLearnable(s.Type, s.Flags));
        }
    }
}