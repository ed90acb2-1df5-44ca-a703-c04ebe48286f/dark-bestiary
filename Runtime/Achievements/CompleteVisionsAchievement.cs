using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Data;

namespace DarkBestiary.Achievements
{
    public class CompleteVisionsAchievement : Achievement
    {
        public CompleteVisionsAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }
    }
}