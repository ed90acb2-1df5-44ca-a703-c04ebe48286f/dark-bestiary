using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class RewardData : Identity<int>
    {
        public string Type;
    }

    [Serializable]
    public class RandomSkillsUnlockRewardData : RewardData
    {
        public int Count;
    }

    [Serializable]
    public class TalentPointsRewardData : RewardData
    {
        public int Count;
    }

    [Serializable]
    public class AttributePointsRewardData : RewardData
    {
        public int Count;
    }

    [Serializable]
    public class RewardCollectionData : RewardData
    {
        public List<int> Rewards;
        public bool IsChoosable;
    }

    [Serializable]
    public class LevelupRewardData : RewardCollectionData
    {
        public int Level;
    }

    [Serializable]
    public class ItemsRewardData : RewardData
    {
        public List<ItemAmountData> Items = new List<ItemAmountData>();
    }

    [Serializable]
    public class CurrenciesRewardData : RewardData
    {
        public List<CurrencyAmountData> Currencies = new List<CurrencyAmountData>();
    }

    [Serializable]
    public class AttributesRewardData : RewardData
    {
        public List<AttributeAmountData> Attributes = new List<AttributeAmountData>();
    }

    [Serializable]
    public class PropertiesRewardData : RewardData
    {
        public List<PropertyAmountData> Properties = new List<PropertyAmountData>();
    }
}