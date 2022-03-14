using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using ModestTree;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class RunesView : View, IRunesView
    {
        public event Payload<Item> ItemDroppedIn;
        public event Payload<Item> ItemDroppedOut;
        public event Payload<Item, int> RuneDroppedIn;
        public event Payload<int> RuneDroppedOut;
        public event Payload<int> RuneRemoved;

        [SerializeField]
        private InventoryItemSlot itemSlot;

        [SerializeField]
        private TextMeshProUGUI itemName;

        [SerializeField]
        private Interactable closeButton;

        [SerializeField]
        private Transform socketContainer;

        private RuneSocketView[] runeSocketViews;
        private InventoryPanel inventoryPanel;

        public void Construct(InventoryPanel inventoryPanel)
        {
            this.inventoryPanel = inventoryPanel;
        }

        public void ChangeItem(Item item)
        {
            this.itemSlot.ChangeItem(item);
            this.itemName.text = item.IsEmpty ? "" : $"{item.ColoredName}\n<color=#fff><size=70%>{item.Type.Name.ToString()}";

            foreach (var socketView in this.runeSocketViews)
            {
                socketView.gameObject.SetActive(false);
            }

            if (item.IsEmpty)
            {
                return;
            }

            for (var i = 0; i < item.Runes.Count; i++)
            {
                this.runeSocketViews[i].ChangeType(Item.DetermineRuneTypeByIndex(i, item));
                this.runeSocketViews[i].Change(item.Runes[i]);
                this.runeSocketViews[i].gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            if (this.inventoryPanel == null)
            {
                return;
            }

            this.inventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;
        }

        private void OnDisable()
        {
            if (this.inventoryPanel == null)
            {
                return;
            }

            this.inventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
        }

        protected override void OnInitialize()
        {
            this.closeButton.PointerClick += Hide;

            this.itemSlot.ItemDroppedIn += OnItemDroppedIn;
            this.itemSlot.InventoryItem.Clicked += OnItemClicked;
            this.itemSlot.InventoryItem.RightClicked += OnItemClicked;
            this.itemSlot.InventoryItem.IsDraggable = false;

            this.runeSocketViews = this.socketContainer.GetComponentsInChildren<RuneSocketView>();

            foreach (var runeSocketView in this.runeSocketViews)
            {
                runeSocketView.Construct();
                runeSocketView.ItemDroppedIn += OnRuneDroppedIn;
                runeSocketView.ItemDroppedOut += OnRuneDroppedOut;
                runeSocketView.ItemControlOrRightClicked += OnRuneControlOrRightClicked;
            }
        }

        protected override void OnHidden()
        {
            if (this.runeSocketViews == null)
            {
                return;
            }

            ChangeItem(Item.CreateEmpty());
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            if (!inventoryItem.Item.IsAnyRune)
            {
                ItemDroppedIn?.Invoke(inventoryItem.Item);
                return;
            }

            var index = 0;

            for (var i = 0; i < this.runeSocketViews.Length; i++)
            {
                if (!this.runeSocketViews[i].Slot.InventoryItem.Item.IsEmpty)
                {
                    continue;
                }

                index = i;
                break;
            }

            RuneDroppedIn?.Invoke(inventoryItem.Item, index);
        }

        private void OnRuneDroppedIn(RuneSocketView socket, ItemDroppedEventData payload)
        {
            RuneDroppedIn?.Invoke(payload.InventoryItem.Item, this.runeSocketViews.IndexOf(socket));
        }

        private void OnRuneControlOrRightClicked(RuneSocketView socket, InventoryItem inventoryItem)
        {
            RuneRemoved?.Invoke(this.runeSocketViews.IndexOf(socket));
        }

        private void OnRuneDroppedOut(RuneSocketView socket, ItemDroppedEventData payload)
        {
            if (this.runeSocketViews.Any(x => x.Slot == payload.InventorySlot))
            {
                return;
            }

            if (payload.InventorySlot == this.itemSlot)
            {
                return;
            }

            // Note: to drop only into InventoryPanel slots
            if (payload.InventorySlot.InventoryItem.Item.Equipment != null ||
                payload.InventorySlot.InventoryItem.Item.Inventory == null)
            {
                return;
            }

            // Note: wait for InventoryPanel to pick up dropped item.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                RuneDroppedOut?.Invoke(this.runeSocketViews.IndexOf(socket));
            });
        }

        private void OnItemDroppedIn(ItemDroppedEventData payload)
        {
            ItemDroppedIn?.Invoke(payload.InventoryItem.Item);
        }

        private void OnItemClicked(InventoryItem item)
        {
            ItemDroppedOut?.Invoke(item.Item);
        }
    }
}