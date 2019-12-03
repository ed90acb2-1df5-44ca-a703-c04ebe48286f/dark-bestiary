using System.Collections.Generic;
using DarkBestiary.Components;
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

        [SerializeField] private InventoryPanel inventoryPanel;
        [SerializeField] private EquipmentPanel equipmentPanel;
        [SerializeField] private CharacterPanel characterPanel;
        [SerializeField] private ItemListRow itemPrefab;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable gambleButton;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image priceIcon;

        private MonoBehaviourPool<ItemListRow> itemPool;

        private Character character;
        private InventoryComponent inventory;
        private EquipmentComponent equipment;

        public void Construct(Character character, Currency price)
        {
            this.character = character;
            this.inventory = character.Entity.GetComponent<InventoryComponent>();
            this.equipment = character.Entity.GetComponent<EquipmentComponent>();

            this.priceIcon.sprite = Resources.Load<Sprite>(price.Icon);
            this.priceText.text = price.Amount.ToString();
        }

        protected override void OnInitialize()
        {
            this.itemPool = MonoBehaviourPool<ItemListRow>.Factory(this.itemPrefab, this.itemContainer);

            this.characterPanel.Initialize(this.character);
            this.equipmentPanel.Initialize(this.equipment);
            this.inventoryPanel.Initialize(this.inventory);
            this.inventoryPanel.ItemRightClicked += OnInventoryItemRightClicked;

            this.gambleButton.PointerUp += OnGambleButtonClicked;
            this.closeButton.PointerUp += Hide;
        }

        protected override void OnTerminate()
        {
            this.characterPanel.Terminate();
            this.equipmentPanel.Terminate();
            this.inventoryPanel.Terminate();

            this.itemPool.Clear();
            this.gambleButton.PointerUp -= OnGambleButtonClicked;
            this.closeButton.PointerUp -= Hide;
        }

        public void Display(List<Item> items)
        {
            foreach (var itemView in this.itemPool.Items)
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

        private void OnInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            if (this.inventory.MaybeUse(inventoryItem.Item))
            {
                return;
            }

            AudioManager.Instance.PlayItemPlace(inventoryItem.Item);
            this.equipment.Equip(inventoryItem.Item);
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

        private void TriggerBuying(Item item)
        {
            Buy?.Invoke(item);
        }
    }
}