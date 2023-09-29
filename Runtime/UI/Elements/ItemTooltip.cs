using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
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
        public event Action<ItemTooltip>? Shown;
        public event Action<ItemTooltip>? Hidden;

        public bool DisplayPrice { get; set; } = true;
        public bool HasOverflow { get; private set; }

        [SerializeField]
        private Sprite m_EmptySocket = null!;

        [SerializeField]
        private CustomText m_TextPrefab = null!;

        [SerializeField]
        private TextWithIcon m_TextWithIconPrefab = null!;

        [SerializeField]
        private ItemTooltipRune m_RunePrefab = null!;

        [SerializeField]
        private ItemTooltipDifference m_DifferencePrefab = null!;

        [SerializeField]
        private GameObject m_SeparatorPrefab = null!;

        [SerializeField]
        private SkillTooltipSet m_SetPrefab = null!;

        [SerializeField]
        private ItemTooltipHeader m_HeaderPrefab = null!;

        [SerializeField]
        private ItemTooltipSocket m_SocketPrefab = null!;

        [SerializeField]
        private SkillTooltipCost m_CostPrefab = null!;

        [SerializeField]
        private ItemTooltipPrice m_PricePrefab = null!;

        [SerializeField]
        private Transform m_Container = null!;

        private RectTransform m_RectTransform;
        private RectTransform m_ParentRectTransform;

        private ItemTooltipHeader m_Header;
        private ItemTooltipStars m_Stars;
        private ItemTooltipPrice m_Price;

        private MonoBehaviourPool<ItemTooltipDifference> m_DifferencePool;
        private MonoBehaviourPool<ItemTooltipSocket> m_SocketPool;
        private MonoBehaviourPool<SkillTooltipCost> m_CostPool;
        private MonoBehaviourPool<CustomText> m_TextPool;
        private MonoBehaviourPool<TextWithIcon> m_TextWithIconPool;
        private MonoBehaviourPool<ItemTooltipRune> m_InscriptionPool;
        private MonoBehaviourPool<SkillTooltipSet> m_SetPool;
        private GameObjectPool m_SeparatorPool;

        private bool m_IsInitialized;

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
            if (Math.Abs(pivotY - m_RectTransform.pivot.y) < Mathf.Epsilon)
            {
                return;
            }

            m_RectTransform.ChangePivot(new Vector2(m_RectTransform.pivot.x, pivotY));
            m_RectTransform.ClampPositionToParent(m_ParentRectTransform);
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_Price = Instantiate(m_PricePrefab, m_Container);
            m_Price.Initialize();

            m_DifferencePool = MonoBehaviourPool<ItemTooltipDifference>.Factory(m_DifferencePrefab, m_Container);
            m_SocketPool = MonoBehaviourPool<ItemTooltipSocket>.Factory(m_SocketPrefab, m_Container);
            m_CostPool = MonoBehaviourPool<SkillTooltipCost>.Factory(m_CostPrefab, m_Container);
            m_TextPool = MonoBehaviourPool<CustomText>.Factory(m_TextPrefab, m_Container);
            m_TextWithIconPool = MonoBehaviourPool<TextWithIcon>.Factory(m_TextWithIconPrefab, m_Container);
            m_InscriptionPool = MonoBehaviourPool<ItemTooltipRune>.Factory(m_RunePrefab, m_Container);
            m_SeparatorPool = GameObjectPool.Factory(m_SeparatorPrefab, m_Container);
            m_SetPool = MonoBehaviourPool<SkillTooltipSet>.Factory(m_SetPrefab, m_Container);

            m_RectTransform = GetComponent<RectTransform>();
            m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();

            m_IsInitialized = true;
        }

        public void Terminate()
        {
            if (!m_IsInitialized)
            {
                return;
            }

            m_Price.Terminate();

            m_DifferencePool.Clear();
            m_SeparatorPool.Clear();
            m_SocketPool.Clear();
            m_CostPool.Clear();
            m_TextPool.Clear();
            m_SetPool.Clear();
            m_TextWithIconPool.Clear();
            m_InscriptionPool.Clear();
        }

        public void Show(Item item, RectTransform rect = null)
        {
            gameObject.SetActive(true);

            Clear();

            CreateHeader(item);

            if (item.IsRelic)
            {
                CreateRelicAlreadyKnownNotification(item);
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
            CreateLore(item);
            CreateSetInfo(item);
            CreateAffixes(item);
            CreateInscriptions(item);
            CreateDifference(item);
            CreateWarnings(item);
            CreatePrice(item);

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);

            if (rect != null)
            {
                m_RectTransform.MoveTooltip(rect, m_ParentRectTransform);
                m_RectTransform.ClampPositionToParent();
            }

            HasOverflow = m_RectTransform.rect.height - m_ParentRectTransform.rect.height > 0;

            if (HasOverflow)
            {
                ChangePivotY(0);
            }

            Shown?.Invoke(this);
        }

        private void CreateRelicAlreadyKnownNotification(Item item)
        {
            var isRelicAlreadyUnlocked = Game.Instance.Character.Entity.GetComponent<ReliquaryComponent>().Available
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
            m_DifferencePool.DespawnAll();
            m_SeparatorPool.DespawnAll();
            m_SocketPool.DespawnAll();
            m_CostPool.DespawnAll();
            m_TextPool.DespawnAll();
            m_SetPool.DespawnAll();
            m_TextWithIconPool.DespawnAll();
            m_InscriptionPool.DespawnAll();
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

            var inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
            var equipment = Game.Instance.Character.Entity.GetComponent<EquipmentComponent>();

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
                    row.Text = $"    {behaviour.Description.ToString(new StringVariableContext(Game.Instance.Character.Entity)).Replace("\n", "\n    ")}";
                }
            }
        }

        private void CreateDifference(Item item)
        {
            var equipment = Game.Instance.Character.Entity.GetComponent<EquipmentComponent>();

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

        private void CreateWarnings(Item item)
        {
            if (item.Rarity.Id != Constants.c_ItemRarityIdVision || !item.IsEquipment)
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
            if (!DisplayPrice || item.GetPrice().All(x => x.Amount == 0))
            {
                m_Price.gameObject.SetActive(false);
                return;
            }

            m_Price.gameObject.SetActive(true);
            m_Price.Refresh(item.GetPrice());
            m_Price.transform.SetAsLastSibling();
        }

        private void CreateInscriptions(Item item)
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
                    socket.IsEmpty ? m_EmptySocket : Resources.Load<Sprite>(socket.Icon),
                    socket.IsEmpty ? I18N.Instance.Get("ui_empty_socket") : string.Join(", ", socketBonusParts)
                );
            }
        }

        private void CreateModifiers(Item item)
        {
            var attributeModifiers = item.GetAttributeModifiers(false);
            var propertyModifiers = item.GetPropertyModifiers(false);

            if (attributeModifiers.Count > 0 || propertyModifiers.Count > 0)
            {
                CreateText().Text = " ";
                CreateAttributeModifiers(attributeModifiers);
                CreatePropertyModifiers(propertyModifiers);
            }

            if (item is { HasRandomSuffix: true, Suffix: null })
            {
                CreateText().Text = " ";
                CreateText().Text = I18N.Instance.Translate("ui_random_attributes");
            }
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
            CreatePassiveDescriptionText(item.PassiveDescription.ToString(new StringVariableContext(Game.Instance.Character.Entity)));
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
                .ToString(new StringVariableContext(Game.Instance.Character.Entity));
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

        private void CreateWeaponSkillDescription(Skill skill)
        {
            if (skill == null)
            {
                return;
            }

            CreateText().Text = " ";

            var description = CreateText();
            description.Color = Color.green;
            description.Text = $"{skill.Name}: " + skill.Description.ToString(new StringVariableContext(Game.Instance.Character.Entity, skill));

            if (skill.GetCost(ResourceType.ActionPoint) > 0 || skill.Cooldown > 0)
            {
                CreateCost().Refresh(skill);
            }
        }

        private void CreateHeader(Item item)
        {
            if (m_Header == null)
            {
                m_Header = Instantiate(m_HeaderPrefab, m_Container);
            }

            var title = $"<smallcaps>{item.ColoredName}</smallcaps>\n<size=70%><color=#ccc>{item.Type.Name.ToString()}";

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

            m_Header.Construct(Resources.Load<Sprite>(item.Icon), title);
        }

        private ItemTooltipDifference CreateDifference()
        {
            return m_DifferencePool.Spawn();
        }

        private ItemTooltipSocket CreateSocket()
        {
            return m_SocketPool.Spawn();
        }

        private SkillTooltipCost CreateCost()
        {
            return m_CostPool.Spawn();
        }

        private TextWithIcon CreateTextWithIcon()
        {
            var icon = m_TextWithIconPool.Spawn();
            icon.Icon.gameObject.SetActive(true);
            icon.Text = "";

            return icon;
        }

        private ItemTooltipRune CreateRuneSocket(Item inscription, Item item, int index)
        {
            var instance = m_InscriptionPool.Spawn();
            instance.Construct(inscription, Item.DetermineRuneTypeByIndex(index, item));

            return instance;
        }

        private CustomText CreateText()
        {
            var text = m_TextPool.Spawn();
            text.Color = Color.white;
            text.Style = FontStyles.Normal;
            text.Alignment = TextAlignmentOptions.MidlineLeft;

            return text;
        }

        private void CreateSeparator()
        {
            m_SeparatorPool.Spawn();
        }
    }
}