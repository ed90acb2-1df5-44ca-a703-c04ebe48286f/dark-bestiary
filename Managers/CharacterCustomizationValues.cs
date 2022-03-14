using System.Collections.Generic;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CharacterCustomizationValues : Singleton<CharacterCustomizationValues>
    {
        public List<SpriteGroup> Hairstyles => this.hairstyles;
        public List<SpriteGroup> Beards => this.beards;
        public List<Color> SkinColors => this.skinColors;
        public List<Color> HairColors => this.hairColors;

        [Header("Customization")]
        [SerializeField] private List<SpriteGroup> hairstyles;
        [SerializeField] private List<SpriteGroup> beards;
        [SerializeField] private List<Color> skinColors;
        [SerializeField] private List<Color> hairColors;

        [Header("Weapon Sharpening")]
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningBase;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningAxe1h;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningAxe2h;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningSword1h;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningSword2h;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningStaff;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningWand;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningCrossbow;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningDagger;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningGun2h;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningScythe;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningSickle;
        [SerializeField] private WeaponSharpeningParticles weaponSharpeningClaws;

        public WeaponSharpeningParticles GetWeaponSharpeningParticles(Item item)
        {
            switch (item.Type.Type)
            {
                case ItemTypeType.OneHandedAxe:
                case ItemTypeType.OneHandedMace:
                    return weaponSharpeningAxe1h;
                case ItemTypeType.OneHandedSword:
                case ItemTypeType.OneHandedSpear:
                    return this.weaponSharpeningSword1h;
                case ItemTypeType.TwoHandedAxe:
                case ItemTypeType.TwoHandedMace:
                    return this.weaponSharpeningAxe2h;
                case ItemTypeType.TwoHandedSpear:
                case ItemTypeType.TwoHandedSword:
                    return this.weaponSharpeningSword2h;
                case ItemTypeType.Crossbow:
                    return this.weaponSharpeningCrossbow;
                case ItemTypeType.Dagger:
                    return this.weaponSharpeningDagger;
                case ItemTypeType.Claws:
                case ItemTypeType.Katar:
                    return this.weaponSharpeningClaws;
                case ItemTypeType.Scythe:
                    return this.weaponSharpeningScythe;
                case ItemTypeType.Sickle:
                    return this.weaponSharpeningSickle;
                case ItemTypeType.CombatStaff:
                case ItemTypeType.MagicStaff:
                    return this.weaponSharpeningStaff;
                case ItemTypeType.Rifle:
                    return this.weaponSharpeningGun2h;
                case ItemTypeType.Wand:
                    return this.weaponSharpeningWand;
            }

            return this.weaponSharpeningBase;
        }
    }
}