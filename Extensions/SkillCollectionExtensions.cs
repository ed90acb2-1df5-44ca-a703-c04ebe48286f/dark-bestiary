using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Skills;

namespace DarkBestiary.Extensions
{
    public static class SkillCollectionExtensions
    {
        public static IEnumerable<SkillData> Tradable(
            this IEnumerable<SkillData> collection, Func<SkillData, bool> predicate)
        {
            return collection.Tradable().Where(predicate);
        }

        public static IEnumerable<SkillData> Tradable(this IEnumerable<SkillData> collection)
        {
            return collection.Where(s => Skill.IsTradable(s.Type, s.Flags));
        }

        public static IEnumerable<Skill> Tradable(
            this IEnumerable<Skill> collection, Func<Skill, bool> predicate)
        {
            return collection.Tradable().Where(predicate);
        }

        public static IEnumerable<Skill> Tradable(this IEnumerable<Skill> collection)
        {
            return collection.Where(s => s.IsTradable());
        }
    }
}