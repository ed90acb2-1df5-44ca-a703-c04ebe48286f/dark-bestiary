using System;
using System.Collections.Generic;
using DarkBestiary.Skills;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SpecializationSaveData
    {
        public int SpecializationId;
        public int Level;
        public int Experience;
        public List<int> ClaimedRewards = new List<int>();

        public SpecializationSaveData()
        {
        }

        public SpecializationSaveData(Specialization specialization)
        {
            this.SpecializationId = specialization.Data.Id;
            this.Level = specialization.Experience?.Level ?? 0;
            this.Experience = specialization.Experience?.Current ?? 0;
        }
    }
}