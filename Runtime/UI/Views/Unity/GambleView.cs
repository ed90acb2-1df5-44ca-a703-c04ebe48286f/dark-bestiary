using System;
using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class GambleView : View, IGambleView
    {
        public event Action Gamble;
        public event Action<Item> Buy;
        public event Action<Item> Sell;

        [SerializeField] private ItemListRow m_ItemPrefab;
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_GambleButton;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private Interactable m_SellDropArea;
        [SerializeField] private Image m_PriceIcon;

        private InventoryPanel m_InventoryPanel;
        private MonoBehaviourPool<ItemListRow> m_ItemPool;

        public void Construct(InventoryPanel inventoryPanel)
        {
            m_InventoryPanel = inventoryPanel;
            m_ItemPool = MonoBehaviourPool<ItemListRow>.Factory(m_ItemPrefab, m_ItemContainer);

            m_GambleButton.PointerClick += OnGambleButtonClicked;
            m_CloseButton.PointerClick += Hide;
            m_SellDropArea.Dropped += OnSellAreaDropped;
        }

        protected override void OnTerminate()
        {
            m_ItemPool.Clear();
            m_GambleButton.PointerClick -= OnGambleButtonClicked;
            m_CloseButton.PointerClick -= Hide;
            m_SellDropArea.Dropped -= OnSellAreaDropped;
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

        public void UpdatePrice(Currency price)
        {
            m_PriceIcon.sprite = Resources.Load<Sprite>(price.Icon);
            m_PriceText.text = price.Amount.ToString();
        }

        public void Display(List<Item> items)
        {
            foreach (var itemView in m_ItemPool.ActiveItems)
            {
                itemView.Clicked -= OnItemClicked;
                itemView.RightClicked -= OnItemRightClicked;
            }

            m_ItemPool.DespawnAll();

            foreach (var item in items)
            {
                var itemView = m_ItemPool.Spawn();
                itemView.Clicked += OnItemClicked;
                itemView.RightClicked += OnItemRightClicked;
                itemView.Construct(item);
            }
        }

        private void OnSellAreaDropped(GameObject dropped)
        {
            var inventoryItem = dropped.GetComponent<InventoryItem>();

            if (inventoryItem == null)
            {
                return;
            }

            TriggerSelling(inventoryItem.Item);
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            TriggerSelling(inventoryItem.Item);
        }

        private void OnGambleButtonClicked()
        {
            Gamble?.Invoke();
        }

        private void OnItemClicked(ItemListRow itemView)
        {
            Dialog.Instance.Clear()
                .AddText(I18N.Instance.Get("ui_buy") + $" {itemView.Item.ColoredName}?")
                .AddSpace()
                .AddOption(I18N.Instance.Get("ui_yes"), () => TriggerBuying(itemView.Item))
                .AddOption(I18N.Instance.Get("ui_no"))
                .Show();
        }

        private void OnItemRightClicked(ItemListRow itemView)
        {
            TriggerBuying(itemView.Item);
        }

        private void TriggerSelling(Item item)
        {
            Sell?.Invoke(item);
            AudioManager.Instance.PlayItemSell();
        }

        private void TriggerBuying(Item item)
        {
            Buy?.Invoke(item);
        }
    }
}