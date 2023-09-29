using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AchievementsSaveData
    {
        public List<AchievementStatusData> AchievementStatuses = new();
    }

    [Serializable]
    public class AchievementStatusData
    {
        public int AchievementId;
        public int Quantity;
        public bool IsUnlocked;
        public long UnlockedAt;
    }
}