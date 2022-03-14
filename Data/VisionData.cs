using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Visions;

namespace DarkBestiary.Data
{
    [Serializable]
    public class VisionData : Identity<int>
    {
        public string LabelKey;
        public string NameKey;
        public string DescriptionKey;
        public VisionType Type;
        public string Icon;
        public string Label;
        public string Prefab;
        public string Sound;
        public float Probability;
        public bool IsFinal;
        public bool IsGuaranteed;
        public bool IsEnabled;
        public bool IsUnique;
        public int UnitId;
        public int Sanity;
        public int ActMin;
        public int ActMax;
        public int RarityId;
        public int LootId;
        public int EffectId;
        public int ScenarioId;
        public List<int> Visions;

        public VisionData()
        {
        }

        public VisionData(VisionData data)
        {
            this.Id = data.Id;
            this.LabelKey = data.LabelKey;
            this.NameKey = data.NameKey;
            this.DescriptionKey = data.DescriptionKey;
            this.Type = data.Type;
            this.Icon = data.Icon;
            this.Label = data.Label;
            this.Prefab = data.Prefab;
            this.Sound = data.Sound;
            this.Probability = data.Probability;
            this.IsFinal = data.IsFinal;
            this.IsGuaranteed = data.IsGuaranteed;
            this.IsEnabled = data.IsEnabled;
            this.IsUnique = data.IsUnique;
            this.UnitId = data.UnitId;
            this.Sanity = data.Sanity;
            this.ActMin = data.ActMin;
            this.ActMax = data.ActMax;
            this.RarityId = data.RarityId;
            this.LootId = data.LootId;
            this.EffectId = data.EffectId;
            this.ScenarioId = data.ScenarioId;
            this.Visions = data.Visions.ToList();
        }
    }
}