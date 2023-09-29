using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class DismantlingView : View, IDismantlingView
    {
        public event Action OkayButtonClicked;
        public event Action DismantleButtonClicked;
        public event Action ClearButtonClicked;
        public event Action<RarityType> PlaceItems;
        public event Action<Item> ItemPlacing;
        public event Action<Item> ItemRemoving;

        [SerializeField] private Button m_OkayButton;
        [SerializeField] private Button m_DismantleButton;
        [SerializeField] private Button m_ClearButton;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private Button m_DismantleMagicButton;
        [SerializeField] private Button m_DismantleRareButton;
        [SerializeField] private Button m_DismantleUniqueButton;
        [SerializeField] private ItemListRow m_ItemRowPrefab;
        [SerializeField] private Transform m_ItemRowContainer;
        [SerializeField] private ItemDropArea m_ItemDropArea;

        private MonoBehaviourPool<ItemListRow> m_ItemRowPool;
        private InventoryPanel m_InventoryPanel;

        public void Construct(InventoryPanel inventoryPanel)
        {
            m_InventoryPanel = inventoryPanel;
            m_ItemRowPool = MonoBehaviourPool<ItemListRow>.Factory(m_ItemRowPrefab, m_ItemRowContainer);

            m_ItemDropArea.ItemDroppedIn += OnItemDroppedIn;
            m_OkayButton.onClick.AddListener(() => OkayButtonClicked?.Invoke());
            m_DismantleButton.onClick.AddListener(() => DismantleButtonClicked?.Invoke());
            m_ClearButton.onClick.AddListener(() => ClearButtonClicked?.Invoke());
            m_CloseButton.onClick.AddListener(Hide);

            m_DismantleMagicButton.onClick.AddListener(OnDismantleMagicButtonClicked);
            m_DismantleRareButton.onClick.AddListener(OnDismantleRareButtonClicked);
            m_DismantleUniqueButton.onClick.AddListener(OnDismantleUniqueButtonClicked);
        }

        private void OnEnable()
        {
            if (m_InventoryPanel == null)
            {
                return;
            }

            m_InventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;
        }

        private void OnDisable()
        {
            if (m_InventoryPanel == null)
            {
                return;
            }

            m_InventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
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
            m_ItemRowPool.Clear();
        }

        protected override void OnHidden()
        {
            ClearButtonClicked?.Invoke();
        }

        public void DisplayDismantlingItems(IEnumerable<Item> items)
        {
            m_OkayButton.gameObject.SetActive(false);
            m_DismantleButton.gameObject.SetActive(true);
            m_ClearButton.gameObject.SetActive(true);

            ClearItems();
            CreateDismantlingItems(items);
        }

        public void DisplayDismantlingResult(IEnumerable<Item> items)
        {
            m_OkayButton.gameObject.SetActive(true);
            m_DismantleButton.gameObject.SetActive(false);
            m_ClearButton.gameObject.SetActive(false);

            ClearItems();
            CreateDismantledItems(items);
        }

        private void CreateDismantledItems(IEnumerable<Item> items)
        {
            foreach (var group in items.GroupBy(item => item.Id))
            {
                var row = m_ItemRowPool.Spawn();
                row.Construct(group.First());
                row.OverwriteStackCount(group.Sum(item => item.StackCount));
                row.HidePrice();
            }
        }

        private void CreateDismantlingItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                var row = m_ItemRowPool.Spawn();
                row.Clicked += OnRowClicked;
                row.Construct(item);
                row.HidePrice();

                m_InventoryPanel
                    .Slots
                    .FirstOrDefault(slot => slot.InventoryItem.Item.Equals(item))?
                    .InventoryItem
                    .Block();
            }
        }

        private void ClearItems()
        {
            foreach (var slot in m_InventoryPanel.Slots)
            {
                slot.InventoryItem.Unblock();
            }

            foreach (var row in m_ItemRowPool.ActiveItems.ToList())
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