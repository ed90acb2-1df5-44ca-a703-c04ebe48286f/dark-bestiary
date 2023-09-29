using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class StashData
    {
        public List<ItemSaveData> Items = new();
    }
}