using System;
using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemData : Identity<int>
    {
        public bool IsEnabled;
        public string NameKey;
        public string LoreKey;
        public string BookTextKey;
        public string ConsumeDescriptionKey;
        public string PassiveDescriptionKey;
        public string Icon;
        public ItemFlags Flags;
        public int StackSize;
        public int SocketCount;
        public int SkinId;
        public int WeaponSkillAId;
        public int WeaponSkillBId;
        public int UnlockRelicId;
        public int TypeId;
        public int RarityId;
        public int SuffixId;
        public int SetId;
        public int ConsumeCooldown;
        public int ConsumeLootId;
        public int ConsumeEffectId;
        public int EnchantmentBehaviourId;
        public int EnchantmentItemCategoryId;
        public string ConsumeSound;
        public CurrencyType CurrencyType;
        public List<AttachmentInfo> Attachments;
        public List<AttributeModifierData> AttributeModifiers = new();
        public List<PropertyModifierData> PropertyModifiers = new();
        public List<PriceData> Price = new();
        public List<int> FixedModifiers = new();
        public List<int> Modifiers = new();
        public List<int> Behaviours = new();
        public List<int> Categories = new();
        public EquipmentSlotType Slot;
    }
}