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
        public int Level;
        public int RequiredLevel;
        public int StackSize;
        public int SocketCount;
        public int SkinId;
        public int WeaponSkillAId;
        public int WeaponSkillBId;
        public int LearnSkillId;
        public int UnlockSkillId;
        public int UnlockRelicId;
        public int UnlockScenarioId;
        public int BlueprintRecipeId;
        public int BlueprintRecipeItemTypeId;
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
        public List<AttributeModifierData> AttributeModifiers = new List<AttributeModifierData>();
        public List<PropertyModifierData> PropertyModifiers = new List<PropertyModifierData>();
        public List<PriceData> Price = new List<PriceData>();
        public List<int> FixedModifiers = new List<int>();
        public List<int> Modifiers = new List<int>();
        public List<int> Behaviours = new List<int>();
        public EquipmentSlotType Slot;
    }
}