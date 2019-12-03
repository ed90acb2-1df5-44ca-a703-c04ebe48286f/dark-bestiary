using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Modifiers;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class EquipmentComponent : Component
    {
        public event Payload<Item> ItemEquipped;
        public event Payload<Item> ItemUnequipped;
        public event Payload ItemsSwapped;

        public List<EquipmentSlot> Slots { get; private set; }

        private readonly Dictionary<Item, List<AttributeModifier>> itemsAttributeModifiers =
            new Dictionary<Item, List<AttributeModifier>>();
        private readonly Dictionary<Item, List<PropertyModifier>> itemsPropertyModifiers =
            new Dictionary<Item, List<PropertyModifier>>();

        public bool IsOnAltWeaponSet { get; private set; }
        public List<Item> AltWeaponSet { get; private set; } = new List<Item>{Item.CreateEmpty(), Item.CreateEmpty()};

        private ExperienceComponent experience;
        private InventoryComponent inventory;
        private AttributesComponent attributes;
        private PropertiesComponent properties;
        private BehavioursComponent behaviours;
        private ActorComponent actor;

        public EquipmentComponent Construct()
        {
            Slots = new List<EquipmentSlot>
            {
                new EquipmentSlot("Helm", EquipmentSlotType.Head),
                new EquipmentSlot("Necklace", EquipmentSlotType.Neck),
                new EquipmentSlot("Armor", EquipmentSlotType.Chest),
                new EquipmentSlot("Belt", EquipmentSlotType.Belt),
                new EquipmentSlot("Boots", EquipmentSlotType.Feet),
                new EquipmentSlot("Gloves", EquipmentSlotType.Hands),
                new EquipmentSlot("Ring_1", EquipmentSlotType.Ring),
                new EquipmentSlot("Ring_2", EquipmentSlotType.Ring),
                new EquipmentSlot("Accessory_1", EquipmentSlotType.Relic),
                new EquipmentSlot("Accessory_2", EquipmentSlotType.Relic),
                new EquipmentSlot("Main_Hand", EquipmentSlotType.MainHand),
                new EquipmentSlot("Off_Hand", EquipmentSlotType.OffHand),
            };

            return this;
        }

        protected override void OnInitialize()
        {
            Item.AnyItemStatsUpdated += OnAnyItemStatsUpdated;

            this.experience = GetComponent<ExperienceComponent>();
            this.inventory = GetComponent<InventoryComponent>();
            this.attributes = GetComponent<AttributesComponent>();
            this.properties = GetComponent<PropertiesComponent>();
            this.behaviours = GetComponent<BehavioursComponent>();
            this.actor = GetComponent<ActorComponent>();
        }

        public void SetAltWeaponSet(List<Item> items)
        {
            for (var i = 0; i < 2; i++)
            {
                var item = items.IndexInBounds(i) ? items[i] : Item.CreateEmpty();
                item.Equipment = this;
                item.Inventory = this.inventory;
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

            var mh = Slots.First(s => s.Type == EquipmentSlotType.MainHand);
            var oh = Slots.First(s => s.Type == EquipmentSlotType.OffHand);

            if (mh.Item.IsAnySkillOnCooldown() || oh.Item.IsAnySkillOnCooldown())
            {
                throw new SkillIsOnCooldownException();
            }

            var current = new List<Item>{mh.Item, oh.Item};

            if (!mh.IsEmpty)
            {
                RemoveItemBonuses(mh.Item, mh);
                RemoveVisuals(mh.Item, mh);
            }

            if (!oh.IsEmpty)
            {
                RemoveItemBonuses(oh.Item, oh);
                RemoveVisuals(oh.Item, oh);
            }

            mh.Put(AltWeaponSet[0]);
            oh.Put(AltWeaponSet[1]);

            if (!mh.IsEmpty)
            {
                ApplyItemBonuses(mh.Item, mh);
                ApplyVisuals(mh.Item, mh);
            }

            if (!oh.IsEmpty)
            {
                ApplyItemBonuses(oh.Item, oh);
                ApplyVisuals(oh.Item, oh);
            }

            IsOnAltWeaponSet = !IsOnAltWeaponSet;

            SetAltWeaponSet(current);

            ItemEquipped?.Invoke(mh.Item);
            ItemEquipped?.Invoke(oh.Item);
        }

        private void OnAnyItemStatsUpdated(Item item)
        {
            if (!IsEquipped(item))
            {
                return;
            }

            RemoveStatModifiers(item);
            ApplyStatModifiers(item);
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

            return mh.Item.IsWeapon &&
                   oh.Item.Type.Type != ItemTypeType.Shield && oh.Item.Type.Type != ItemTypeType.OffHand;
        }

        public int GetItemSetPiecesEquipped(ItemSet set)
        {
            return set.Items.Count(item => IsEquipped(item.Id));
        }

        public int GetItemSetPiecesObtained(ItemSet set)
        {
            return Slots.Select(slot => slot.Item)
                .Concat(this.inventory.Items)
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
            if (this.experience != null && item.RequiredLevel > this.experience.Experience.Level)
            {
                return;
            }

            if (item.IsEmpty)
            {
                Debug.LogError("Trying to equip empty item.");
                return;
            }

            if (!slot.CanEquip(item))
            {
                Debug.LogError($"Can't equip item {item.Name} into slot with type {slot.Type}");
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
            item.Inventory = this.inventory;

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
                this.actor.Model.ApplySkin(item.Skin);
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

            if (slot.Type == EquipmentSlotType.Head && this.actor.IsHelmVisible)
            {
                this.actor.Model.HideHair();
            }

            this.actor.CreateAttachments(slot, item.Attachments, attachmentPoint);
        }

        private void RemoveVisuals(Item item, EquipmentSlot slot)
        {
            if (item.Skin != null)
            {
                this.actor.Model.RemoveSkin(item.Skin);
            }

            if (slot.Type == EquipmentSlotType.Head)
            {
                this.actor.Model.ShowHair();
            }

            this.actor.DestroyAttachments(slot);
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
            this.itemsAttributeModifiers.Add(item, item.GetAttributeModifiers());
            this.itemsPropertyModifiers.Add(item, item.GetPropertyModifiers());

            foreach (var socket in item.Sockets)
            {
                this.itemsAttributeModifiers[item].AddRange(socket.GetAttributeModifiers());
                this.itemsPropertyModifiers[item].AddRange(socket.GetPropertyModifiers());
            }

            this.attributes.ApplyModifiers(this.itemsAttributeModifiers[item]);
            this.properties.ApplyModifiers(this.itemsPropertyModifiers[item]);
        }

        private void RemoveStatModifiers(Item item)
        {
            this.attributes.RemoveModifiers(this.itemsAttributeModifiers[item]);
            this.properties.RemoveModifiers(this.itemsPropertyModifiers[item]);

            this.itemsAttributeModifiers.Remove(item);
            this.itemsPropertyModifiers.Remove(item);
        }

        private void ApplyBehaviours(Item item)
        {
            foreach (var behaviour in item.Behaviours)
            {
                this.behaviours.Apply(behaviour, gameObject);
            }
        }

        private void RemoveBehaviours(Item item)
        {
            foreach (var behaviour in item.Behaviours)
            {
                this.behaviours.RemoveAllStacks(behaviour.Id);
            }
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

                spellbook.PlaceOnActionBar(
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

                spellbook.PlaceOnActionBar(spellbook.Slots.First(s => s.Index == 1), item.WeaponSkillB);
            }

            if (item.UnlockSkill != null)
            {
                item.UnlockSkill.EquipmentSlot = slot;
                item.UnlockSkill.Icon = item.Icon;

                spellbook.Add(item.UnlockSkill);
            }
        }

        private void RemoveSkills(Item item, EquipmentSlot slot)
        {
            var spellbook = GetComponent<SpellbookComponent>();

            if (item.WeaponSkillA != null)
            {
                item.WeaponSkillA.EquipmentSlot = null;
                spellbook.RemoveFromActionBar(item.WeaponSkillA);
            }

            if (item.WeaponSkillB != null)
            {
                item.WeaponSkillB.EquipmentSlot = null;
                spellbook.RemoveFromActionBar(item.WeaponSkillB);
            }

            if (item.UnlockSkill != null)
            {
                item.UnlockSkill.EquipmentSlot = null;

                if (spellbook.IsOnActionBarHash(item.UnlockSkill))
                {
                    spellbook.RemoveFromActionBar(item.UnlockSkill);
                }

                spellbook.Remove(item.UnlockSkill);
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
                    this.behaviours.Apply(behaviour, gameObject);
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
                    this.behaviours.RemoveAllStacks(behaviour.Id);
                }
            }
        }
    }
}