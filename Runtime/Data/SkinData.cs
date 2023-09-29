using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SkinData : Identity<int>
    {
        public List<SkinPartData> Parts = new();
    }
}