using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Modifiers;
using DarkBestiary.Properties;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltip : Singleton<ItemTooltip>
    {
        public event Payload<ItemTooltip> Shown;
        public event Payload<ItemTooltip> Hidden;

        public bool DisplayPrice { get; set; } = true;
        public bool HasOverflow { get; private set; }

        [SerializeField] private Sprite emptySocket;
        [SerializeField] private CustomText textPrefab;
        [SerializeField] private TextWithIcon textWithIconPrefab;
        [SerializeField] private ItemTooltipRune runePrefab;
        [SerializeField] private ItemTooltipDifference differencePrefab;
        [SerializeField] private GameObject separatorPrefab;
        [SerializeField] private SkillTooltipSet setPrefab;
        [SerializeField] private ItemTooltipHeader headerPrefab;
        [SerializeField] private ItemTooltipStars starsPrefab;
        [SerializeField] private ItemTooltipSocket socketPrefab;
        [SerializeField] private SkillTooltipCost costPrefab;
        [SerializeField] private ItemTooltipPrice pricePrefab;
        [SerializeField] private Transform container;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private Character character;

        private ItemTooltipHeader header;
        private ItemTooltipStars stars;
        private ItemTooltipPrice price;

        private MonoBehaviourPool<ItemTooltipDifference> differencePool;
        private MonoBehaviourPool<ItemTooltipSocket> socketPool;
        private MonoBehaviourPool<SkillTooltipCost> costPool;
        private MonoBehaviourPool<CustomText> textPool;
        private MonoBehaviourPool<TextWithIcon> textWithIconPool;
        private MonoBehaviourPool<ItemTooltipRune> runePool;
        private MonoBehaviourPool<SkillTooltipSet> setPool;
        private GameObjectPool separatorPool;

        private bool isInitialized;

        private void Start()
        {
            Initialize();
            Instance.Hide();
        }

        private void Update()
        {
            if (!(Mathf.Abs(Input.mouseScrollDelta.y) > 0))
            {
                return;
            }

            if (HasOverflow)
            {
                ChangePivotY(Input.mouseScrollDelta.y > 0 ? 0 : 1);
            }
        }

        private void ChangePivotY(int pivotY)
        {
            if (Math.Abs(pivotY - this.rectTransform.pivot.y) < Mathf.Epsilon)
            {
                return;
            }

            this.rectTransform.ChangePivot(new Vector2(this.rectTransform.pivot.x, pivotY));
            this.rectTransform.ClampPositionToParent(this.parentRectTransform);
        }

        public void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.price = Instantiate(this.pricePrefab, this.container);
            this.price.Initialize();

            this.differencePool = MonoBehaviourPool<ItemTooltipDifference>.Factory(this.differencePrefab, this.container);
            this.socketPool = MonoBehaviourPool<ItemTooltipSocket>.Factory(this.socketPrefab, this.container);
            this.costPool = MonoBehaviourPool<SkillTooltipCost>.Factory(this.costPrefab, this.container);
            this.textPool = MonoBehaviourPool<CustomText>.Factory(this.textPrefab, this.container);
            this.textWithIconPool = MonoBehaviourPool<TextWithIcon>.Factory(this.textWithIconPrefab, this.container);
            this.runePool = MonoBehaviourPool<ItemTooltipRune>.Factory(this.runePrefab, this.container);
            this.separatorPool = GameObjectPool.Factory(this.separatorPrefab, this.container);
            this.setPool = MonoBehaviourPool<SkillTooltipSet>.Factory(this.setPrefab, this.container);

            this.rectTransform = GetComponent<RectTransform>();
            this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();

            CharacterManager.CharacterSelected += OnCharacterSelected;
            this.character = CharacterManager.Instance.Character;

            this.isInitialized = true;
        }

        public void Terminate()
        {
            if (!this.isInitialized)
            {
                return;
            }

            this.price.Terminate();

            this.differencePool.Clear();
            this.separatorPool.Clear();
            this.socketPool.Clear();
            this.costPool.Clear();
            this.textPool.Clear();
            this.setPool.Clear();
            this.textWithIconPool.Clear();
            this.runePool.Clear();
        }

        private void OnCharacterSelected(Character character)
        {
            this.character = character;
        }

        public void Show(Item item, RectTransform rect = null)
        {
            gameObject.SetActive(true);

            Clear();

            CreateStars(item);
            CreateHeader(item);

            if (item.IsMarkedAsIllusory)
            {
                CreateText().Text = " ";
                var text = CreateText();
                text.Color = Color.green;
                text.Text = I18N.Instance.Get("ui_illusory");
            }

            var isBlueprint = item.IsBlueprint;

            if (isBlueprint)
            {
                CreateRecipeAlreadyKnownNotification(item);
                item = item.BlueprintRecipe.Item;
            }

            if (item.IsRelic)
            {
                CreateRelicAlreadyKnownNotification(item);
            }

            CreateSharpeningModifiers(item);

            if (item.LearnSkill != null)
            {
                CreateSkillAlreadyKnownNotification(item);
            }

            if (!item.Flags.HasFlag(ItemFlags.HasRandomSuffix))
            {
                CreateFixedModifiers(item);
            }

            CreateModifiers(item);
            CreateSockets(item);
            CreateEnchantDescription(item);
            CreateWeaponSkillDescription(item.WeaponSkillA);
            CreateWeaponSkillDescription(item.WeaponSkillB);

            if (item.Flags.HasFlag(ItemFlags.HasRandomSuffix))
            {
                CreateFixedModifiersAsPassive(item);
            }

            CreatePassiveDescription(item);
            CreateConsumeDescription(item);
            CreateUnlockSkillDescription(item.UnlockSkill);
            CreateLearnSkillDescription(item.LearnSkill);
            CreateLore(item);
            CreateSetInfo(item);
            CreateAffixes(item);
            CreateRunes(item);
            CreateDifference(item);
            CreateRequiredLevelLabel(item);
            CreateWarnings(item);
            CreatePrice(item);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);

            if (rect != null)
            {
                this.rectTransform.MoveTooltip(rect, this.parentRectTransform);
                this.rectTransform.ClampPositionToParent();
            }

            HasOverflow = this.rectTransform.rect.height - this.parentRectTransform.rect.height > 0;

            if (HasOverflow)
            {
                ChangePivotY(0);
            }

            Shown?.Invoke(this);
        }

        private void CreateRecipeAlreadyKnownNotification(Item item)
        {
            if (this.character.Data.UnlockedRecipes.Contains(item.BlueprintRecipe.Id))
            {
                CreateText().Text = " ";

                var text = CreateText();
                text.Text = I18N.Instance.Get("ui_already_known");
                text.Color = Color.red;
            }
        }

        private void CreateSkillAlreadyKnownNotification(Item item)
        {
            if (!this.character.Entity.GetComponent<SpellbookComponent>().IsKnown(item.LearnSkill.Id))
            {
                return;
            }

            CreateText().Text = " ";

            var text = CreateText();
            text.Text = I18N.Instance.Get("ui_already_known");
            text.Color = Color.red;
        }

        private void CreateRelicAlreadyKnownNotification(Item item)
        {
            var isRelicAlreadyUnlocked = this.character.Entity.GetComponent<ReliquaryComponent>().Available
                .Any(r => r.Id == item.Data.UnlockRelicId);

            if (isRelicAlreadyUnlocked)
            {
                CreateText().Text = " ";

                var text = CreateText();
                text.Text = I18N.Instance.Get("ui_already_known");
                text.Color = Color.red;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Hidden?.Invoke(this);
        }

        private void Clear()
        {
            this.differencePool.DespawnAll();
            this.separatorPool.DespawnAll();
            this.socketPool.DespawnAll();
            this.costPool.DespawnAll();
            this.textPool.DespawnAll();
            this.setPool.DespawnAll();
            this.textWithIconPool.DespawnAll();
            this.runePool.DespawnAll();
        }

        private void CreateAffixes(Item item)
        {
            if (item.Affixes.Count == 0)
            {
                return;
            }

            CreateSeparator();

            var index = 0;

            foreach (var affix in item.Affixes)
            {
                index++;

                var text = CreateTextWithIcon();
                text.Color = affix.Rarity.Color();
                text.Icon.sprite = Resources.Load<Sprite>(affix.Icon);

                var nameText = affix.Name + (affix.IsUnique ? $" ({I18N.Instance.Translate("ui_talent")})" : "");
                text.Text = $"{nameText}\n<color=white>{affix.Description}</color>";

                if (index < item.Affixes.Count)
                {
                    CreateText().Text = " ";
                }
            }
        }

        private void CreateSetInfo(Item item)
        {
            if (item.Set == null)
            {
                return;
            }

            var inventory = this.character.Entity.GetComponent<InventoryComponent>();
            var equipment = this.character.Entity.GetComponent<EquipmentComponent>();

            var piecesObtained = equipment.GetItemSetPiecesObtained(item.Set);
            var piecesEquipped = equipment.GetItemSetPiecesEquipped(item.Set);

            CreateText().Text = " ";
            CreateText().Text =
                item.Set.Name + $" ({piecesObtained}/{item.Set.Items.Count})";

            foreach (var setItem in item.Set.Items)
            {
                var row = CreateText();

                row.Color = inventory.Contains(setItem.Id) || equipment.IsEquipped(setItem.Id)
                    ? Color.white
                    : Color.grey;

                row.Text = "    " + setItem.BaseName;
            }

            CreateText().Text = " ";

            foreach (var behaviourInfo in item.Set.Behaviours)
            {
                var bonusAvailable = piecesEquipped >= behaviourInfo.Key;

                var row = CreateText();
                row.Color = bonusAvailable ? Color.green : Color.grey;
                row.Text = I18N.Instance.Get("ui_x_item_set_pieces").ToString(behaviourInfo.Key);

                foreach (var behaviour in behaviourInfo.Value)
                {
                    row = CreateText();
                    row.Color = bonusAvailable ? Color.green : Color.grey;
                    row.Text = $"    {behaviour.Description.ToString(new StringVariableContext(this.character.Entity)).Replace("\n", "\n    ")}";
                }
            }
        }

        private void CreateDifference(Item item)
        {
            var equipment = this.character.Entity.GetComponent<EquipmentComponent>();

            if (equipment.IsEquipped(item))
            {
                return;
            }

            var equipped = equipment.FindSuitableSlot(item)?.Item;

            if (equipped == null)
            {
                return;
            }

            var attributeDifference = equipped.GetAttributeDifference(item);
            var propertyDifference = equipped.GetPropertyDifference(item);

            if (attributeDifference.Count == 0 && propertyDifference.Count == 0)
            {
                return;
            }

            CreateSeparator();

            foreach (var difference in attributeDifference.OrderBy(pair => pair.Key.Index))
            {
                CreateAttributeDifferenceRow(difference.Key, difference.Value);
            }

            foreach (var difference in propertyDifference.OrderBy(pair => pair.Key.Index))
            {
                CreatePropertyDifferenceRow(difference.Key, difference.Value);
            }
        }

        private void CreateAttributeDifferenceRow(Attribute attribute, float delta)
        {
            CreateDifference().Construct(delta > 0, $"{Mathf.Ceil(Mathf.Abs(delta))} {attribute.Name}");
        }

        private void CreatePropertyDifferenceRow(Property property, float delta)
        {
            CreateDifference().Construct(delta > 0, $"{Property.ValueString(property.Type, Mathf.Abs(delta))} {property.Name}");
        }

        private void CreateRequiredLevelLabel(Item item)
        {
            if (item.RequiredLevel <= 1)
            {
                return;
            }

            CreateText().Text = " ";

            var requiredLevel = CreateText();
            requiredLevel.Text = $"{I18N.Instance.Get("ui_required_level")}: {item.RequiredLevel}";
            requiredLevel.Alignment = TextAlignmentOptions.MidlineRight;

            if (this.character.Entity.GetComponent<ExperienceComponent>().Experience.Level < item.RequiredLevel)
            {
                requiredLevel.Color = Color.red;
            }
        }

        private void CreateWarnings(Item item)
        {
            if (item.Rarity.Id != Constants.ItemRarityIdVision || !item.IsEquipment)
            {
                return;
            }

            CreateText().Text = " ";

            var warning = CreateText();
            warning.Text = I18N.Instance.Translate("exception_equip_more_than_one_item_of_type");
            warning.Color = Color.red;
        }

        private void CreatePrice(Item item)
        {
            if (!DisplayPrice || item.GetPrice().Count == 0)
            {
                this.price.gameObject.SetActive(false);
                return;
            }

            this.price.gameObject.SetActive(true);
            this.price.Refresh(item.GetPrice());
            this.price.transform.SetAsLastSibling();
        }

        private void CreateRunes(Item item)
        {
            if (item.Runes.Count == 0)
            {
                return;
            }

            CreateSeparator();

            for (var i = 0; i < item.Runes.Count; i++)
            {
                CreateRuneSocket(item.Runes[i], item, i);

                if (i < item.Runes.Count - 1)
                {
                    CreateText().Text = " ";
                }
            }
        }

        private void CreateSockets(Item item)
        {
            if (item.Sockets.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";

            foreach (var socket in item.Sockets)
            {
                var socketBonusParts = new List<string>(4);
                socketBonusParts.AddRange(socket.GetAttributeModifiers().Select(modifier => modifier.ToString()));
                socketBonusParts.AddRange(socket.GetPropertyModifiers().Select(modifier => modifier.ToString()));

                CreateSocket().Construct(
                    socket.IsEmpty ? this.emptySocket : Resources.Load<Sprite>(socket.Icon),
                    socket.IsEmpty ? I18N.Instance.Get("ui_empty_socket") : string.Join(", ", socketBonusParts)
                );
            }
        }

        private void CreateModifiers(Item item)
        {
            var attributeModifiers = item.GetAttributeModifiers(false);
            var propertyModifiers = item.GetPropertyModifiers(false);

            if (attributeModifiers.Count == 0 && propertyModifiers.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";
            CreateAttributeModifiers(attributeModifiers);
            CreatePropertyModifiers(propertyModifiers);
        }

        private void CreatePropertyModifiers(List<PropertyModifier> modifiers)
        {
            foreach (var group in modifiers.OrderBy(mod => mod.Property.Index).GroupBy(mod => mod.Property.Id))
            {
                CreateText().Text =
                    $"+{Property.ValueString(group.First().Property.Type, group.Sum(mod => mod.GetAmount()))} {group.First().Property.Name}";
            }
        }

        private void CreateAttributeModifiers(List<AttributeModifier> modifiers)
        {
            foreach (var group in modifiers.OrderBy(mod => mod.Attribute.Index).GroupBy(mod => mod.Attribute.Id))
            {
                CreateText().Text = $"+{Mathf.Ceil(group.Sum(mod => mod.GetAmount()))} {group.First().Attribute.Name}";
            }
        }

        private void CreateLore(Item item)
        {
            if (item.Lore.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";

            var lore = CreateText();
            lore.Color = new Color(0.9f, 0.8f, 0.5f);
            lore.Text = item.Lore;
        }

        private void CreateSharpeningModifiers(Item item)
        {
            if (item.SharpeningLevel < 1)
            {
                return;
            }

            CreateText().Text = $"{I18N.Instance.Translate("ui_improved")}: {item.GetSharpeningAttributeModifiers().First()}";
        }

        private void CreateFixedModifiers(Item item)
        {
            var attributeModifiers = item.GetFixedAttributeModifiers();
            var propertyModifiers = item.GetFixedPropertyModifiers();

            if (attributeModifiers.Count == 0 && propertyModifiers.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";

            CreateAttributeModifiers(attributeModifiers);
            CreatePropertyModifiers(propertyModifiers);
        }

        private void CreateFixedModifiersAsPassive(Item item)
        {
            var attributeModifiers = item.GetFixedAttributeModifiers();
            var propertyModifiers = item.GetFixedPropertyModifiers();

            if (attributeModifiers.Count == 0 && propertyModifiers.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";

            foreach (var modifier in attributeModifiers)
            {
                CreatePassiveDescriptionText(modifier.GetDescriptionText());
            }

            foreach (var modifier in propertyModifiers)
            {
                CreatePassiveDescriptionText(modifier.GetDescriptionText());
            }
        }

        private void CreatePassiveDescription(Item item)
        {
            if (item.PassiveDescription.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";
            CreatePassiveDescriptionText(item.PassiveDescription.ToString(new StringVariableContext(this.character.Entity)));
        }

        private void CreatePassiveDescriptionText(string description)
        {
            var text = CreateText();
            text.Color = Color.green;
            text.Text = I18N.Instance.Get("ui_passive") + ": " + description;
        }

        private void CreateEnchantDescription(Item item)
        {
            if (item.EnchantmentBehaviour == null)
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Color = Color.green;
            description.Text = I18N.Instance.Get("ui_enchantment") + ": " + item.EnchantmentBehaviour.Description
                                   .ToString(new StringVariableContext(this.character.Entity));
        }

        private void CreateConsumeDescription(Item item)
        {
            if (item.ConsumeDescription.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Text = I18N.Instance.Get("ui_using") + ": " + item.ConsumeDescription;
            description.Color = Color.green;
        }

        private void CreateLearnSkillDescription(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            CreateText().Text = " ";
            CreateText().Text = skill.Description.ToString(new StringVariableContext(this.character.Entity, skill));

            if (skill.GetCost(ResourceType.ActionPoint) > 0 || skill.GetCost(ResourceType.Rage) > 0 || skill.Cooldown > 0)
            {
                CreateCost().Refresh(skill);
            }

            CreateSkillRequirements(skill);
            CreateSkillSets(skill);
        }

        private void CreateSkillRequirements(Skill skill)
        {
            if (!skill.HaveEquipmentRequirements() && !skill.RequiresDualWielding())
            {
                return;
            }

            CreateText().Text = " ";

            if (skill.HaveEquipmentRequirements())
            {
                var text = CreateText();
                text.Color = skill.EquipmentRequirementsMet() ? Color.white : Color.red;
                text.Text = $"{I18N.Instance.Get("ui_requires")}: {skill.RequiredItemCategory.Name}";
            }

            if (skill.RequiresDualWielding())
            {
                var text = CreateText();
                text.Color = skill.DualWieldRequirementMet() ? Color.white : Color.red;
                text.Text = $"{I18N.Instance.Get("ui_requires")}: {I18N.Instance.Get("ui_dual_wield")}";
            }
        }

        private void CreateSkillSets(Skill skill)
        {
            if (skill.Sets.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";
            CreateSeparator();
            CreateText().Text = " ";

            foreach (var set in skill.Sets)
            {
                CreateSet().Construct(skill.Caster, set);
                CreateText().Text = " ";
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
        }

        private SkillTooltipSet CreateSet()
        {
            return this.setPool.Spawn();
        }

        private void CreateWeaponSkillDescription(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Color = Color.green;
            description.Text = $"{skill.Name}: " + skill.Description.ToString(new StringVariableContext(this.character.Entity, skill));

            if (skill.GetCost(ResourceType.ActionPoint) > 0 || skill.Cooldown > 0)
            {
                CreateCost().Refresh(skill);
            }
        }

        private void CreateUnlockSkillDescription(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Color = Color.green;
            description.Text = $"{I18N.Instance.Translate("ui_skill")}: {skill.Description.ToString(new StringVariableContext(this.character.Entity, skill))}";

            if (skill.GetCost(ResourceType.ActionPoint) > 0 || skill.Cooldown > 0)
            {
                CreateCost().Refresh(skill);
            }

            CreateSkillRequirements(skill);
            CreateSkillSets(skill);
        }

        private void CreateHeader(Item item)
        {
            if (this.header == null)
            {
                this.header = Instantiate(this.headerPrefab, this.container);
            }

            var title = $"<smallcaps>{item.ColoredName}</smallcaps>\n<size=70%><color=#ccc>{item.Type.Name}";

            if (item.IsBlueprint)
            {
                title += $": {item.BlueprintRecipe.Item.Type.Name}";
            }

            if (item.IsOneHandedWeapon)
            {
                if (item.SlotType == EquipmentSlotType.MainHand)
                {
                    title += $" ({I18N.Instance.Get("ui_right_hand")})";
                }

                if (item.SlotType == EquipmentSlotType.OffHand)
                {
                    title += $" ({I18N.Instance.Get("ui_left_hand")})";
                }
            }

            if (item.IsUniqueEquipped)
            {
                title += $"\n<size=70%><color=#ccc>{I18N.Instance.Get("ui_unique_equipped")}";
            }

            if (item.WeaponSkillA?.Type == SkillType.Weapon)
            {
                title += $"\n<size=70%><color=#ccc>{item.WeaponSkillA.Category.Name}";
            }

            this.header.Construct(Resources.Load<Sprite>(item.Icon), title);
        }

        private void CreateStars(Item item)
        {
            if (this.stars == null)
            {
                this.stars = Instantiate(this.starsPrefab, this.container);
            }

            this.stars.gameObject.SetActive(item.ForgeLevel > 0);
            this.stars.Construct(item.ForgeLevel);
        }

        private ItemTooltipDifference CreateDifference()
        {
            return this.differencePool.Spawn();
        }

        private ItemTooltipSocket CreateSocket()
        {
            return this.socketPool.Spawn();
        }

        private SkillTooltipCost CreateCost()
        {
            return this.costPool.Spawn();
        }

        private TextWithIcon CreateTextWithIcon()
        {
            var icon = this.textWithIconPool.Spawn();
            icon.Icon.gameObject.SetActive(true);
            icon.Text = "";

            return icon;
        }

        private void CreateRuneSocket(Item rune, Item item, int index)
        {
            var instance = this.runePool.Spawn();
            instance.Construct(rune, Item.DetermineRuneTypeByIndex(index, item));
        }

        private CustomText CreateText()
        {
            var text = this.textPool.Spawn();
            text.Color = Color.white;
            text.Style = FontStyles.Normal;
            text.Alignment = TextAlignmentOptions.MidlineLeft;

            return text;
        }

        private void CreateSeparator()
        {
            this.separatorPool.Spawn();
        }
    }
}