using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemSaveData
    {
        public int ItemId;
        public int StackCount;
        public int ForgeLevel;
        public int SuffixId;
        public float SuffixQuality = 1.0f;
        public List<int> Sockets = new List<int>();

        public bool IsIdenticalTo(ItemSaveData other)
        {
            return this.ItemId == other.ItemId &&
                   this.StackCount == other.StackCount &&
                   this.ForgeLevel == other.ForgeLevel &&
                   this.SuffixId == other.SuffixId &&
                   Mathf.Approximately(this.SuffixQuality, other.SuffixQuality) &&
                   this.Sockets.Count == other.Sockets.Count &&
                   this.Sockets.All(id => other.Sockets.Contains(id));
        }
    }
}