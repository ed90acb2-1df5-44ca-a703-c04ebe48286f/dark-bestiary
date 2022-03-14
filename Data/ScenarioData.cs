using System;
using System.Collections.Generic;
using DarkBestiary.Scenarios;
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
        public ScenarioType Type;
        public bool IsUnlocked;
        public bool IsDisposable;
        public bool IsAscension;
        public bool IsDepths;
        public bool IsHidden;
        public bool IsStart;
        public bool IsEnd;
        public List<int> Children = new List<int>();
        public List<EpisodeData> Episodes = new List<EpisodeData>();
        public List<ScenarioRewardData> Rewards = new List<ScenarioRewardData>();
    }

    [Serializable]
    public class EpisodeData
    {
        public int EnvironmentId;
        public List<int> Scenes = new List<int>();
        public EncounterData Encounter = new EncounterData();
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
        public UnitTableData UnitTable = new UnitTableData();
        public List<int> UnitGroups = new List<int>();
        public List<int> Phrases = new List<int>();
    }

    [Serializable]
    public class UnitTableData
    {
        public int Count;
        public List<UnitTableUnitData> Units = new List<UnitTableUnitData>();
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