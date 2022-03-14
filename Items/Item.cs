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
using DarkBestiary.Messaging;
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
            return new Item(EmptyItemId, new I18NString(new I18NStringData("Empty")), new ItemData(), null);
        }

        public const int MaxForgeLevel = 5;
        public const int MaxSharpeningLevel = 15;
        public const int EmptyItemId = -1;

        public static event Payload<Item> AnyItemStatsUpdated;
        public static event Payload<Item> AnyItemCooldownStarted;

        public event Payload StackCountChanged;
        public event Payload<Item> Forged;
        public event Payload<Item> SharpeningSuccess;
        public event Payload<Item> SharpeningFailed;
        public event Payload<Item> SharpeningLevelChanged;
        public event Payload<Item> CooldownStarted;
        public event Payload<Item> CooldownUpdated;
        public event Payload<Item> CooldownFinished;
        public event Payload<Item, Item> SocketInserted;
        public event Payload<Item, Item> Enchanted;

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
        public int RequiredLevel { get; private set; }
        public CurrencyType CurrencyType { get; private set; }
        public List<PropertyModifier> BasePropertyModifiers { get; private set; } = new List<PropertyModifier>();
        public List<AttributeModifier> BaseAttributeModifiers { get; private set; } = new List<AttributeModifier>();

        public int Cooldown => Data.ConsumeCooldown;
        public int CooldownRemaining { get; private set; }
        public bool IsOnCooldown => Cooldown > 0 && CooldownRemaining > 0;

        public Rarity Rarity => this.rarity;
        public ItemType Type { get; private set; }
        public List<Item> Sockets { get; set; } = new List<Item>();
        public List<Behaviour> Affixes { get; set; } = new List<Behaviour>();
        public List<Behaviour> Behaviours { get; set; } = new List<Behaviour>();
        public List<Item> Runes { get; set; } = new List<Item>();

        public Behaviour EnchantmentBehaviour { get; set; }
        public ItemCategory EnchantmentItemCategory { get; set; }
        public EquipmentSlot EquipmentSlot { get; set; }
        public EquipmentComponent Equipment { get; set; }
        public InventoryComponent Inventory { get; set; }
        public int ForgeLevel { get; set; }
        public int SharpeningLevel { get; set; }
        public ItemModifier Suffix { get; set; }
        public List<AttachmentInfo> Attachments { get; set; }
        public ItemSet Set { get; set; }

        public Skill WeaponSkillA
        {
            get => this.weaponSkillA;
            set
            {
                this.weaponSkillA = value;

                if (this.weaponSkillA == null)
                {
                    return;
                }

                this.weaponSkillA.Weapon = this;
            }
        }

        private Skill weaponSkillA;

        public Skill WeaponSkillB
        {
            get => this.weaponSkillB;
            set
            {
                this.weaponSkillB = value;

                if (this.weaponSkillB == null)
                {
                    return;
                }

                this.weaponSkillB.Weapon = this;
            }
        }

        private Skill weaponSkillB;

        public Skill LearnSkill { get; set; }
        public Skill UnlockSkill { get; set; }
        public Recipe BlueprintRecipe { get; set; }
        public Skin Skin { get; set; }
        public Effect ConsumeEffect { get; set; }
        public float PriceMultiplier { get; set; } = 1;
        public bool IsFixedPrice { get; set; }

        public int Level => BaseLevel + ForgeLevel;
        public int BaseLevel => this.level;

        public string Name => GetName();
        public string ColoredName => Rarity == null ? Name : $"<color={Rarity.ColorCode}>{Name}</color>";
        public string ColoredBaseName => Rarity == null ? BaseName : $"<color={Rarity.ColorCode}>{BaseName}</color>";

        public bool IsMarkedAsIllusory { get; set; }
        public bool IsBuyout { get; set; }
        public bool IsMaximumSocketCount => Sockets.Count >= Type.MaxSocketCount;
        public bool IsGambleable => Flags.HasFlag(ItemFlags.Gambleable);
        public bool IsStackable => Flags.HasFlag(ItemFlags.Stackable);
        public bool IsDismantable => Flags.HasFlag(ItemFlags.Dismantable);
        public bool IsUniqueEquipped => Flags.HasFlag(ItemFlags.UniqueEquipped);
        public bool IsCampaignOnly => Flags.HasFlag(ItemFlags.CampaignOnly);
        public bool IsVisionsOnly => Flags.HasFlag(ItemFlags.VisionsOnly);
        public bool IsIllusory => Flags.HasFlag(ItemFlags.Illusory);
        public bool IsSharpening => Flags.HasFlag(ItemFlags.Sharpening);

        public bool IsRune => Type?.Type == ItemTypeType.Rune;
        public bool IsMinorRune => Type?.Type == ItemTypeType.MinorRune;
        public bool IsMajorRune => Type?.Type == ItemTypeType.MajorRune;
        public bool IsAnyRune => Type?.Type == ItemTypeType.Rune || Type?.Type == ItemTypeType.MajorRune || Type?.Type == ItemTypeType.MinorRune;

        public bool IsEnchantment => Type?.Type == ItemTypeType.Enchantment;
        public bool IsConsumable => Type?.Type == ItemTypeType.Consumable;
        public bool IsRelic => Type?.Type == ItemTypeType.Relic;
        public bool IsSkillBook => Type?.Type == ItemTypeType.SkillBook;
        public bool IsIngredient => Type?.Type == ItemTypeType.Ingredient;
        public bool IsBlueprint => Type?.Type == ItemTypeType.Blueprint;
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
        public bool IsGem => ItemCategory.Gems.Contains(Type);
        public bool IsMeleeWeapon => ItemCategory.MeleeWeapon.Contains(Type);
        public bool IsRangedWeapon => ItemCategory.RangedWeapon.Contains(Type);
        public bool IsWeapon => ItemCategory.Weapon.Contains(Type);
        public bool IsArmor => ItemCategory.Armor.Contains(Type);
        public bool IsLightArmor => ItemCategory.LightArmor.Contains(Type);
        public bool IsMediumArmor => ItemCategory.MediumArmor.Contains(Type);
        public bool IsHeavyArmor => ItemCategory.HeavyArmor.Contains(Type);
        public bool IsChestArmor => ItemCategory.ChestArmor.Contains(Type);
        public bool IsJewelry => ItemCategory.Jewelry.Contains(Type);
        public bool IsOneHandedWeapon => ItemCategory.OneHandedWeapon.Contains(Type);
        public bool IsTwoHandedWeapon => ItemCategory.TwoHandedWeapon.Contains(Type);
        public bool IsTwoHandedRangedWeapon => ItemCategory.TwoHandedRangedWeapon.Contains(Type);
        public bool IsTwoHandedMeleeWeapon => ItemCategory.TwoHandedMeleeWeapon.Contains(Type);
        public bool IsSlashingMeleeWeapon => ItemCategory.SlashingMeleeWeapon.Contains(Type);
        public bool IsCrushingMeleeWeapon => ItemCategory.CrushingMeleeWeapon.Contains(Type);
        public bool IsPiercingMeleeWeapon => ItemCategory.PiercingMeleeWeapon.Contains(Type);
        public bool IsEmpty => Id == EmptyItemId;
        public bool IsEnchantable => IsSocketable;
        public bool IsSocketable => Type?.MaxSocketCount > 0;
        public bool HasEmptySockets => Sockets.Any(s => s.IsEmpty);

        private List<ItemModifier> fixedItemModifiers = new List<ItemModifier>();
        private List<ItemModifier> itemModifiers = new List<ItemModifier>();
        private List<Currency> price = new List<Currency>();
        private GameObject owner;
        private int level;
        private Rarity rarity;

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
            RequiredLevel = data.RequiredLevel;
            Attachments = data.Attachments;
            StackCount = 1;
            Type = type;
            BaseAttributeModifiers = attributeModifiers;
            BasePropertyModifiers = propertyModifiers;

            this.price = price;
            this.level = data.Level;
            this.rarity = rarity;
            this.fixedItemModifiers = fixedItemModifiers;
            this.itemModifiers = itemModifiers;

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

        public static bool MatchDropByMonsterLevel(ItemData item, int monsterLevel)
        {
            return MatchDropByMonsterLevel(item.Level, monsterLevel);
        }

        public static bool MatchDropByMonsterLevel(Item item, int monsterLevel)
        {
            return MatchDropByMonsterLevel(item.Level, monsterLevel);
        }

        private static bool MatchDropByMonsterLevel(int itemLevel, int monsterLevel)
        {
            if (monsterLevel > 15)
            {
                return itemLevel >= 2;
            }

            if (monsterLevel.InRange(10, 15))
            {
                return itemLevel == 1 || itemLevel == 2;
            }

            return itemLevel == 1;
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
                RequiredLevel = RequiredLevel,
                Attachments = Attachments,
                Behaviours = Behaviours,
                CurrencyType = CurrencyType,
                ConsumeSound = ConsumeSound,
                Set = Set,
                Skin = Skin,
                Suffix = Suffix,
                ForgeLevel = ForgeLevel,
                ConsumeEffect = ConsumeEffect,
                BaseAttributeModifiers = BaseAttributeModifiers,
                BasePropertyModifiers = BasePropertyModifiers,
                BlueprintRecipe = BlueprintRecipe,
                Equipment = Equipment,
                PriceMultiplier = PriceMultiplier,
                IsBuyout = IsBuyout,
                IsFixedPrice = IsFixedPrice,
                IsMarkedAsIllusory = IsMarkedAsIllusory,
                EnchantmentBehaviour = EnchantmentBehaviour,
                EnchantmentItemCategory = EnchantmentItemCategory,
                CooldownRemaining = CooldownRemaining,
                Affixes = Affixes,
                SharpeningLevel = SharpeningLevel,

                WeaponSkillA = WeaponSkillA != null
                    ? Container.Instance.Resolve<ISkillRepository>().Find(WeaponSkillA.Id) : null,

                WeaponSkillB = WeaponSkillB != null
                    ? Container.Instance.Resolve<ISkillRepository>().Find(WeaponSkillB.Id) : null,

                UnlockSkill = UnlockSkill != null
                    ? Container.Instance.Resolve<ISkillRepository>().Find(UnlockSkill.Id) : null,

                LearnSkill = LearnSkill != null
                    ? Container.Instance.Resolve<ISkillRepository>().Find(LearnSkill.Id) : null,

                price = this.price,
                level = this.level,
                rarity = this.rarity,
                fixedItemModifiers = this.fixedItemModifiers,
                itemModifiers = this.itemModifiers,
            };

            clone.ChangeOwner(this.owner);

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

            if (SharpeningLevel > 0)
            {
                name += $" +{SharpeningLevel}";
            }

            return name;
        }

        public int GetPriceAmount(CurrencyType type)
        {
            var currency = this.price.FirstOrDefault(c => c.Type == type);
            return (int) (currency?.Amount ?? 0 * GetPriceMultiplier());
        }

        private float GetPriceMultiplier()
        {
            return IsFixedPrice ? 1 : PriceMultiplier;
        }

        public void ChangePrice(List<Currency> price)
        {
            this.price = price;
        }

        public List<Currency> GetPrice()
        {
            var result = new List<Currency>();

            foreach (var currency in this.price)
            {
                result.Add(currency.Clone().Set((int) (currency.Amount * GetPriceMultiplier())));
            }

            return result;
        }

        public bool IsAnySkillOnCooldown()
        {
            return WeaponSkillA?.IsOnCooldown() == true || WeaponSkillB?.IsOnCooldown() == true ||
                   UnlockSkill?.IsOnCooldown() == true;
        }

        public void Forge()
        {
            ForgeLevel++;
            Forged?.Invoke(this);
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
            this.owner = entity;

            foreach (var socket in Sockets)
            {
                socket.ChangeOwner(this.owner);
            }

            if (WeaponSkillA != null)
            {
                WeaponSkillA.Caster = this.owner;
            }

            if (WeaponSkillB != null)
            {
                WeaponSkillB.Caster = this.owner;
            }

            if (UnlockSkill != null)
            {
                UnlockSkill.Caster = this.owner;
            }

            if (LearnSkill != null)
            {
                LearnSkill.Caster = this.owner;
            }

            BlueprintRecipe?.Item.ChangeOwner(this.owner);
        }

        public void ChangeRarity(Rarity rarity)
        {
            this.rarity = rarity;
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
                var table = new RandomizerTable();

                for (var i = 0; i < 5; i++)
                {
                    table.Add(new RandomizerIntValue(i + 1, 100 - 5 * i));
                }

                return Math.Max(2, (table.Evaluate().FirstOrDefault() as RandomizerIntValue)?.Value ?? 0);
            }

            float RarityIdToProbability(int rarity)
            {
                switch (rarity)
                {
                    case Constants.ItemRarityIdMagic:
                        return 100;
                    case Constants.ItemRarityIdRare:
                        return 90;
                    case Constants.ItemRarityIdUnique:
                        return 80;
                    case Constants.ItemRarityIdLegendary:
                        return 30;
                    default:
                        return 0;
                }
            }

            List<Behaviour> RollRandomAffixes(int count)
            {
                var table = new RandomizerTable(count);
                table.Add(new RandomizerIntValue(Constants.ItemRarityIdMagic, RarityIdToProbability(Constants.ItemRarityIdMagic), true, false, true));
                table.Add(new RandomizerIntValue(Constants.ItemRarityIdRare, RarityIdToProbability(Constants.ItemRarityIdRare), true, false, true));
                table.Add(new RandomizerIntValue(Constants.ItemRarityIdUnique, RarityIdToProbability(Constants.ItemRarityIdUnique), true, false, true));
                table.Add(new RandomizerIntValue(Constants.ItemRarityIdLegendary, RarityIdToProbability(Constants.ItemRarityIdLegendary), true, false, true));

                var itemCategoryRepository = Container.Instance.Resolve<IItemCategoryRepository>();

                var affixes = Container.Instance.Resolve<IBehaviourRepository>()
                    .Find(b => b.Flags.HasFlag(BehaviourFlags.ItemAffix) && (b.ItemCategories.Count == 0 || itemCategoryRepository.Find(b.ItemCategories).All(c => !c.Contains(Type.Id))));

                var result = new List<Behaviour>();

                foreach (var value in table.Evaluate())
                {
                    if (value is RandomizerIntValue integer)
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
            var table = new RandomizerTable();
            var total = Type.MaxSocketCount * 100f;

            table.Add(new RandomizerNullValue(total));

            for (var i = 1; i <= Type.MaxSocketCount; i++)
            {
                table.Add(new RandomizerIntValue(i, total / (2 * i)));
            }

            var result = table.Evaluate().FirstOrDefault() as RandomizerIntValue;

            if (result == null)
            {
                return;
            }

            for (var i = 0; i < result.Value; i++)
            {
                AddSocket();
            }
        }

        public List<AttributeModifier> GetAttributeModifiers(bool includeFixed = true)
        {
            var modifiers = new List<AttributeModifier>()
                .Concat(Suffix?.GetAttributeModifiers(BaseLevel, ForgeLevel, CraftUtils.GetRarityMultiplier(this)) ?? new List<AttributeModifier>())
                .Concat(this.itemModifiers.SelectMany(m => m.GetAttributeModifiers(BaseLevel, ForgeLevel, CraftUtils.GetRarityMultiplier(this))))
                .ToList();

            if (includeFixed)
            {
                modifiers = modifiers
                    .Concat(GetFixedAttributeModifiers())
                    .ToList();
            }

            return modifiers.ToList();
        }

        public List<AttributeModifier> GetSharpeningAttributeModifiers()
        {
            var sharpening = new List<AttributeModifier>();

            if (SharpeningLevel < 1)
            {
                return sharpening;
            }

            var attributeId = IsArmor || IsShield ? Constants.AttributeIdConstitution : Constants.AttributeIdMight;
            var attribute = Container.Instance.Resolve<IAttributeRepository>().Find(attributeId);
            sharpening.Add(new AttributeModifier(attribute, new AttributeModifierData(attribute.Id, SharpeningLevel * 2, ModifierType.Flat)));

            return sharpening;
        }

        public List<AttributeModifier> GetFixedAttributeModifiers()
        {
            return this.fixedItemModifiers
                .SelectMany(m => m.GetAttributeModifiers(BaseLevel, 0, CraftUtils.GetRarityMultiplier(this)))
                .Concat(BaseAttributeModifiers)
                .ToList();
        }

        public List<PropertyModifier> GetPropertyModifiers(bool includeFixed = true)
        {
            var modifiers = new List<PropertyModifier>()
                .Concat(Suffix?.GetPropertyModifiers(BaseLevel, ForgeLevel, CraftUtils.GetRarityMultiplier(this)) ?? new List<PropertyModifier>())
                .Concat(this.itemModifiers.SelectMany(m => m.GetPropertyModifiers(BaseLevel, ForgeLevel, CraftUtils.GetRarityMultiplier(this))))
                .ToList();

            if (includeFixed)
            {
                modifiers = modifiers.Concat(GetFixedPropertyModifiers()).ToList();
            }

            return modifiers;
        }

        public List<PropertyModifier> GetFixedPropertyModifiers()
        {
            return this.fixedItemModifiers
                .SelectMany(m => m.GetPropertyModifiers(BaseLevel, 0, CraftUtils.GetRarityMultiplier(this)))
                .Concat(BasePropertyModifiers)
                .ToList();
        }

        public void Consume(GameObject entity)
        {
            if (IsOnCooldown)
            {
                throw new ItemIsOnCooldownException();
            }

            MaybeLearnSkill();
            MaybeUnlockRecipe();
            MaybeUnlockScenario();
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

        private void MaybeLearnSkill()
        {
            if (LearnSkill == null)
            {
                return;
            }

            var spellbook = CharacterManager.Instance.Character.Entity.GetComponent<SpellbookComponent>();

            if (spellbook.IsKnown(LearnSkill.Id))
            {
                throw new GameplayException(I18N.Instance.Get("exception_skill_already_known"));
            }

            spellbook.Add(LearnSkill);
        }

        private void MaybeUnlockRecipe()
        {
            if (BlueprintRecipe == null)
            {
                return;
            }

            var character = CharacterManager.Instance.Character;

            if (character.Data.UnlockedRecipes.Contains(BlueprintRecipe.Id))
            {
                throw new GameplayException(I18N.Instance.Get("exception_recipe_already_unlocked"));
            }

            character.UnlockRecipe(BlueprintRecipe);
        }

        private void MaybeUnlockScenario()
        {
            if (Data.UnlockScenarioId == 0)
            {
                return;
            }

            var character = CharacterManager.Instance.Character;

            if (character.AvailableScenarios.Contains(Data.UnlockScenarioId) ||
                character.CompletedScenarios.Contains(Data.UnlockScenarioId))
            {
                throw new GameplayException(I18N.Instance.Get("exception_scenario_already_unlocked"));
            }

            character.UnlockScenario(Data.UnlockScenarioId);
        }

        private void MaybeGiveLoot(GameObject entity)
        {
            if (Data.ConsumeLootId == 0)
            {
                return;
            }

            var lootData = Container.Instance.Resolve<ILootDataRepository>().Find(Data.ConsumeLootId);
            var loot = Container.Instance.Instantiate<Loot>(new object[] {lootData});

            ContainerWindow.Instance.Show(this, loot, entity);
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
            if (enchant.IsSharpening)
            {
                var nextLevel = SharpeningLevel + 1;

                if (!CraftUtils.SharpeningTable.ContainsKey(nextLevel))
                {
                    throw new GameplayException("Max sharpening level reached.");
                }

                var chance = CraftUtils.SharpeningTable[nextLevel];

                if (RNG.Test(chance))
                {
                    SharpeningLevel = Mathf.Clamp(SharpeningLevel + 1, 0, MaxSharpeningLevel);
                    SharpeningSuccess?.Invoke(this);
                }
                else
                {
                    SharpeningLevel = enchant.Rarity.Type == RarityType.Legendary ? Mathf.Clamp(SharpeningLevel - 1, 0, MaxSharpeningLevel) : 0;
                    SharpeningFailed?.Invoke(this);
                }

                SharpeningLevelChanged?.Invoke(this);
                AnyItemStatsUpdated?.Invoke(this);
                return;
            }

            if (enchant.Id == Constants.ItemIdIllusorySubstance)
            {
                IsMarkedAsIllusory = true;
                return;
            }

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