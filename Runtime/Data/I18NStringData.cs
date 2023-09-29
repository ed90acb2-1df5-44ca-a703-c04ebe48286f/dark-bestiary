using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class I18NStringData : Identity<int>
    {
        public string Key;
        public List<I18NStringVariableData> Variables = new();

        public I18NStringData()
        {
        }

        public I18NStringData(string key)
        {
            Key = key;
        }
    }
}