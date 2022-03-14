using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class DismantlingView : View, IDismantlingView
    {
        public event Payload OkayButtonClicked;
        public event Payload DismantleButtonClicked;
        public event Payload ClearButtonClicked;
        public event Payload<RarityType> PlaceItems;
        public event Payload<Item> ItemPlacing;
        public event Payload<Item> ItemRemoving;

        [SerializeField] private Button okayButton;
        [SerializeField] private Button dismantleButton;
        [SerializeField] private Button clearButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button dismantleMagicButton;
        [SerializeField] private Button dismantleRareButton;
        [SerializeField] private Button dismantleUniqueButton;
        [SerializeField] private ItemListRow itemRowPrefab;
        [SerializeField] private Transform itemRowContainer;
        [SerializeField] private ItemDropArea itemDropArea;

        private MonoBehaviourPool<ItemListRow> itemRowPool;
        private InventoryPanel inventoryPanel;

        public void Construct(InventoryPanel inventoryPanel)
        {
            this.inventoryPanel = inventoryPanel;
            this.itemRowPool = MonoBehaviourPool<ItemListRow>.Factory(this.itemRowPrefab, this.itemRowContainer);

            this.itemDropArea.ItemDroppedIn += OnItemDroppedIn;
            this.okayButton.onClick.AddListener(() => OkayButtonClicked?.Invoke());
            this.dismantleButton.onClick.AddListener(() => DismantleButtonClicked?.Invoke());
            this.clearButton.onClick.AddListener(() => ClearButtonClicked?.Invoke());
            this.closeButton.onClick.AddListener(Hide);

            this.dismantleMagicButton.onClick.AddListener(OnDismantleMagicButtonClicked);
            this.dismantleRareButton.onClick.AddListener(OnDismantleRareButtonClicked);
            this.dismantleUniqueButton.onClick.AddListener(OnDismantleUniqueButtonClicked);
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

        private void OnDismantleMagicButtonClicked()
        {
            PlaceItems?.Invoke(RarityType.Magic);
        }

        private void OnDismantleRareButtonClicked()
        {
            PlaceItems?.Invoke(RarityType.Rare);
        }

        private void OnDismantleUniqueButtonClicked()
        {
            PlaceItems?.Invoke(RarityType.Unique);
        }

        protected override void OnTerminate()
        {
            this.itemRowPool.Clear();
        }

        protected override void OnHidden()
        {
            ClearButtonClicked?.Invoke();
        }

        public void DisplayDismantlingItems(IEnumerable<Item> items)
        {
            this.okayButton.gameObject.SetActive(false);
            this.dismantleButton.gameObject.SetActive(true);
            this.clearButton.gameObject.SetActive(true);

            ClearItems();
            CreateDismantlingItems(items);
        }

        public void DisplayDismantlingResult(IEnumerable<Item> items)
        {
            this.okayButton.gameObject.SetActive(true);
            this.dismantleButton.gameObject.SetActive(false);
            this.clearButton.gameObject.SetActive(false);

            ClearItems();
            CreateDismantledItems(items);
        }

        private void CreateDismantledItems(IEnumerable<Item> items)
        {
            foreach (var group in items.GroupBy(item => item.Id))
            {
                var row = this.itemRowPool.Spawn();
                row.Construct(group.First());
                row.OverwriteStackCount(group.Sum(item => item.StackCount));
                row.HidePrice();
            }
        }

        private void CreateDismantlingItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                var row = this.itemRowPool.Spawn();
                row.Clicked += OnRowClicked;
                row.Construct(item);
                row.HidePrice();

                this.inventoryPanel
                    .Slots
                    .FirstOrDefault(slot => slot.InventoryItem.Item.Equals(item))?
                    .InventoryItem
                    .Block();
            }
        }

        private void ClearItems()
        {
            foreach (var slot in this.inventoryPanel.Slots)
            {
                slot.InventoryItem.Unblock();
            }

            foreach (var row in this.itemRowPool.ActiveItems.ToList())
            {
                row.Clicked -= OnRowClicked;
                row.Despawn();
            }
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            ItemPlacing?.Invoke(inventoryItem.Item);
        }

        private void OnItemDroppedIn(Item item)
        {
            ItemPlacing?.Invoke(item);
        }

        private void OnRowClicked(ItemListRow row)
        {
            ItemRemoving?.Invoke(row.Item);
        }
    }
}