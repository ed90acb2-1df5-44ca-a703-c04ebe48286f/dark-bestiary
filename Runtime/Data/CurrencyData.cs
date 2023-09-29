using System;
using DarkBestiary.Currencies;

namespace DarkBestiary.Data
{
    [Serializable]
    public class CurrencyData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public string Icon;
        public CurrencyType Type;
    }
}