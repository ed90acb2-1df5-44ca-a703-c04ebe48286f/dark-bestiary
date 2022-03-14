using System;
using System.Collections.Generic;
using DarkBestiary.Scenarios;
using DarkBestiary.Visions;

namespace DarkBestiary.Data
{
    [Serializable]
    public class VisionMapSaveData
    {
        public float Sanity;
        public int CurrentAct;
        public int LastCompletedVisionIndex;
        public VisionData FinalVisionData;
        public Summary Summary;
        public List<VisionSaveData> Visions = new List<VisionSaveData>();
        public List<VisionSkillSaveData> Skills = new List<VisionSkillSaveData>();
        public List<int> TalentBehaviours = new List<int>();
        public List<VisionBehaviourSaveData> TemporaryBehaviours = new List<VisionBehaviourSaveData>();
        public CharacterData Character;
    }

    [Serializable]
    public class VisionSaveData
    {
        public VisionData VisionData;
        public VisionViewState VisionState;
    }

    [Serializable]
    public class VisionBehaviourSaveData
    {
        public int BehaviourId;
        public int RemainingDuration;
        public int StackCount;
    }

    [Serializable]
    public class VisionSkillSaveData
    {
        public int SkillId;
        public int Cooldown;
    }
}