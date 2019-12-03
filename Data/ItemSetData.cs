using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemSetData : Identity<int>
    {
        public string NameKey;
        public List<int> Items;
        public List<ItemSetBehaviourData> Behaviours = new List<ItemSetBehaviourData>();
    }

    [Serializable]
    public class ItemSetBehaviourData
    {
        public int ItemCount;
        public List<int> Behaviours;
    }
}