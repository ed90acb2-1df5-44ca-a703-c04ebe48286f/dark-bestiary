using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class StashView : View, IStashView
    {
        public event Payload<Item, int> Deposit;
        public event Payload<Item, int> Withdraw;
        public event Payload<int> DepositIngredients;
        public event Payload<int> WithdrawIngredients;

        [SerializeField] private InventoryPanel stashInventoryPanel;
        [SerializeField] private Transform tabContainer;
        [SerializeField] private Tab tabPrefab;
        [SerializeField] private Interactable closeButton;

        private readonly List<Tab> tabs = new List<Tab>();

        private Tab selectedTab;
        private InventoryPanel inventoryPanel;
        private InventoryComponent inventoryComponent;
        private EquipmentComponent equipmentComponent;
        private InventoryComponent[] inventories;

        public void Construct(InventoryPanel inventoryPanel, Character character, InventoryComponent[] inventories)
        {
            this.inventoryPanel = inventoryPanel;
            this.inventories = inventories;

            this.inventoryComponent = character.Entity.GetComponent<InventoryComponent>();
            this.equipmentComponent = character.Entity.GetComponent<EquipmentComponent>();

            this.stashInventoryPanel.Initialize(this.inventories[0]);

            this.closeButton.PointerClick += Hide;
            this.stashInventoryPanel.ItemRightClicked += OnStashInventoryItemRightClicked;
            this.stashInventoryPanel.ItemControlClicked += OnStashInventoryItemControlClicked;

            for (var i = 0; i < Stash.TabCount; i++)
            {
                var label = StringUtils.ToRomanNumeral(i + 1);

                if (Stash.Instance.Inventories[i].IsIngredientsStash)
                {
                    label = I18N.Instance.Translate("item_category_ingredients");
                }

                var tab = Instantiate(this.tabPrefab, this.tabContainer);
                tab.Construct(label);
                tab.Clicked += OnTabClicked;
                this.tabs.Add(tab);
            }

            OnTabClicked(this.tabs[0]);
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

        protected override void OnTerminate()
        {
            this.stashInventoryPanel.Terminate();

            this.closeButton.PointerClick -= Hide;
            this.stashInventoryPanel.ItemRightClicked -= OnStashInventoryItemRightClicked;
            this.stashInventoryPanel.ItemControlClicked -= OnStashInventoryItemControlClicked;
        }

        private void OnTabClicked(Tab tab)
        {
            this.selectedTab?.Deselect();
            this.selectedTab = tab;
            this.selectedTab.Select();

            this.stashInventoryPanel.ChangeInventory(this.inventories[this.tabs.IndexOf(tab)]);
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            Deposit?.Invoke(inventoryItem.Item, this.tabs.IndexOf(this.selectedTab));
        }

        private void OnStashInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            Withdraw?.Invoke(inventoryItem.Item, this.tabs.IndexOf(this.selectedTab));
        }

        private void OnStashInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            if (this.inventoryComponent.MaybeUse(inventoryItem.Item))
            {
                return;
            }

            AudioManager.Instance.PlayItemPlace(inventoryItem.Item);
            this.equipmentComponent.Equip(inventoryItem.Item);
        }
    }
}