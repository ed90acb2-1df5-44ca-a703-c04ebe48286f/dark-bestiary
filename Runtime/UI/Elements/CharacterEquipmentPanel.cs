using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CharacterEquipmentPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_CharacterNameText = null!;

        [SerializeField]
        private TextMeshProUGUI m_LevelText = null!;

        [SerializeField]
        private TextMeshProUGUI m_ExperienceText = null!;

        [SerializeField]
        private Image m_ExperienceFiller = null!;

        [SerializeField]
        private List<InventoryItemSlot> m_Slots = null!;

        [Header("Weapon swap")]
        [SerializeField]
        private Interactable m_SwapWeaponButton = null!;

        private ExperienceComponent m_Experience = null!;

        [SerializeField]
        private Toggle m_HelmCheckbox = null!;

        private EquipmentComponent m_Equipment = null!;
        private InventoryComponent m_Inventory = null!;

        private Item? m_EnchantSource;
        private Item? m_EnchantTarget;

        public void Initialize(Character character)
        {
            m_Inventory = character.Entity.GetComponent<InventoryComponent>();

            m_Equipment = character.Entity.GetComponent<EquipmentComponent>();
            m_Equipment.ItemEquipped += OnItemEquipped;
            m_Equipment.ItemUnequipped += OnItemUnequipped;
            m_Equipment.ItemsSwapped += OnItemsSwapped;

            for (var i = 0; i < m_Equipment.Slots.Count; i++)
            {
                m_Slots[i].ChangeItem(m_Equipment.Slots[i].Item);
                m_Slots[i].ItemDroppedIn += OnItemDroppedIn;
                m_Slots[i].ItemDroppedOut += OnItemDroppedOut;
                m_Slots[i].InventoryItem.RightClicked += OnItemRightClicked;
            }

            m_SwapWeaponButton.PointerClick += OnSwapWeaponButtonPointerClick;

            m_Experience = character.Entity.GetComponent<ExperienceComponent>();
            m_Experience.Experience.Changed += OnExperienceChanged;
            OnExperienceChanged(m_Experience.Experience);

            var actor = character.Entity.GetComponent<ActorComponent>();

            m_HelmCheckbox.isOn = actor.IsHelmVisible;
            m_HelmCheckbox.onValueChanged.AddListener(b => actor.SetHelmVisible(b));

            m_CharacterNameText.text = character.Name;

            InventoryItem.AnyBeginDrag += OnAnyItemBeginDrag;
            InventoryItem.AnyEndDrag += OnAnyItemEndDrag;
        }

        public void Terminate()
        {
            m_Equipment.ItemEquipped -= OnItemEquipped;
            m_Equipment.ItemUnequipped -= OnItemUnequipped;
            m_Equipment.ItemsSwapped -= OnItemsSwapped;

            m_SwapWeaponButton.PointerClick -= OnSwapWeaponButtonPointerClick;

            foreach (var slot in m_Slots)
            {
                slot.ItemDroppedIn -= OnItemDroppedIn;
                slot.ItemDroppedOut -= OnItemDroppedOut;
                slot.InventoryItem.RightClicked -= OnItemRightClicked;
            }

            m_Experience.Experience.Changed -= OnExperienceChanged;

            InventoryItem.AnyBeginDrag -= OnAnyItemBeginDrag;
            InventoryItem.AnyEndDrag -= OnAnyItemEndDrag;
        }

        public void Refresh()
        {
            for (var i = 0; i < m_Equipment.Slots.Count; i++)
            {
                m_Slots[i].ChangeItem(m_Equipment.Slots[i].Item);
            }
        }

        private void OnExperienceChanged(Experience experience)
        {
            m_LevelText.text = $"{I18N.Instance.Get("ui_level")} {experience.Level}";
            m_ExperienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
            m_ExperienceFiller.fillAmount = experience.GetObtainedFraction();
        }

        private void OnSwapWeaponButtonPointerClick()
        {
            try
            {
                m_Equipment.SwapWeapon();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnAnyItemBeginDrag(InventoryItem item)
        {
            var suitable = m_Equipment.FindSuitableSlot(item.Item);

            if (suitable == null)
            {
                return;
            }

            m_Slots[m_Equipment.Slots.IndexOf(suitable)].Highlight();
        }

        private void OnAnyItemEndDrag(InventoryItem item)
        {
            foreach (var slot in m_Slots)
            {
                slot.Unhighlight();
            }
        }

        private void OnItemsSwapped()
        {
            Refresh();
        }

        private void OnItemUnequipped(Item item)
        {
            Refresh();
        }

        private void OnItemEquipped(Item item)
        {
            Refresh();
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            try
            {
                if (m_Equipment.IsEquipped(data.InventoryItem.Item))
                {
                    m_Equipment.Swap(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
                    return;
                }

                if (data.InventoryItem.Item.IsEnchantment &&
                    data.InventorySlot.InventoryItem.Item.IsEnchantable)
                {
                    m_EnchantSource = data.InventoryItem.Item;
                    m_EnchantTarget = data.InventorySlot.InventoryItem.Item;

                    ConfirmationWindow.Instance.Cancelled += OnEnchantCancelled;
                    ConfirmationWindow.Instance.Confirmed += OnEnchantConfirmed;
                    ConfirmationWindow.Instance.Show(
                        I18N.Instance.Get("ui_confirm_enchant_x").ToString(data.InventorySlot.InventoryItem.Item.ColoredName),
                        I18N.Instance.Get("ui_confirm")
                    );

                    return;
                }

                if (data.InventoryItem.Item.IsGem &&
                    data.InventorySlot.InventoryItem.Item.IsSocketable &&
                    data.InventorySlot.InventoryItem.Item.HasEmptySockets)
                {
                    data.InventorySlot.InventoryItem.Item.InsertSocket(data.InventoryItem.Item);
                    return;
                }

                m_Equipment.EquipIntoSlot(data.InventoryItem.Item, m_Equipment.Slots[m_Slots.IndexOf(data.InventorySlot)]);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnEnchantConfirmed()
        {
            try
            {
                m_EnchantTarget.Enchant(m_EnchantSource);
                (m_EnchantSource.Inventory ? m_EnchantSource.Inventory : m_Inventory)
                    .Remove(m_EnchantSource, 1);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
            finally
            {
                OnEnchantCancelled();
            }
        }

        private void OnEnchantCancelled()
        {
            ConfirmationWindow.Instance.Cancelled -= OnEnchantCancelled;
            ConfirmationWindow.Instance.Confirmed -= OnEnchantConfirmed;

            m_EnchantSource = null;
            m_EnchantTarget = null;
        }

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            if (m_Slots.Any(slot => slot.Equals(data.InventorySlot)))
            {
                return;
            }

            Timer.Instance.WaitForEndOfFrame(() =>
            {
                try
                {
                    m_Equipment.Unequip(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
                }
                catch (GameplayException exception)
                {
                    UiErrorFrame.Instance.ShowMessage(exception.Message);
                }
            });
        }

        private void OnItemRightClicked(InventoryItem inventoryItem)
        {
            try
            {
                m_Equipment.Unequip(inventoryItem.Item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}