using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Modifiers;
using DarkBestiary.Properties;
using DarkBestiary.Randomization;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Attribute = DarkBestiary.Attributes.Attribute;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Items
{
    public class Item
    {
        public static Item CreateEmpty()
        {
            return new Item(c_EmptyItemId, new I18NString(new I18NStringData("Empty")), new ItemData(), null);
        }

        public const int c_EmptyItemId = -1;

        public static event Action<Item>? AnyItemStatsUpdated;
        public static event Action<Item>? AnyItemCooldownStarted;

        public event Action? StackCountChanged;
        public event Action<Item>? CooldownStarted;
        public event Action<Item>? CooldownUpdated;
        public event Action<Item>? CooldownFinished;
        public event Action<Item, Item>? SocketInserted;
        public event Action<Item, Item>? Enchanted;

        public int Id { get; }
        public ItemData Data { get; }
        public I18NString BaseName { get; }
        public I18NString Lore { get; private set; }
        public I18NString BookText { get; private set; }
        public I18NString ConsumeDescription { get; private set; }
        public I18NString PassiveDescription { get; private set; }
        public string Icon { get; private set; }
        public string ConsumeSound { get; private set; }
        public int StackCount { get; private set; }
        public int StackCountMax { get; private set; }
        public int StackSpaceRemaining => StackCountMax - StackCount;
        public ItemFlags Flags { get; private set; }
        public EquipmentSlotType SlotType { get; private set; }
        public CurrencyType CurrencyType { get; private set; }
        public List<PropertyModifier> BasePropertyModifiers { get; private set; } = new();
        public List<AttributeModifier> BaseAttributeModifiers { get; private set; } = new();

        public int Cooldown => Data.ConsumeCooldown;
        public int CooldownRemaining { get; private set; }
        public bool IsOnCooldown => Cooldown > 0 && CooldownRemaining > 0;

        public Rarity? Rarity => m_Rarity;
        public ItemType Type { get; private set; }
        public List<Item> Sockets { get; set; } = new();
        public List<Behaviour> Affixes { get; set; } = new();
        public List<Behaviour> Behaviours { get; set; } = new();
        public List<Item> Runes { get; set; } = new();

        public Behaviour? EnchantmentBehaviour { get; set; }
        public ItemCategory? EnchantmentItemCategory { get; set; }

        public EquipmentSlot EquipmentSlot { get; set; }
        public EquipmentComponent Equipment { get; set; }
        public InventoryComponent Inventory { get; set; }
        public ItemModifier? Suffix { get; set; }
        public List<AttachmentInfo> Attachments { get; set; }
        public ItemSet Set { get; set; }

        public Skill? WeaponSkillA
        {
            get => m_WeaponSkillA;
            set
            {
                m_WeaponSkillA = value;

                if (m_WeaponSkillA == null)
                {
                    return;
                }

                m_WeaponSkillA.Weapon = this;
            }
        }

        public Skill? WeaponSkillB
        {
            get => m_WeaponSkillB;
            set
            {
                m_WeaponSkillB = value;

                if (m_WeaponSkillB == null)
                {
                    return;
                }

                m_WeaponSkillB.Weapon = this;
            }
        }

        private Skill? m_WeaponSkillA;
        private Skill? m_WeaponSkillB;

        public Skin Skin { get; set; }
        public Effect ConsumeEffect { get; set; }
        public float PriceMultiplier { get; set; } = 1;
        public bool IsFixedPrice { get; set; }

        public string Name => GetName();
        public string ColoredName => Rarity == null ? Name : $"<color={Rarity.ColorCode}>{Name}</color>";
        public string ColoredBaseName => Rarity == null ? BaseName : $"<color={Rarity.ColorCode}>{BaseName}</color>";

        public bool IsBuyout { get; set; }
        public bool IsMaximumSocketCount => Sockets.Count >= Type.MaxSocketCount;
        public bool IsGambleable => Flags.HasFlag(ItemFlags.Gambleable);
        public bool IsStackable => Flags.HasFlag(ItemFlags.Stackable);
        public bool IsDismantable => Flags.HasFlag(ItemFlags.Dismantable);
        public bool IsUniqueEquipped => Flags.HasFlag(ItemFlags.UniqueEquipped);
        public bool HasRandomSuffix => Flags.HasFlag(ItemFlags.HasRandomSuffix);

        public bool IsRune => Type?.Type == ItemTypeType.Rune;
        public bool IsMinorRune => Type?.Type == ItemTypeType.MinorRune;
        public bool IsMajorRune => Type?.Type == ItemTypeType.MajorRune;
        public bool IsAnyRune => Type?.Type == ItemTypeType.Rune || Type?.Type == ItemTypeType.MajorRune || Type?.Type == ItemTypeType.MinorRune;

        public bool IsEnchantment => Type?.Type == ItemTypeType.Enchantment;
        public bool IsConsumable => Type?.Type == ItemTypeType.Consumable;
        public bool IsRelic => Type?.Type == ItemTypeType.Relic;
        public bool IsIngredient => Type?.Type == ItemTypeType.Ingredient;
        public bool IsStaff => Type?.Type == ItemTypeType.MagicStaff;
        public bool IsCombatStaff => Type?.Type == ItemTypeType.CombatStaff;
        public bool IsRifle => Type?.Type == ItemTypeType.Rifle;
        public bool IsCrossbow => Type?.Type == ItemTypeType.Crossbow;
        public bool IsTwoHandedSpear => Type?.Type == ItemTypeType.TwoHandedSpear;
        public bool IsBow => Type?.Type == ItemTypeType.Bow;
        public bool IsShield => Type?.Type == ItemTypeType.Shield;
        public bool IsBook => Type?.Type == ItemTypeType.Book;
        public bool IsPage => Type?.Type == ItemTypeType.Page;
        public bool IsPotion => Type?.Type == ItemTypeType.Potion;
        public bool IsJunk => Type?.Type == ItemTypeType.Junk;

        public bool IsEquipment => IsArmor || IsWeapon || IsJewelry;
        public bool IsGem => ItemCategory.s_Gems.Contains(Type);
        public bool IsMeleeWeapon => ItemCategory.s_MeleeWeapon.Contains(Type);
        public bool IsRangedWeapon => ItemCategory.s_RangedWeapon.Contains(Type);
        public bool IsWeapon => ItemCategory.s_Weapon.Contains(Type);
        public bool IsArmor => ItemCategory.s_Armor.Contains(Type);
        public bool IsLightArmor => ItemCategory.s_LightArmor.Contains(Type);
        public bool IsMediumArmor => ItemCategory.s_MediumArmor.Contains(Type);
        public bool IsHeavyArmor => ItemCategory.s_HeavyArmor.Contains(Type);
        public bool IsChestArmor => ItemCategory.s_ChestArmor.Contains(Type);
        public bool IsJewelry => ItemCategory.s_Jewelry.Contains(Type);
        public bool IsOneHandedWeapon => ItemCategory.s_OneHandedWeapon.Contains(Type);
        public bool IsTwoHandedWeapon => ItemCategory.s_TwoHandedWeapon.Contains(Type);
        public bool IsTwoHandedRangedWeapon => ItemCategory.s_TwoHandedRangedWeapon.Contains(Type);
        public bool IsTwoHandedMeleeWeapon => ItemCategory.s_TwoHandedMeleeWeapon.Contains(Type);
        public bool IsSlashingMeleeWeapon => ItemCategory.s_SlashingMeleeWeapon.Contains(Type);
        public bool IsCrushingMeleeWeapon => ItemCategory.s_CrushingMeleeWeapon.Contains(Type);
        public bool IsPiercingMeleeWeapon => ItemCategory.s_PiercingMeleeWeapon.Contains(Type);

        public bool IsEmpty => Id == c_EmptyItemId;
        public bool IsEnchantable => IsSocketable;
        public bool IsSocketable => Type?.MaxSocketCount > 0;
        public bool HasEmptySockets => Sockets.Any(s => s.IsEmpty);

        private List<ItemModifier> m_FixedItemModifiers = new();
        private List<ItemModifier> m_ItemModifiers = new();
        private List<Currency> m_Price = new();
        private GameObject m_Owner;
        private Rarity? m_Rarity;

        public Item(ItemData data, Rarity rarity, ItemType type, List<AttributeModifier> attributeModifiers,
            List<PropertyModifier> propertyModifiers, List<ItemModifier> fixedItemModifiers,
            List<ItemModifier> itemModifiers, List<Currency> price)
        {
            Data = data;
            Id = data.Id;
            BaseName = I18N.Instance.Get(data.NameKey);
            Lore = I18N.Instance.Get(data.LoreKey);
            BookText = I18N.Instance.Get(data.BookTextKey);
            ConsumeDescription = I18N.Instance.Get(data.ConsumeDescriptionKey);
            PassiveDescription = I18N.Instance.Get(data.PassiveDescriptionKey);
            Icon = data.Icon;
            SlotType = data.Slot;
            StackCountMax = Mathf.Max(1, data.StackSize);
            Flags = data.Flags;
            CurrencyType = data.CurrencyType;
            ConsumeSound = data.ConsumeSound;
            Attachments = data.Attachments;
            StackCount = 1;
            Type = type;
            BaseAttributeModifiers = attributeModifiers;
            BasePropertyModifiers = propertyModifiers;

            m_Price = price;
            m_Rarity = rarity;
            m_FixedItemModifiers = fixedItemModifiers;
            m_ItemModifiers = itemModifiers;

            for (var i = 0; i < Data.SocketCount; i++)
            {
                AddSocket();
            }
        }

        public Item(int id, I18NString name, ItemData data, InventoryComponent inventory)
        {
            Data = data;
            Id = id;
            Icon = data.Icon;
            BaseName = name;
            Inventory = inventory;
        }

        public Item Clone()
        {
            var clone = new Item(Id, BaseName, Data, Inventory)
            {
                Lore = Lore,
                BookText = BookText,
                ConsumeDescription = ConsumeDescription,
                PassiveDescription = PassiveDescription,
                Icon = Icon,
                Type = Type,
                SlotType = SlotType,
                StackCount = StackCount,
                StackCountMax = StackCountMax,
                Flags = Flags,
                Sockets = Sockets.ToList(),
                Runes = Runes.ToList(),
                Attachments = Attachments,
                Behaviours = Behaviours,
                CurrencyType = CurrencyType,
                ConsumeSound = ConsumeSound,
                Set = Set,
                Skin = Skin,
                Suffix = Suffix,
                ConsumeEffect = ConsumeEffect,
                BaseAttributeModifiers = BaseAttributeModifiers,
                BasePropertyModifiers = BasePropertyModifiers,
                Equipment = Equipment,
                PriceMultiplier = PriceMultiplier,
                IsBuyout = IsBuyout,
                IsFixedPrice = IsFixedPrice,
                EnchantmentBehaviour = EnchantmentBehaviour,
                EnchantmentItemCategory = EnchantmentItemCategory,
                CooldownRemaining = CooldownRemaining,
                Affixes = Affixes,

                WeaponSkillA = WeaponSkillA != null
                    ? Container.Instance.Resolve<ISkillRepository>().Find(WeaponSkillA.Id)
                    : null,

                WeaponSkillB = WeaponSkillB != null
                    ? Container.Instance.Resolve<ISkillRepository>().Find(WeaponSkillB.Id)
                    : null,

                m_Price = m_Price,
                m_Rarity = m_Rarity,
                m_FixedItemModifiers = m_FixedItemModifiers,
                m_ItemModifiers = m_ItemModifiers,
            };

            clone.ChangeOwner(m_Owner);

            return clone;
        }

        public static ItemTypeType DetermineRuneTypeByIndex(int index, Item item)
        {
            if (item.IsChestArmor || item.IsTwoHandedWeapon)
            {
                if (index >= 4)
                {
                    return ItemTypeType.MajorRune;
                }

                return index >= 2 ? ItemTypeType.Rune : ItemTypeType.MinorRune;
            }

            if (item.IsOneHandedWeapon || item.SlotType == EquipmentSlotType.OffHand)
            {
                if (index >= 2)
                {
                    return ItemTypeType.MajorRune;
                }

                return index >= 1 ? ItemTypeType.Rune : ItemTypeType.MinorRune;
            }

            return index == 2 ? ItemTypeType.Rune : ItemTypeType.MinorRune;
        }

        private string GetName()
        {
            var name = "";

            if (Type?.Type == ItemTypeType.Blueprint)
            {
                name += Type.Name + ": ";
            }

            if (Suffix == null)
            {
                name += BaseName;
            }
            else
            {
                name += BaseName + " " + Suffix.SuffixText;
            }

            return name;
        }

        public int GetPriceAmount(CurrencyType type)
        {
            var currency = m_Price.FirstOrDefault(c => c.Type == type);
            return (int) (currency?.Amount ?? 0 * GetPriceMultiplier());
        }

        private float GetPriceMultiplier()
        {
            return IsFixedPrice ? 1 : PriceMultiplier;
        }

        public void ChangePrice(List<Currency> price)
        {
            m_Price = price;
        }

        public List<Currency> GetPrice()
        {
            var result = new List<Currency>();

            foreach (var currency in m_Price)
            {
                result.Add(currency.Clone().Set((int) (currency.Amount * GetPriceMultiplier())));
            }

            return result;
        }

        public bool IsAnySkillOnCooldown()
        {
            return WeaponSkillA?.IsOnCooldown() == true || WeaponSkillB?.IsOnCooldown() == true;
        }

        public void RunCooldown()
        {
            if (Cooldown == 0)
            {
                return;
            }

            CooldownRemaining = Cooldown;
            CooldownStarted?.Invoke(this);
            AnyItemCooldownStarted?.Invoke(this);
        }

        public void FinishCooldown()
        {
            if (Cooldown == 0)
            {
                return;
            }

            CooldownRemaining = 0;
            CooldownFinished?.Invoke(this);
        }

        public void TickCooldown()
        {
            if (!IsOnCooldown)
            {
                return;
            }

            CooldownRemaining--;

            CooldownUpdated?.Invoke(this);

            if (CooldownRemaining > 0)
            {
                return;
            }

            CooldownRemaining = 0;
            CooldownFinished?.Invoke(this);
        }

        public void ChangeOwner(GameObject entity)
        {
            m_Owner = entity;

            foreach (var socket in Sockets)
            {
                socket.ChangeOwner(m_Owner);
            }

            if (WeaponSkillA != null)
            {
                WeaponSkillA.Caster = m_Owner;
            }

            if (WeaponSkillB != null)
            {
                WeaponSkillB.Caster = m_Owner;
            }
        }

        public void ChangeRarity(Rarity rarity)
        {
            m_Rarity = rarity;
            AnyItemStatsUpdated?.Invoke(this);
        }

        public void ChangeSuffix(ItemModifier suffix)
        {
            Suffix = suffix;
            AnyItemStatsUpdated?.Invoke(this);
        }

        public void RollAffixes()
        {
            int RollAffixCount()
            {
                var table = new RandomTable();

                for (var i = 0; i < 5; i++)
                {
                    table.AddEntry(new RandomTableNumberEntry(i + 1, new RandomTableEntryParameters(100 - 5 * i)));
                }

                return Math.Max(2, (table.Evaluate().FirstOrDefault() as RandomTableNumberEntry)?.Value ?? 0);
            }

            float RarityIdToProbability(int rarity)
            {
                switch (rarity)
                {
                    case Constants.c_ItemRarityIdMagic:
                        return 100;
                    case Constants.c_ItemRarityIdRare:
                        return 90;
                    case Constants.c_ItemRarityIdUnique:
                        return 80;
                    case Constants.c_ItemRarityIdLegendary:
                        return 30;
                    default:
                        return 0;
                }
            }

            List<Behaviour> RollRandomAffixes(int count)
            {
                var table = new RandomTable(count);
                table.AddEntry(new RandomTableNumberEntry(Constants.c_ItemRarityIdMagic, new RandomTableEntryParameters(RarityIdToProbability(Constants.c_ItemRarityIdMagic), true, false, true)));
                table.AddEntry(new RandomTableNumberEntry(Constants.c_ItemRarityIdRare, new RandomTableEntryParameters(RarityIdToProbability(Constants.c_ItemRarityIdRare), true, false, true)));
                table.AddEntry(new RandomTableNumberEntry(Constants.c_ItemRarityIdUnique, new RandomTableEntryParameters(RarityIdToProbability(Constants.c_ItemRarityIdUnique), true, false, true)));
                table.AddEntry(new RandomTableNumberEntry(Constants.c_ItemRarityIdLegendary, new RandomTableEntryParameters(RarityIdToProbability(Constants.c_ItemRarityIdLegendary), true, false, true)));

                var itemCategoryRepository = Container.Instance.Resolve<IItemCategoryRepository>();

                var affixes = Container.Instance.Resolve<IBehaviourRepository>()
                    .Find(b => b.Flags.HasFlag(BehaviourFlags.ItemAffix) && (b.ItemCategories.Count == 0 || itemCategoryRepository.Find(b.ItemCategories).All(c => !c.Contains(Type.Id))));

                var result = new List<Behaviour>();

                foreach (var value in table.Evaluate())
                {
                    if (value is RandomTableNumberEntry integer)
                    {
                        result.Add(affixes.Where(b => b.Rarity.Id == integer.Value).Random());
                    }
                }

                return result;
            }

            Affixes = RollRandomAffixes(RollAffixCount()).OrderBy(a => a.Rarity.Type).ToList();

            AnyItemStatsUpdated?.Invoke(this);
        }

        public void RollSuffix()
        {
            var suffix = Container.Instance.Resolve<IItemModifierRepository>().RandomSuffixForItem(this);

            if (suffix == null)
            {
                return;
            }

            Suffix = suffix;

            AnyItemStatsUpdated?.Invoke(this);
        }

        public void MakeMaxSockets()
        {
            for (var i = 0; i < Type.MaxSocketCount; i++)
            {
                AddSocket();
            }
        }

        public void RollSockets()
        {
            var table = new RandomTable();
            var total = Type.MaxSocketCount * 100f;

            table.AddEntry(new RandomTableNullEntry(new RandomTableEntryParameters(total)));

            for (var i = 1; i <= Type.MaxSocketCount; i++)
            {
                table.AddEntry(new RandomTableNumberEntry(i, new RandomTableEntryParameters(total / (2 * i))));
            }

            var result = table.Evaluate().FirstOrDefault() as RandomTableNumberEntry;

            if (result == null)
            {
                return;
            }

            for (var i = 0; i < result.Value; i++)
            {
                AddSocket();
            }
        }

        private float GetAttributeMultiplier()
        {
            if (Rarity == null)
            {
                return 1;
            }

            return Math.Min((int) Rarity.Type, 4);
        }

        public List<AttributeModifier> GetAttributeModifiers(bool includeFixed = true)
        {
            var multiplier = GetAttributeMultiplier();

            var modifiers = m_ItemModifiers.SelectMany(x => x.GetAttributeModifiers(multiplier)).ToList();

            if (Suffix != null)
            {
                modifiers.AddRange(Suffix.GetAttributeModifiers(multiplier));
            }

            if (includeFixed)
            {
                modifiers.AddRange(GetFixedAttributeModifiers());
            }

            return modifiers;
        }

        public List<PropertyModifier> GetPropertyModifiers(bool includeFixed = true)
        {
            var multiplier = GetAttributeMultiplier();

            var modifiers = m_ItemModifiers.SelectMany(x => x.GetPropertyModifiers(multiplier)).ToList();

            if (Suffix != null)
            {
                modifiers.AddRange(Suffix.GetPropertyModifiers(multiplier));
            }

            if (includeFixed)
            {
                modifiers.AddRange(GetFixedPropertyModifiers());
            }

            return modifiers;
        }

        public List<AttributeModifier> GetFixedAttributeModifiers()
        {
            return m_FixedItemModifiers
                .SelectMany(x => x.GetAttributeModifiers(GetAttributeMultiplier()))
                .Concat(BaseAttributeModifiers)
                .ToList();
        }

        public List<PropertyModifier> GetFixedPropertyModifiers()
        {
            return m_FixedItemModifiers
                .SelectMany(x => x.GetPropertyModifiers(GetAttributeMultiplier()))
                .Concat(BasePropertyModifiers)
                .ToList();
        }

        public void Consume(GameObject entity)
        {
            if (IsOnCooldown)
            {
                throw new ItemIsOnCooldownException();
            }

            MaybeGiveLoot(entity);
            MaybeApplyEffect(entity);
            MaybeUnlockRelic(entity);

            var inventory = Inventory ? Inventory : entity.GetComponent<InventoryComponent>();
            inventory.Remove(this, 1);

            if (!string.IsNullOrEmpty(ConsumeSound))
            {
                AudioManager.Instance.PlayOneShot(ConsumeSound);
            }
        }

        private void MaybeApplyEffect(GameObject entity)
        {
            ConsumeEffect?.Clone().Apply(entity, entity);
        }

        private void MaybeUnlockRelic(GameObject entity)
        {
            if (Data.UnlockRelicId == 0)
            {
                return;
            }

            var reliquary = entity.GetComponent<ReliquaryComponent>();

            if (reliquary.Available.Any(r => r.Id == Data.UnlockRelicId))
            {
                throw new GameplayException(I18N.Instance.Get("exception_relic_already_unlocked"));
            }

            reliquary.Unlock(Data.UnlockRelicId);
        }

        private void MaybeGiveLoot(GameObject entity)
        {
            if (Data.ConsumeLootId == 0)
            {
                return;
            }

            Container.Instance.Instantiate<Loot>().RollDropAsync(Data.ConsumeLootId, items =>
            {
                ContainerWindow.Instance.Show(this, items);
                entity.GetComponent<InventoryComponent>().Pickup(items);
            });
        }

        public Dictionary<Attribute, float> GetAttributeDifference(Item item)
        {
            var difference = new Dictionary<Attribute, float>();

            var attributeTypes = new List<AttributeType>();
            attributeTypes.AddRange(item.GetAttributeModifiers().Select(modifier => modifier.Attribute.Type));
            attributeTypes.AddRange(GetAttributeModifiers().Select(modifier => modifier.Attribute.Type));

            foreach (var attributeType in attributeTypes.Distinct())
            {
                var delta = item.GetAttributeModifiers().Where(modifier => modifier.Attribute.Type == attributeType)
                                .Select(modifier => Mathf.Ceil(modifier.GetAmount())).DefaultIfEmpty(0).Sum() -
                            GetAttributeModifiers().Where(modifier => modifier.Attribute.Type == attributeType)
                                .Select(modifier => Mathf.Ceil(modifier.GetAmount())).DefaultIfEmpty(0).Sum();

                if (Math.Abs(delta) < Mathf.Epsilon)
                {
                    continue;
                }

                difference.Add(GetAttributeModifiers().Concat(item.GetAttributeModifiers()).First(
                    modifier => modifier.Attribute.Type == attributeType).Attribute, delta);
            }

            return difference;
        }

        public Dictionary<Property, float> GetPropertyDifference(Item item)
        {
            var difference = new Dictionary<Property, float>();

            var propertyTypes = new List<PropertyType>();
            propertyTypes.AddRange(item.GetPropertyModifiers().Select(modifier => modifier.Property.Type));
            propertyTypes.AddRange(GetPropertyModifiers().Select(modifier => modifier.Property.Type));

            foreach (var propertyType in propertyTypes.Distinct())
            {
                var delta = item.GetPropertyModifiers().Where(modifier => modifier.Property.Type == propertyType)
                                .Select(modifier => modifier.GetAmount()).DefaultIfEmpty(0).Sum() -
                            GetPropertyModifiers().Where(modifier => modifier.Property.Type == propertyType)
                                .Select(modifier => modifier.GetAmount()).DefaultIfEmpty(0).Sum();

                if (Math.Abs(delta) < Mathf.Epsilon)
                {
                    continue;
                }

                difference.Add(GetPropertyModifiers().Concat(item.GetPropertyModifiers()).First(
                    modifier => modifier.Property.Type == propertyType).Property, delta);
            }

            return difference;
        }

        public Item AddStack(int amount = 1)
        {
            SetStack(StackCount + amount);
            return this;
        }

        public Item RemoveStack(int amount = 1)
        {
            SetStack(StackCount - amount);
            return this;
        }

        public Item SetStack(int amount)
        {
            StackCount = Mathf.Clamp(amount, 1, StackCountMax);
            StackCountChanged?.Invoke();
            return this;
        }

        public void AddSocket()
        {
            if (IsMaximumSocketCount)
            {
                return;
            }

            Sockets.Add(CreateEmpty());
        }

        public void InsertSocket(Item item, int index)
        {
            if (!item.IsGem)
            {
                return;
            }

            Sockets[index] = item;
        }

        public void InsertSocket(Item item)
        {
            if (!item.IsGem)
            {
                return;
            }

            var index = Sockets.FindIndex(s => s.IsEmpty);

            if (index == -1)
            {
                return;
            }

            Sockets[index] = item.Clone().SetStack(1);
            item.Inventory.Remove(item, 1);

            SocketInserted?.Invoke(this, item);
            AnyItemStatsUpdated?.Invoke(this);
        }

        public void RemoveSocket(Item item)
        {
            Sockets[Sockets.IndexOf(item)] = CreateEmpty();
        }

        public void Enchant(Item enchant)
        {
            if (enchant.EnchantmentItemCategory?.Contains(Type) == false)
            {
                throw new InvalidSkillTargetException();
            }

            EnchantmentBehaviour = Container.Instance.Resolve<IBehaviourRepository>()
                .Find(enchant.EnchantmentBehaviour.Id);

            Enchanted?.Invoke(this, enchant);
            AnyItemStatsUpdated?.Invoke(this);
        }
    }
}