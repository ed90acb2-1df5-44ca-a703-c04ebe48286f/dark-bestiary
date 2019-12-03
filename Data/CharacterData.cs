using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class CharacterData : Identity<int>
    {
        public int UnitId;
        public string Name;
        public int Level;
        public int Rerolls;
        public int Experience;
        public bool IsDead;
        public bool IsHardcore;
        public bool IsRandomSkills;
        public bool IsStartScenarioCompleted;
        public bool IsHelmHidden;
        public int HairstyleIndex;
        public int HairColorIndex;
        public int SkinColorIndex;
        public int BeardIndex;
        public int BeardColorIndex;
        public long Timestamp;
        public CharacterRelicData Relics = new CharacterRelicData();
        public List<int> AvailableScenarios = new List<int>();
        public List<int> CompletedScenarios = new List<int>();
        public List<int> UnlockedSkills = new List<int>();
        public List<int> UnlockedRecipes = new List<int>();
        public List<int> UnlockedMonsters = new List<int>();
        public List<int> ReadDialogues = new List<int>();
        public List<CharacterActiveSkillData> ActiveSkills = new List<CharacterActiveSkillData>();
        public List<ItemSaveData> Items = new List<ItemSaveData>();
        public List<ItemSaveData> Equipment = new List<ItemSaveData>();
        public List<ItemSaveData> AltWeapon = new List<ItemSaveData>();
        public List<MasterySaveData> Masteries = new List<MasterySaveData>();
        public List<CurrencyAmountData> Currencies = new List<CurrencyAmountData>();
        public CharacterAttributeData Attributes = new CharacterAttributeData();
        public CharacterTalentsData Talents = new CharacterTalentsData();
    }

    [Serializable]
    public class CharacterRelicData
    {
        public List<RelicSaveData> Active = new List<RelicSaveData>();
        public List<RelicSaveData> Available = new List<RelicSaveData>();
    }

    [Serializable]
    public class CharacterAttributeData
    {
        public int Points;
        public List<SpentAttributePointsData> Attributes = new List<SpentAttributePointsData>();
    }

    [Serializable]
    public class SpentAttributePointsData
    {
        public int AttributeId;
        public int Points;
    }

    [Serializable]
    public class CharacterTalentsData
    {
        public int Points;
        public List<int> Learned = new List<int>();
    }

    [Serializable]
    public class CharacterActiveSkillData
    {
        public int Index;
        public int SkillId;

        public CharacterActiveSkillData()
        {
        }

        public CharacterActiveSkillData(int index, int skillId)
        {
            this.Index = index;
            this.SkillId = skillId;
        }
    }
}