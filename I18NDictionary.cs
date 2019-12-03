using System;
using System.Collections.Generic;

namespace DarkBestiary
{
    [Serializable]
    public class I18NDictionary
    {
        public string Name;
        public string DisplayName;
        public Dictionary<string, string> Data = new Dictionary<string, string>();
    }
}