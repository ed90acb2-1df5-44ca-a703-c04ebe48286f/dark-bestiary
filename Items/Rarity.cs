using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Items
{
    public class Rarity
    {
        public int Id { get; }
        public I18NString Name { get; }
        public RarityType Type { get; }
        public string ColorCode { get; }

        public Rarity(int id, I18NString name, RarityType type, string colorCode)
        {
            Id = id;
            Name = name;
            Type = type;
            ColorCode = colorCode;
        }

        public Color Color()
        {
            return ColorCode.ToColor();
        }
    }
}