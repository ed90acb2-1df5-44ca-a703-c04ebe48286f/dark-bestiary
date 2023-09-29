using System.Collections.Generic;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CharacterCustomizationValues : Singleton<CharacterCustomizationValues>
    {
        public List<SpriteGroup> Hairstyles => m_Hairstyles;
        public List<SpriteGroup> Beards => m_Beards;
        public List<Color> SkinColors => m_SkinColors;
        public List<Color> HairColors => m_HairColors;


        [Header("Customization")]
        [SerializeField]
        private List<SpriteGroup> m_Hairstyles = null!;

        [SerializeField]
        private List<SpriteGroup> m_Beards = null!;

        [SerializeField]
        private List<Color> m_SkinColors = null!;

        [SerializeField]
        private List<Color> m_HairColors = null!;
    }
}