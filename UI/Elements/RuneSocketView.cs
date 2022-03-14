using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RuneSocketView : MonoBehaviour
    {
        public event Payload<RuneSocketView, ItemDroppedEventData> ItemDroppedIn;
        public event Payload<RuneSocketView, ItemDroppedEventData> ItemDroppedOut;
        public event Payload<RuneSocketView, InventoryItem> ItemControlOrRightClicked;

        public InventoryItemSlot Slot => this.itemSlot;

        [SerializeField] private InventoryItemSlot itemSlot;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image border;
        [SerializeField] private Sprite minorBorder;
        [SerializeField] private Sprite regularBorder;
        [SerializeField] private Sprite majorBorder;

        private ItemTypeType type;

        public void Construct()
        {
            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.ItemDroppedOut += OnItemDroppedOut;
            this.itemSlot.InventoryItem.RightClicked += OnItemControlClicked;
            this.itemSlot.InventoryItem.ControlClicked += OnItemControlClicked;
        }

        public void ChangeType(ItemTypeType type)
        {
            this.type = type;

            switch (type)
            {
                case ItemTypeType.MinorRune:
                    this.border.sprite = this.minorBorder;
                    break;
                case ItemTypeType.Rune:
                    this.border.sprite = this.regularBorder;
                    break;
                case ItemTypeType.MajorRune:
                    this.border.sprite = this.majorBorder;
                    break;
            }
        }

        public void Change(Item item)
        {
            this.itemSlot.ChangeItem(item);
            this.nameText.text = item.IsEmpty ? "" : $"{item.ColoredName}\n<color=#fff><size=80%>{item.PassiveDescription.ToString()}";

            if (!item.IsEmpty)
            {
                this.nameText.alignment = TextAlignmentOptions.TopLeft;
                return;
            }

            this.nameText.alignment = TextAlignmentOptions.Left;

            switch (this.type)
            {
                case ItemTypeType.MinorRune:
                    this.nameText.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_minor_rune_slot")}";
                    break;
                case ItemTypeType.Rune:
                    this.nameText.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_rune_slot")}";
                    break;
                case ItemTypeType.MajorRune:
                    this.nameText.text = $"<color=#888>{I18N.Instance.Translate("ui_empty_major_rune_slot")}";
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