using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class UnitGroupData : Identity<int>
    {
        public string Label;
        public List<int> Units = new();
    }
}