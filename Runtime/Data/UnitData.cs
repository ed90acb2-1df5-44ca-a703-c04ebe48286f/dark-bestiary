using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class UnitData : Identity<int>
    {
        public string Label;
        public string NameKey;
        public string DescriptionKey;
        public string Model;
        public string Corpse;
        public int BehaviourTreeId;
        public int ArchetypeId;
        public int ChallengeRating;
        public UnitFlags Flags;
        public ArmorSound ArmorSound;
        public EnvironmentData Environment = new();
        public List<int> Loot = new();
        public List<int> Behaviours = new();
        public List<int> Skills = new();
        public List<int> Equipment = new();
    }
}