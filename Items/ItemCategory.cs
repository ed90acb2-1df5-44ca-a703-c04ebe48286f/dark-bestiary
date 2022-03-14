using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items
{
    public class ItemCategory
    {
        public static readonly ItemCategory All;
        public static readonly ItemCategory Runes;
        public static readonly ItemCategory Gems;
        public static readonly ItemCategory Weapon;
        public static readonly ItemCategory MeleeWeapon;
        public static readonly ItemCategory RangedWeapon;
        public static readonly ItemCategory SlashingMeleeWeapon;
        public static readonly ItemCategory CrushingMeleeWeapon;
        public static readonly ItemCategory PiercingMeleeWeapon;
        public static readonly ItemCategory OneHandedWeapon;
        public static readonly ItemCategory TwoHandedWeapon;
        public static readonly ItemCategory TwoHandedRangedWeapon;
        public static readonly ItemCategory TwoHandedMeleeWeapon;
        public static readonly ItemCategory Armor;
        public static readonly ItemCategory LightArmor;
        public static readonly ItemCategory MediumArmor;
        public static readonly ItemCategory HeavyArmor;
        public static readonly ItemCategory ChestArmor;
        public static readonly ItemCategory Jewelry;
        public static readonly ItemCategory Ingredients;

        static ItemCategory()
        {
            var categoryRepository = Container.Instance.Resolve<IItemCategoryRepository>();

            All = new ItemCategory(-1, I18N.Instance.Get("ui_all"), ItemCategoryType.All, new List<ItemType>());
            Gems = categoryRepository.FindByType(ItemCategoryType.Gems);
            Runes = categoryRepository.FindByType(ItemCategoryType.Runes);
            Weapon = categoryRepository.FindByType(ItemCategoryType.Weapon);
            MeleeWeapon = categoryRepository.FindByType(ItemCategoryType.MeleeWeapon);
            RangedWeapon = categoryRepository.FindByType(ItemCategoryType.RangedWeapon);
            OneHandedWeapon = categoryRepository.FindByType(ItemCategoryType.OneHandedWeapon);
            TwoHandedWeapon = categoryRepository.FindByType(ItemCategoryType.TwoHandedWeapon);
            TwoHandedRangedWeapon = categoryRepository.FindByType(ItemCategoryType.TwoHandedRangedWeapon);
            TwoHandedMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.TwoHandedMeleeWeapon);
            SlashingMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.SlashingMeleeWeapon);
            CrushingMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.CrushingMeleeWeapon);
            PiercingMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.PiercingMeleeWeapon);
            Armor = categoryRepository.FindByType(ItemCategoryType.Armor);
            LightArmor = categoryRepository.FindByType(ItemCategoryType.LightArmor);
            MediumArmor = categoryRepository.FindByType(ItemCategoryType.MediumArmor);
            HeavyArmor = categoryRepository.FindByType(ItemCategoryType.HeavyArmor);
            ChestArmor = categoryRepository.FindByType(ItemCategoryType.ChestArmor);
            Jewelry = categoryRepository.FindByType(ItemCategoryType.Jewelry);
            Ingredients = categoryRepository.FindByType(ItemCategoryType.Ingredients);
        }

        public int Id { get; }
        public I18NString Name { get; }
        public ItemCategoryType Type { get; }
        public List<ItemType> ItemTypes { get; }

        public ItemCategory(int id, I18NString name, ItemCategoryType type, List<ItemType> itemTypes)
        {
            Id = id;
            Name = name;
            Type = type;
            ItemTypes = itemTypes;
        }

        public bool Contains(ItemTypeType itemType)
        {
            if (Type == ItemCategoryType.All)
            {
                return true;
            }

            return ItemTypes.Any(element => element.Type == itemType);
        }

        public bool Contains(int typeId)
        {
            if (Type == ItemCategoryType.All)
            {
                return true;
            }

            return ItemTypes.Any(element => element.Id == typeId);
        }

        public bool Contains(ItemType itemType)
        {
            if (Type == ItemCategoryType.All)
            {
                return true;
            }

            if (itemType == null)
            {
                return false;
            }

            return ItemTypes.Any(element => element.Id == itemType.Id);
        }
    }
}