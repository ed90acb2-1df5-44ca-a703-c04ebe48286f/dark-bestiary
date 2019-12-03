using System.Collections.Generic;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CharacterCustomizationValues : Singleton<CharacterCustomizationValues>
    {
        public List<SpriteGroup> Hairstyles => this.hairstyles;
        public List<SpriteGroup> Beards => this.beards;
        public List<Color> SkinColors => this.skinColors;
        public List<Color> HairColors => this.hairColors;

        [SerializeField] private List<SpriteGroup> hairstyles;
        [SerializeField] private List<SpriteGroup> beards;
        [SerializeField] private List<Color> skinColors;
        [SerializeField] private List<Color> hairColors;
    }
}