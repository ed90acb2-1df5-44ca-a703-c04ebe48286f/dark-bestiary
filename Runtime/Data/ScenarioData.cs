using System;
using System.Collections.Generic;
using DarkBestiary.Scenarios.Encounters;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ScenarioData : Identity<int>
    {
        public int Index;
        public string NameKey;
        public string DescriptionKey;
        public string CommentaryKey;
        public string CompleteKey;
        public float PositionX;
        public float PositionY;
        public int MaxMonsterLevel;
        public int MinMonsterLevel;
        public bool IsUnlocked;
        public bool IsDisposable;
        public bool IsAscension;
        public bool IsDepths;
        public bool IsStart;
        public bool IsEnd;
        public List<int> Children = new();
        public List<EpisodeData> Episodes = new();
        public List<ScenarioRewardData> Rewards = new();
    }

    [Serializable]
    public class EpisodeData
    {
        public int EnvironmentId;
        public List<int> Scenes = new();
        public EncounterData Encounter = new();
    }

    [Serializable]
    public class EncounterData
    {
        public EncounterType Type;
        public EncounterUnitSourceType UnitSourceType;
        public int UnitGroupChallengeRating;
        public int UnitGroupEnvironmentId;
        public int UnitGroupBonusLevel;
        public int StartPhraseId;
        public int CompletePhraseId;
        public UnitTableData UnitTable = new();
        public List<int> UnitGroups = new();
        public List<int> Phrases = new();
    }

    [Serializable]
    public class UnitTableData
    {
        public int Count;
        public List<UnitTableUnitData> Units = new();
    }

    [Serializable]
    public class UnitTableUnitData
    {
        public int UnitId;
        public float Probability;
        public bool IsEnabled;
        public bool IsGuaranteed;
        public bool IsUnique;
    }

    [Serializable]
    public class ScenarioRewardData
    {
        public int ItemId;
        public int StackCount;
        public bool IsChoosable;
    }
}