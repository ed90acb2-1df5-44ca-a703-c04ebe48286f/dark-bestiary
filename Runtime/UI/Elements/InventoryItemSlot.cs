using System;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class InventoryItemSlot : PoolableMonoBehaviour, IDropHandler, IDragHandler, IPointerUpHandler
    {
        public event Action<ItemDroppedEventData> ItemDroppedIn;
        public event Action<ItemDroppedEventData> ItemDroppedOut;

        private static InventoryItemSlot s_Hovered;

        public InventoryItem InventoryItem => m_InventoryItem;

        [SerializeField] private InventoryItem m_InventoryItem;
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_Border;
        [SerializeField] private Image m_Highlight;

        private void Start()
        {
            InventoryItem.AnyBeginDrag += OnAnyItemBeginDrag;
            InventoryItem.AnyEndDrag += OnAnyItemEndDrag;

            InventoryItem.Blocked += OnItemBlocked;
            InventoryItem.Unblocked += OnItemUnblocked;
            InventoryItem.Dropped += OnItemDropped;
            InventoryItem.BeginDrag += OnItemBeginDrag;
            InventoryItem.EndDrag += OnItemEndDrag;
        }

        private void OnDestroy()
        {
            InventoryItem.AnyBeginDrag -= OnAnyItemBeginDrag;
            InventoryItem.AnyEndDrag -= OnAnyItemEndDrag;

            InventoryItem.Blocked -= OnItemBlocked;
            InventoryItem.Unblocked -= OnItemUnblocked;
            InventoryItem.Dropped -= OnItemDropped;
            InventoryItem.BeginDrag -= OnItemBeginDrag;
            InventoryItem.EndDrag -= OnItemEndDrag;
        }

        public void ChangeItem(Item item)
        {
            InventoryItem.Change(item);
            UpdateBorderColor(item);

            name = $"Slot - {InventoryItem.Item.Name}  #{InventoryItem.Item.GetHashCode()}";
        }

        public void Highlight()
        {
            m_Highlight.color = m_Highlight.color.With(a: 1);
        }

        public void Unhighlight()
        {
            m_Highlight.color = m_Highlight.color.With(a: 0);
        }

        private void UpdateBorderColor(Item item)
        {
            if (item.IsEmpty)
            {
                m_Border.color = m_Border.color.With(a: 0);
                return;
            }

            m_Border.color = item.Rarity.Color();
        }

        private void ResetBorderColor()
        {
            m_Border.color = m_Border.color.With(a: 0);
        }

        private void OnItemUnblocked(InventoryItem inventoryItem)
        {
            UpdateBorderColor(inventoryItem.Item);
        }

        private void OnAnyItemBeginDrag(InventoryItem item)
        {
            if (item.Item.IsGem && InventoryItem.Item.HasEmptySockets)
            {
                Highlight();
            }
        }

        private void OnAnyItemEndDrag(InventoryItem item)
        {
            Unhighlight();
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
            InventoryItem.OnPointerClick(pointer);
        }

        public void OnDrop(PointerEventData pointer)
        {
            var dragging = pointer.pointerDrag.GetComponent<InventoryItem>();

            if (dragging == null || InventoryItem.Item.Equals(dragging.Item) || dragging.Item.IsEmpty)
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
            m_Button.OnPointerExit(new PointerEventData(EventSystem.current));
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