using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Map;

namespace DarkBestiary.Data
{
    [Serializable]
    public class MapEncounterData : Identity<int>
    {
        public string LabelKey;
        public string NameKey;
        public string DescriptionKey;
        public MapEncounterType Type;
        public string Icon;
        public string Label;
        public string Sound;
        public string Prefab;
        public float Probability;
        public bool IsFinal;
        public bool IsGuaranteed;
        public bool IsEnabled;
        public bool IsUnique;
        public int UnitId;
        public int RarityId;
        public int LootId;
        public int ScenarioId;
        public List<int> Encounters;

        public MapEncounterData()
        {
        }

        public MapEncounterData(MapEncounterData encounterData)
        {
            Id = encounterData.Id;
            LabelKey = encounterData.LabelKey;
            NameKey = encounterData.NameKey;
            DescriptionKey = encounterData.DescriptionKey;
            Type = encounterData.Type;
            Icon = encounterData.Icon;
            Label = encounterData.Label;
            Sound = encounterData.Sound;
            Probability = encounterData.Probability;
            IsFinal = encounterData.IsFinal;
            IsGuaranteed = encounterData.IsGuaranteed;
            IsEnabled = encounterData.IsEnabled;
            IsUnique = encounterData.IsUnique;
            UnitId = encounterData.UnitId;
            RarityId = encounterData.RarityId;
            LootId = encounterData.LootId;
            ScenarioId = encounterData.ScenarioId;
            Encounters = encounterData.Encounters.ToList();
        }
    }
}