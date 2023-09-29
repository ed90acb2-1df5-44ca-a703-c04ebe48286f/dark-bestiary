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
        public List<ItemAmountData> Items = new();
    }

    [Serializable]
    public class CurrenciesRewardData : RewardData
    {
        public List<CurrencyAmountData> Currencies = new();
    }

    [Serializable]
    public class AttributesRewardData : RewardData
    {
        public List<AttributeAmountData> Attributes = new();
    }

    [Serializable]
    public class PropertiesRewardData : RewardData
    {
        public List<PropertyAmountData> Properties = new();
    }
}