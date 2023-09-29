using System;
using DarkBestiary.Events;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RuneSocketView : MonoBehaviour
    {
        public event Action<RuneSocketView, ItemDroppedEventData> ItemDroppedIn;
        public event Action<RuneSocketView, ItemDroppedEventData> ItemDroppedOut;
        public event Action<RuneSocketView, InventoryItem> ItemControlOrRightClicked;

        public InventoryItemSlot Slot => m_ItemSlot;

        [SerializeField] private InventoryItemSlot m_ItemSlot;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private Image m_Border;
        [SerializeField] private Sprite m_MinorBorder;
        [SerializeField] private Sprite m_RegularBorder;
        [SerializeField] private Sprite m_MajorBorder;

        private ItemTypeType m_Type;

        public void Construct()
        {
            m_ItemSlot.ItemDroppedIn += OnItemDroppedIn;
            m_ItemSlot.ItemDroppedOut += OnItemDroppedOut;
            m_ItemSlot.InventoryItem.RightClicked += OnItemControlClicked;
            m_ItemSlot.InventoryItem.ControlClicked += OnItemControlClicked;
        }

        public void ChangeType(ItemTypeType type)
        {
            m_Type = type;

            switch (type)
            {
                case ItemTypeType.MinorRune:
                    m_Border.sprite = m_MinorBorder;
                    break;
                case ItemTypeType.Rune:
                    m_Border.sprite = m_RegularBorder;
                    break;
                case ItemTypeType.MajorRune:
                    m_Border.sprite = m_MajorBorder;
                    break;
            }
        }

        public void Change(Item item)
        {
            var characterEntity = Game.Instance.Character.Entity;

            m_ItemSlot.ChangeItem(item);

            m_NameText.text = !item.IsEmpty
                ? $"{item.ColoredName}\n<color=#fff><size=80%>{item.PassiveDescription.ToString(characterEntity)}"
                : "";

            if (!item.IsEmpty)
            {
                m_NameText.alignment = TextAlignmentOptions.TopLeft;
                return;
            }

            m_NameText.alignment = TextAlignmentOptions.Left;

            switch (m_Type)
            {
                case ItemTypeType.MinorRune:
                    m_NameText.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_minor_rune_slot")}";
                    break;
                case ItemTypeType.Rune:
                    m_NameText.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_rune_slot")}";
                    break;
                case ItemTypeType.MajorRune:
                    m_NameText.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_major_rune_slot")}";
                    break;
            }
        }

        private void OnItemDroppedIn(ItemDroppedEventData payload)
        {
            ItemDroppedIn?.Invoke(this, payload);
        }

        private void OnItemDroppedOut(ItemDroppedEventData payload)
        {
            ItemDroppedOut?.Invoke(this, payload);
        }

        private void OnItemControlClicked(InventoryItem item)
        {
            ItemControlOrRightClicked?.Invoke(this, item);
        }
    }
}