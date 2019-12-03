using System;

namespace DarkBestiary.Skills.Targeting
{
    public static class SkillTargetingFactory
    {
        public static ISkillUseStrategy Make(SkillTargetType targetType)
        {
            switch (targetType)
            {
                case SkillTargetType.None:
                    return new NoneSkillUseStrategy();
                case SkillTargetType.Unit:
                    return new UnitSkillUseStrategy();
                case SkillTargetType.AllyUnit:
                    return new AllyUnitSkillUseStrategy();
                case SkillTargetType.EnemyUnit:
                    return new EnemyUnitSkillUseStrategy();
                case SkillTargetType.Point:
                    return new PointSkillUseStrategy();
                case SkillTargetType.Unoccupied:
                    return new UnoccupiedSkillUseStrategy();
                case SkillTargetType.Corpse:
                    return new CorpseSkillUseStrategy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
            }
        }
    }
}