using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Modifiers;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Components
{
    public class EquipmentComponent : Component
    {
        public static event Action AnyWeaponSetSwapped;

        public event Action<Item> ItemEquipped;
        public event Action<Item> ItemUnequipped;
        public event Action ItemsSwapped;
        public event Action WeaponSetSwapped;

        public List<EquipmentSlot> Slots { get; private set; }

        private readonly Dictionary<Item, List<Behaviour>> m_ItemsBehaviours = new();
        private readonly Dictionary<Item, List<AttributeModifier>> m_ItemsAttributeModifiers = new();
        private readonly Dictionary<Item, List<PropertyModifier>> m_ItemsPropertyModifiers = new();

        public bool IsOnAltWeaponSet { get; private set; }
        public List<Item> AltWeaponSet { get; } = new() { Item.CreateEmpty(), Item.CreateEmpty() };

        private ExperienceComponent m_Experience;
        private HealthComponent m_Health;
        private InventoryComponent m_Inventory;
        private AttributesComponent m_Attributes;
        private PropertiesComponent m_Properties;
        private BehavioursComponent m_Behaviours;
        private ActorComponent m_Actor;

        public EquipmentComponent Construct()
        {
            Slots = new List<EquipmentSlot>
            {
                new("Helm", EquipmentSlotType.Head),
                new("Armor", EquipmentSlotType.Chest),
                new("Gloves", EquipmentSlotType.Hands),
                new("Belt", EquipmentSlotType.Belt),
                new("Boots", EquipmentSlotType.Feet),
                new("Necklace", EquipmentSlotType.Neck),
                new("Ring_1", EquipmentSlotType.Ring),
                new("Ring_2", EquipmentSlotType.Ring),
                new("Accessory_1", EquipmentSlotType.Relic),
                new("Accessory_2", EquipmentSlotType.Relic),
                new("Main_Hand", EquipmentSlotType.MainHand),
                new("Off_Hand", EquipmentSlotType.OffHand),
            };

            return this;
        }

        protected override void OnInitialize()
        {
            Item.AnyItemStatsUpdated += OnAnyItemStatsUpdated;
            CombatEncounter.AnyCombatRoundStarted += OnRoundStarted;

            m_Health = GetComponent<HealthComponent>();
            m_Experience = GetComponent<ExperienceComponent>();
            m_Inventory = GetComponent<InventoryComponent>();
            m_Attributes = GetComponent<AttributesComponent>();
            m_Properties = GetComponent<PropertiesComponent>();
            m_Behaviours = GetComponent<BehavioursComponent>();
            m_Actor = GetComponent<ActorComponent>();
        }

        protected override void OnTerminate()
        {
            Item.AnyItemStatsUpdated -= OnAnyItemStatsUpdated;
            CombatEncounter.AnyCombatRoundStarted -= OnRoundStarted;
        }

        private void OnRoundStarted(CombatEncounter combat)
        {
            // TODO: Move to SpellbookComponent

            foreach (var item in AltWeaponSet)
            {
                item.WeaponSkillA?.ReduceCooldown(1);
                item.WeaponSkillB?.ReduceCooldown(1);
            }
        }

        public void ResetAltWeaponCooldown()
        {
            // TODO: Move to SpellbookComponent

            foreach (var item in AltWeaponSet)
            {
                item.WeaponSkillA?.ResetCooldown();
                item.WeaponSkillB?.ResetCooldown();
            }
        }

        public void SetAltWeaponSet(List<Item> items)
        {
            for (var i = 0; i < 2; i++)
            {
                var item = items.IndexInBounds(i) ? items[i] : Item.CreateEmpty();
                item.Equipment = this;
                item.Inventory = m_Inventory;
                item.EquipmentSlot = Slots.First(s => s.Type == (i == 0 ? EquipmentSlotType.MainHand : EquipmentSlotType.OffHand));
                item.ChangeOwner(gameObject);

                AltWeaponSet[i] = item;
            }
        }

        public void SwapWeapon()
        {
            if (CombatEncounter.Active?.IsEntityTurn(gameObject) == false)
            {
                return;
            }

            var fraction = m_Health.HealthFraction;

            var mh = Slots.First(s => s.Type == EquipmentSlotType.MainHand);
            var oh = Slots.First(s => s.Type == EquipmentSlotType.OffHand);

            var current = new List<Item> { mh.Item, oh.Item };

            if (!mh.IsEmpty)
            {
                RemoveItemBonuses(mh.Item, mh);
                RemoveVisuals(mh.Item, mh);
                mh.Clear();
                ItemUnequipped?.Invoke(mh.Item);
            }

            if (!oh.IsEmpty)
            {
                RemoveItemBonuses(oh.Item, oh);
                RemoveVisuals(oh.Item, oh);
                oh.Clear();
                ItemUnequipped?.Invoke(oh.Item);
            }

            mh.Put(AltWeaponSet[0]);
            oh.Put(AltWeaponSet[1]);

            IsOnAltWeaponSet = !IsOnAltWeaponSet;

            SetAltWeaponSet(current);

            if (!mh.IsEmpty)
            {
                ApplyItemBonuses(mh.Item, mh);
                ApplyVisuals(mh.Item, mh);
                ItemEquipped?.Invoke(mh.Item);
            }

            if (!oh.IsEmpty)
            {
                ApplyItemBonuses(oh.Item, oh);
                ApplyVisuals(oh.Item, oh);
                ItemEquipped?.Invoke(oh.Item);
            }

            m_Health.Health = m_Health.HealthMax * fraction;

            WeaponSetSwapped?.Invoke();
            AnyWeaponSetSwapped?.Invoke();
        }

        private void OnAnyItemStatsUpdated(Item item)
        {
            if (!IsEquipped(item))
            {
                return;
            }

            RemoveStatModifiers(item);
            RemoveBehaviours(item);

            ApplyStatModifiers(item);
            ApplyBehaviours(item);
        }

        public Item GetPrimaryOrSecondaryWeapon()
        {
            return Slots.Where(slot => slot.Type == EquipmentSlotType.MainHand ||
                                       slot.Type == EquipmentSlotType.OffHand &&
                                       !slot.Item.IsEmpty)
                .OrderBy(slot => slot.Type != EquipmentSlotType.MainHand)
                .Select(slot => slot.Item)
                .FirstOrDefault();
        }

        public Item GetSecondaryOrPrimaryWeapon()
        {
            return Slots.Where(slot => slot.Type == EquipmentSlotType.MainHand ||
                                       slot.Type == EquipmentSlotType.OffHand &&
                                       !slot.Item.IsEmpty)
                .OrderBy(slot => slot.Type == EquipmentSlotType.MainHand)
                .Select(slot => slot.Item)
                .FirstOrDefault();
        }

        public bool IsDualWielding()
        {
            var mh = Slots.First(s => s.Type == EquipmentSlotType.MainHand);
            var oh = Slots.First(s => s.Type == EquipmentSlotType.OffHand);

            if (mh.Item.IsEmpty || oh.Item.IsEmpty)
            {
                return false;
            }

            return mh.Item.IsWeapon && oh.Item.Type.Type != ItemTypeType.Shield && oh.Item.Type.Type != ItemTypeType.OffHand;
        }

        public int GetItemSetPiecesEquipped(ItemSet set)
        {
            return set.Items.Count(item => IsEquipped(item.Id));
        }

        public int GetItemSetPiecesObtained(ItemSet set)
        {
            return Slots.Select(slot => slot.Item)
                .Concat(m_Inventory.Items)
                .DistinctBy(item => item.Id)
                .Count(item => set.Items.Any(setItem => setItem.Id == item.Id));
        }

        public List<Item> UnequipWeapon()
        {
            var weapon = new List<Item>();

            var mh = Slots.First(s => s.Type == EquipmentSlotType.MainHand).Item;

            if (!mh.IsEmpty)
            {
                weapon.Add(mh);
                Unequip(mh);
            }

            var oh = Slots.First(s => s.Type == EquipmentSlotType.OffHand).Item;

            if (!oh.IsEmpty)
            {
                weapon.Add(oh);
                Unequip(oh);
            }

            return weapon;
        }

        public void Equip(Item item)
        {
            var slot = FindSuitableSlot(item);

            if (slot == null)
            {
                Debug.LogWarning($"No suitable slot for item {item.Name} with type {item.Type.Name}");
                return;
            }

            EquipIntoSlot(item, slot);
        }

        public void EquipIntoSlot(Item item, EquipmentSlot slot)
        {
            if (item.Affixes.Count > 0 && Slots.Any(x => x.Item.Affixes.Count > 0))
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Translate("exception_equip_more_than_one_item_of_type"));
                return;
            }

            if (item.IsEmpty)
            {
                Debug.LogError("Trying to equip empty item.");
                return;
            }

            if (!slot.CanEquip(item))
            {
                return;
            }

            if (IsEquipped(item))
            {
                Unequip(item);
            }

            if (!slot.IsEmpty)
            {
                Unequip(slot.Item);
            }

            item.Type.EquipmentStrategy.Prepare(item, slot, this);
            item.EquipmentSlot = slot;
            item.Equipment = this;

            item.Inventory?.Remove(item);
            item.Inventory = m_Inventory;

            slot.Put(item);

            ApplyItemBonuses(item, slot);
            ApplyVisuals(item, slot);

            ItemEquipped?.Invoke(item);
        }

        public void Unequip(Item item, Item target = null)
        {
            if (item.IsEmpty)
            {
                throw new Exception("Trying to unequip empty item.");
            }

            if (item.IsAnySkillOnCooldown())
            {
                throw new SkillIsOnCooldownException();
            }

            var slot = FindContainingSlot(item);

            if (slot == null)
            {
                throw new Exception($"Item {item.Name} #{item.GetHashCode()} is not equipped.");
            }

            item.Equipment = null;

            if (target == null || target.Inventory == null)
            {
                item.Inventory.Pickup(slot.Item);
            }
            else
            {
                target.Inventory.Pickup(slot.Item, target);
            }

            slot.Clear();

            RemoveItemBonuses(item, slot);
            RemoveVisuals(item, slot);

            item.EquipmentSlot = null;

            ItemUnequipped?.Invoke(item);
        }

        private void ApplyVisuals(Item item, EquipmentSlot slot)
        {
            if (item.Skin != null)
            {
                m_Actor.Model.ApplySkin(item.Skin);
            }

            AttachmentPoint attachmentPoint;

            switch (slot.Type)
            {
                case EquipmentSlotType.MainHand:
                    attachmentPoint = AttachmentPoint.RightHand;
                    break;
                case EquipmentSlotType.OffHand:
                    attachmentPoint = AttachmentPoint.LeftHand;
                    break;
                default:
                    attachmentPoint = AttachmentPoint.None;
                    break;
            }

            m_Actor.CreateEquipmentAttachments(slot, item.Attachments, attachmentPoint);
        }

        private void RemoveVisuals(Item item, EquipmentSlot slot)
        {
            if (item.Skin != null)
            {
                m_Actor.Model.RemoveSkin(item.Skin);
            }

            if (slot.Type == EquipmentSlotType.Head)
            {
                m_Actor.Model.ShowHair();
            }

            m_Actor.DestroyAttachments(slot);
        }

        public void Swap(Item item1, Item item2)
        {
            if (item2.IsEmpty)
            {
                EquipIntoSlot(item1, FindContainingSlot(item2));
            }

            if (item1.SlotType != item2.SlotType)
            {
                return;
            }

            var index1 = Slots.IndexOf(Slots.First(slot => slot.Item.Equals(item1)));
            var index2 = Slots.IndexOf(Slots.First(slot => slot.Item.Equals(item2)));

            Unequip(item1);
            Unequip(item2);

            EquipIntoSlot(item1, Slots[index2]);
            EquipIntoSlot(item2, Slots[index1]);
        }

        public bool IsEquipped(int itemId)
        {
            return FindContainingSlot(itemId) != null;
        }

        public bool IsEquipped(Item item)
        {
            return FindContainingSlot(item) != null;
        }

        public EquipmentSlot FindContainingSlot(int itemId)
        {
            return Slots.FirstOrDefault(s => s.Item.Id == itemId);
        }

        public EquipmentSlot FindContainingSlot(Item item)
        {
            return Slots.FirstOrDefault(s => s.Item == item);
        }

        public EquipmentSlot Get(EquipmentSlotType type)
        {
            return Slots.FirstOrDefault(slot => slot.Type == type);
        }

        public EquipmentSlot FindSuitableSlot(Item item)
        {
            return Slots.Where(s => s.CanEquip(item)).OrderBy(s => !s.IsEmpty).FirstOrDefault();
        }

        private void ApplyItemBonuses(Item item, EquipmentSlot slot)
        {
            ApplyStatModifiers(item);
            ApplyBehaviours(item);
            ApplyItemSetBonuses(item);
            AddSkills(item, slot);
        }

        private void RemoveItemBonuses(Item item, EquipmentSlot slot)
        {
            RemoveStatModifiers(item);
            RemoveBehaviours(item);
            RemoveItemSetBonuses(item);
            RemoveSkills(item, slot);
        }

        private void ApplyStatModifiers(Item item)
        {
            m_ItemsAttributeModifiers.Add(item, item.GetAttributeModifiers());
            m_ItemsPropertyModifiers.Add(item, item.GetPropertyModifiers());

            foreach (var socket in item.Sockets)
            {
                m_ItemsAttributeModifiers[item].AddRange(socket.GetAttributeModifiers());
                m_ItemsPropertyModifiers[item].AddRange(socket.GetPropertyModifiers());
            }

            m_Attributes.ApplyModifiers(m_ItemsAttributeModifiers[item]);
            m_Properties.ApplyModifiers(m_ItemsPropertyModifiers[item]);
        }

        private void RemoveStatModifiers(Item item)
        {
            m_Attributes.RemoveModifiers(m_ItemsAttributeModifiers[item]);
            m_Properties.RemoveModifiers(m_ItemsPropertyModifiers[item]);

            m_ItemsAttributeModifiers.Remove(item);
            m_ItemsPropertyModifiers.Remove(item);
        }

        private void ApplyBehaviours(Item item)
        {
            if (!m_ItemsBehaviours.ContainsKey(item))
            {
                m_ItemsBehaviours.Add(item, new List<Behaviour>());
            }

            m_ItemsBehaviours[item].AddRange(item.Behaviours);
            m_ItemsBehaviours[item].AddRange(item.Affixes);

            if (item.EnchantmentBehaviour != null)
            {
                m_ItemsBehaviours[item].Add(item.EnchantmentBehaviour);
            }

            foreach (var behaviour in m_ItemsBehaviours[item])
            {
                m_Behaviours.ApplyStack(behaviour, gameObject);
            }
        }

        private void RemoveBehaviours(Item item)
        {
            if (!m_ItemsBehaviours.ContainsKey(item))
            {
                return;
            }

            foreach (var behaviour in m_ItemsBehaviours[item])
            {
                m_Behaviours.RemoveStack(behaviour.Id);
            }

            m_ItemsBehaviours[item].Clear();
        }

        private void AddSkills(Item item, EquipmentSlot slot)
        {
            var spellbook = GetComponent<SpellbookComponent>();

            if (item.WeaponSkillA != null)
            {
                item.WeaponSkillA.EquipmentSlot = slot;

                if (string.IsNullOrEmpty(item.WeaponSkillA.Icon))
                {
                    item.WeaponSkillA.Icon = item.Icon;
                }

                spellbook.Learn(
                    spellbook.Slots.First(s => s.Index == (slot.Type == EquipmentSlotType.MainHand ? 0 : 1)),
                    item.WeaponSkillA);
            }

            if (item.WeaponSkillB != null)
            {
                item.WeaponSkillB.EquipmentSlot = slot;

                if (string.IsNullOrEmpty(item.WeaponSkillB.Icon))
                {
                    item.WeaponSkillB.Icon = item.Icon;
                }

                spellbook.Learn(spellbook.Slots.First(s => s.Index == 1), item.WeaponSkillB);
            }
        }

        private void RemoveSkills(Item item, EquipmentSlot slot)
        {
            var spellbook = GetComponent<SpellbookComponent>();

            if (item.WeaponSkillA != null)
            {
                item.WeaponSkillA.EquipmentSlot = null;
                spellbook.Unlearn(item.WeaponSkillA);
            }

            if (item.WeaponSkillB != null)
            {
                item.WeaponSkillB.EquipmentSlot = null;
                spellbook.Unlearn(item.WeaponSkillB);
            }
        }

        private void ApplyItemSetBonuses(Item item)
        {
            if (item.Set == null)
            {
                return;
            }

            var equippedPieces = GetItemSetPiecesEquipped(item.Set);

            foreach (var bonus in item.Set.Behaviours)
            {
                if (equippedPieces < bonus.Key)
                {
                    continue;
                }

                foreach (var behaviour in bonus.Value)
                {
                    m_Behaviours.ApplyAllStacks(behaviour, gameObject);
                }
            }
        }

        private void RemoveItemSetBonuses(Item item)
        {
            if (item.Set == null)
            {
                return;
            }

            var equippedPieces = GetItemSetPiecesEquipped(item.Set);

            foreach (var bonus in item.Set.Behaviours)
            {
                if (equippedPieces >= bonus.Key)
                {
                    continue;
                }

                foreach (var behaviour in bonus.Value)
                {
                    m_Behaviours.RemoveAllStacks(behaviour.Id);
                }
            }
        }
    }
}