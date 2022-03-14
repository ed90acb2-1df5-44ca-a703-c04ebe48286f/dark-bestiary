using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class GambleView : View, IGambleView
    {
        public event Payload Gamble;
        public event Payload<Item> Buy;
        public event Payload<Item> Sell;

        [SerializeField] private ItemListRow itemPrefab;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable gambleButton;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Interactable sellDropArea;
        [SerializeField] private Image priceIcon;

        private InventoryPanel inventoryPanel;
        private MonoBehaviourPool<ItemListRow> itemPool;

        public void Construct(InventoryPanel inventoryPanel)
        {
            this.inventoryPanel = inventoryPanel;
            this.itemPool = MonoBehaviourPool<ItemListRow>.Factory(this.itemPrefab, this.itemContainer);

            this.gambleButton.PointerClick += OnGambleButtonClicked;
            this.closeButton.PointerClick += Hide;
            this.sellDropArea.Dropped += OnSellAreaDropped;
        }

        protected override void OnTerminate()
        {
            this.itemPool.Clear();
            this.gambleButton.PointerClick -= OnGambleButtonClicked;
            this.closeButton.PointerClick -= Hide;
            this.sellDropArea.Dropped -= OnSellAreaDropped;
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

        public void UpdatePrice(Currency price)
        {
            this.priceIcon.sprite = Resources.Load<Sprite>(price.Icon);
            this.priceText.text = price.Amount.ToString();
        }

        public void Display(List<Item> items)
        {
            foreach (var itemView in this.itemPool.ActiveItems)
            {
                itemView.Clicked -= OnItemClicked;
                itemView.RightClicked -= OnItemRightClicked;
            }

            this.itemPool.DespawnAll();

            foreach (var item in items)
            {
                var itemView = this.itemPool.Spawn();
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