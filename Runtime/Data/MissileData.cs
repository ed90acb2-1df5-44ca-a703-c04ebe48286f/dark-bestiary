using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class MissileData : Identity<int>
    {
        public string Name;
        public string Prefab;
        public string ImpactPrefab;
    }
}