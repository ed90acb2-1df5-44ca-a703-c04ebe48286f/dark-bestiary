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
        public int Experience;
        public bool IsHelmHidden;
        public int HairstyleIndex;
        public int HairColorIndex;
        public int SkinColorIndex;
        public int BeardIndex;
        public int BeardColorIndex;
        public long Timestamp;
        public MapSaveData? Map;
        public CharacterRelicData Relics = new();
        public List<int> UnlockedMonsters = new();
        public List<CharacterActiveSkillData> ActiveSkills = new();
        public List<ItemSaveData> Items = new();
        public List<ItemSaveData> Equipment = new();
        public List<ItemSaveData> AltWeapon = new();
        public List<MasterySaveData> Masteries = new();
        public List<CurrencyAmountData> Currencies = new();
        public CharacterAttributeData Attributes = new();
        public CharacterTalentsData Talents = new();
    }

    [Serializable]
    public class CharacterRelicData
    {
        public List<RelicSaveData> Active = new();
        public List<RelicSaveData> Available = new();
    }

    [Serializable]
    public class CharacterAttributeData
    {
        public int Points;
        public List<SpentAttributePointsData> Attributes = new();
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
        public List<int> Learned = new();
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
            Index = index;
            SkillId = skillId;
        }
    }
}