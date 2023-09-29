using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items
{
    public class ItemCategory
    {
        public static readonly ItemCategory s_All;
        public static readonly ItemCategory s_Runes;
        public static readonly ItemCategory s_Gems;
        public static readonly ItemCategory s_Weapon;
        public static readonly ItemCategory s_MeleeWeapon;
        public static readonly ItemCategory s_RangedWeapon;
        public static readonly ItemCategory s_SlashingMeleeWeapon;
        public static readonly ItemCategory s_CrushingMeleeWeapon;
        public static readonly ItemCategory s_PiercingMeleeWeapon;
        public static readonly ItemCategory s_OneHandedWeapon;
        public static readonly ItemCategory s_TwoHandedWeapon;
        public static readonly ItemCategory s_TwoHandedRangedWeapon;
        public static readonly ItemCategory s_TwoHandedMeleeWeapon;
        public static readonly ItemCategory s_Armor;
        public static readonly ItemCategory s_LightArmor;
        public static readonly ItemCategory s_MediumArmor;
        public static readonly ItemCategory s_HeavyArmor;
        public static readonly ItemCategory s_ChestArmor;
        public static readonly ItemCategory s_Jewelry;
        public static readonly ItemCategory s_Ingredients;

        static ItemCategory()
        {
            var categoryRepository = Container.Instance.Resolve<IItemCategoryRepository>();

            s_All = new ItemCategory(-1, I18N.Instance.Get("ui_all"), ItemCategoryType.All, new List<ItemType>());
            s_Gems = categoryRepository.FindByType(ItemCategoryType.Gems);
            s_Runes = categoryRepository.FindByType(ItemCategoryType.Runes);
            s_Weapon = categoryRepository.FindByType(ItemCategoryType.Weapon);
            s_MeleeWeapon = categoryRepository.FindByType(ItemCategoryType.MeleeWeapon);
            s_RangedWeapon = categoryRepository.FindByType(ItemCategoryType.RangedWeapon);
            s_OneHandedWeapon = categoryRepository.FindByType(ItemCategoryType.OneHandedWeapon);
            s_TwoHandedWeapon = categoryRepository.FindByType(ItemCategoryType.TwoHandedWeapon);
            s_TwoHandedRangedWeapon = categoryRepository.FindByType(ItemCategoryType.TwoHandedRangedWeapon);
            s_TwoHandedMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.TwoHandedMeleeWeapon);
            s_SlashingMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.SlashingMeleeWeapon);
            s_CrushingMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.CrushingMeleeWeapon);
            s_PiercingMeleeWeapon = categoryRepository.FindByType(ItemCategoryType.PiercingMeleeWeapon);
            s_Armor = categoryRepository.FindByType(ItemCategoryType.Armor);
            s_LightArmor = categoryRepository.FindByType(ItemCategoryType.LightArmor);
            s_MediumArmor = categoryRepository.FindByType(ItemCategoryType.MediumArmor);
            s_HeavyArmor = categoryRepository.FindByType(ItemCategoryType.HeavyArmor);
            s_ChestArmor = categoryRepository.FindByType(ItemCategoryType.ChestArmor);
            s_Jewelry = categoryRepository.FindByType(ItemCategoryType.Jewelry);
            s_Ingredients = categoryRepository.FindByType(ItemCategoryType.Ingredients);
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