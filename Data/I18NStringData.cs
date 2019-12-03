using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class I18NStringData : Identity<string>
    {
        public List<I18NStringVariableData> Variables = new List<I18NStringVariableData>();

        public I18NStringData()
        {
        }

        public I18NStringData(string id)
        {
            this.Id = id;
        }
    }
}