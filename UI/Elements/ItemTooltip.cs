using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
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
        [SerializeField] private Sprite emptySocket;
        [SerializeField] private CustomText textPrefab;
        [SerializeField] private ItemTooltipDifference differencePrefab;
        [SerializeField] private GameObject separatorPrefab;
        [SerializeField] private ItemTooltipHeader headerPrefab;
        [SerializeField] private ItemTooltipSocket socketPrefab;
        [SerializeField] private SkillTooltipCost costPrefab;
        [SerializeField] private ItemTooltipPrice pricePrefab;
        [SerializeField] private Transform container;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private Character character;

        private ItemTooltipHeader header;
        private ItemTooltipPrice price;

        private MonoBehaviourPool<ItemTooltipDifference> differencePool;
        private MonoBehaviourPool<ItemTooltipSocket> socketPool;
        private MonoBehaviourPool<SkillTooltipCost> costPool;
        private MonoBehaviourPool<CustomText> textPool;
        private GameObjectPool separatorPool;

        private bool isInitialized;

        private void Start()
        {
            Initialize();
            Instance.Hide();
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
            this.separatorPool = GameObjectPool.Factory(this.separatorPrefab, this.container);

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
            this.socketPool.Clear();
            this.costPool.Clear();
            this.textPool.Clear();
            this.separatorPool.Clear();
        }

        private void OnCharacterSelected(Character character)
        {
            this.character = character;
        }

        public void Show(Item item, RectTransform rect = null)
        {
            gameObject.SetActive(true);

            Clear();

            CreateHeader(item);

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

            if (!isBlueprint && item.IsIdentified && (item.IsArmor || item.IsWeapon || item.IsJewelry))
            {
                CreateQuality(item);
            }

            CreateFixedModifiers(item);
            CreateModifiers(item);
            CreateSockets(item);
            CreateWeaponSkillDescription(item.WeaponSkillA);
            CreateWeaponSkillDescription(item.WeaponSkillB);
            CreatePassiveDescription(item);
            CreateUnlockSkillDescription(item);
            CreateConsumeDescription(item);
            CreateLore(item);
            CreateSetInfo(item);
            CreateDifference(item);
            CreateInfo(item);
            CreatePrice(item);

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);

            if (rect == null)
            {
                return;
            }

            this.rectTransform.MoveTooltip(rect, this.parentRectTransform);
            this.rectTransform.ClampPositionToParent();
        }

        private void CreateQuality(Item item)
        {
            CreateText().Text = " ";

            CreateText().Text = item.Suffix == null
                ? $"{I18N.Instance.Get("ui_quality")}: 100%"
                : $"{I18N.Instance.Get("ui_quality")}: {item.Suffix.Quality * 100:F0}%";
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
        }

        private void Clear()
        {
            this.differencePool.DespawnAll();
            this.separatorPool.DespawnAll();
            this.socketPool.DespawnAll();
            this.costPool.DespawnAll();
            this.textPool.DespawnAll();
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

                row.Text = "    " + setItem.Name;
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
            if (!item.IsIdentified)
            {
                return;
            }

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
            CreateDifference().Construct(delta > 0, $"{(int) Mathf.Abs(delta)} {attribute.Name}");
        }

        private void CreatePropertyDifferenceRow(Property property, float delta)
        {
            CreateDifference().Construct(delta > 0, $"{Property.ValueString(property.Type, Mathf.Abs(delta))} {property.Name}");
        }

        private void CreateInfo(Item item)
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

        private void CreatePrice(Item item)
        {
            if (item.GetPrice().Count == 0)
            {
                this.price.gameObject.SetActive(false);
                return;
            }

            this.price.gameObject.SetActive(true);
            this.price.Refresh(item.GetPrice());
            this.price.transform.SetAsLastSibling();
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
                var socketBonusText =
                    string.Join(", ", socket.GetAttributeModifiers().Select(modifier => modifier.ToString())) +
                    string.Join(", ", socket.GetPropertyModifiers().Select(modifier => modifier.ToString()));

                CreateSocket().Construct(
                    socket.IsEmpty ? this.emptySocket : Resources.Load<Sprite>(socket.Icon),
                    socket.IsEmpty ? I18N.Instance.Get("ui_empty_socket") : socketBonusText
                );
            }
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

        private void CreateModifiers(Item item)
        {
            if (!item.IsIdentified)
            {
                CreateText().Text = " ";
                CreateText().Text = $"[{I18N.Instance.Get("ui_random_attributes")}]";
                return;
            }

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
                CreateText().Text = $"+{group.Sum(mod => mod.GetAmount())} {group.First().Attribute.Name}";
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
            lore.Style = FontStyles.Italic;
            lore.Color = Color.gray;
            lore.Text = item.Lore;
        }

        private void CreatePassiveDescription(Item item)
        {
            if (item.PassiveDescription.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Text = I18N.Instance.Get("ui_passive") + ": " + item.PassiveDescription.ToString(new StringVariableContext(this.character.Entity));
            description.Color = Color.green;
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

        private void CreateUnlockSkillDescription(Item item)
        {
            if (item.UnlockSkill == null)
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Color = Color.green;
            description.Text = $"{I18N.Instance.Get("ui_skill")}: " +
                               item.UnlockSkill.Description.ToString(new StringVariableContext(this.character.Entity, item.UnlockSkill));

            if (item.UnlockSkill.GetCost(ResourceType.ActionPoint) > 0 || item.UnlockSkill.Cooldown > 0)
            {
                CreateCost().Refresh(item.UnlockSkill);
            }
        }

        private void CreateHeader(Item item)
        {
            if (this.header == null)
            {
                this.header = Instantiate(this.headerPrefab, this.container);
            }

            var itemName = item.IsIdentified ? item.ColoredName : item.ColoredBaseName;
            var title = $"<smallcaps>{itemName}</smallcaps>\n<size=70%><color=#ccc>{item.Type.Name}";

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