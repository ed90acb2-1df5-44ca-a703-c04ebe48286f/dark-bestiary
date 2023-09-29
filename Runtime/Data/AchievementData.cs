using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AchievementData : Identity<int>
    {
        public bool IsEnabled;
        public int Index;
        public string NameKey;
        public string DescriptionKey;
        public string SteamApiKey;
        public string Type;
        public string Icon;
        public int RequiredQuantity;
        public int Level;
        public int UnitId;
        public int ItemId;
        public int ScenarioId;
        public List<AchievementConditionData> Conditions = new();
    }
}