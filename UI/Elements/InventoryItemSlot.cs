using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class InventoryItemSlot : MonoBehaviour, IDropHandler, IDragHandler, IPointerUpHandler
    {
        public event Payload<ItemDroppedEventData> ItemDroppedIn;
        public event Payload<ItemDroppedEventData> ItemDroppedOut;

        private static InventoryItemSlot hovered;

        [SerializeField] private InventoryItem inventoryItemPrefab;
        [SerializeField] private Button button;
        [SerializeField] private Transform container;
        [SerializeField] private Image border;
        [SerializeField] private Image highlight;

        public InventoryItem InventoryItem { get; private set; }

        public void Construct(Item item)
        {
            if (this.container == null)
            {
                this.container = transform;
            }

            InventoryItem = Instantiate(this.inventoryItemPrefab, this.container);

            ChangeItem(item);

            InventoryItem.Blocked += OnItemBlocked;
            InventoryItem.Unblocked += OnItemUnblocked;
            InventoryItem.Dropped += OnItemDropped;
            InventoryItem.BeginDrag += OnItemBeginDrag;
            InventoryItem.EndDrag += OnItemEndDrag;
        }

        public void ChangeItem(Item item)
        {
            InventoryItem.Change(item);
            UpdateBorderColor(item);

            name = $"Slot - {InventoryItem.Item.Name}  #{InventoryItem.Item.GetHashCode()}";
        }

        public void Highlight()
        {
            this.highlight.color = this.highlight.color.With(a: 1);
        }

        public void Unhighlight()
        {
            this.highlight.color = this.highlight.color.With(a: 0);
        }

        private void UpdateBorderColor(Item item)
        {
            if (this.border == null)
            {
                return;
            }

            if (item.IsEmpty)
            {
                this.border.color = this.border.color.With(a: 0);
                return;
            }

            this.border.color = item.Rarity.Color();
        }

        private void ResetBorderColor()
        {
            if (this.border == null)
            {
                return;
            }

            this.border.color = this.border.color.With(a: 0);
        }

        private void OnItemUnblocked(InventoryItem inventoryItem)
        {
            UpdateBorderColor(inventoryItem.Item);
        }

        private void OnItemBlocked(InventoryItem inventoryItem)
        {
            ResetBorderColor();
        }

        public void OnDrag(PointerEventData pointer)
        {
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            // TODO: Slot blocks item UI click events
            InventoryItem.OnPointerUp(pointer);
        }

        public void OnDrop(PointerEventData pointer)
        {
            var dragging = pointer.pointerDrag.GetComponent<InventoryItem>();

            if (dragging == null || InventoryItem.Item.Equals(dragging.Item))
            {
                return;
            }

            if (InventoryItem.IsBlocked)
            {
                return;
            }

            var data = new ItemDroppedEventData(dragging, this);
            dragging.OnDrop(data);
            ItemDroppedIn?.Invoke(data);
        }

        private void OnItemBeginDrag(InventoryItem inventoryItem)
        {
            this.button.OnPointerExit(new PointerEventData(EventSystem.current));
            ResetBorderColor();
        }

        private void OnItemEndDrag(InventoryItem inventoryItem)
        {
            UpdateBorderColor(inventoryItem.Item);
        }

        private void OnItemDropped(ItemDroppedEventData data)
        {
            if (data.InventorySlot.Equals(this))
            {
                return;
            }

            ItemDroppedOut?.Invoke(data);
        }
    }
}