using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class StashView : View, IStashView
    {
        public event Action<Item, int> Deposit;
        public event Action<Item, int> Withdraw;
        public event Action<int> DepositIngredients;
        public event Action<int> WithdrawIngredients;

        [SerializeField] private InventoryPanel m_StashInventoryPanel;
        [SerializeField] private Transform m_TabContainer;
        [SerializeField] private Tab m_TabPrefab;
        [SerializeField] private Interactable m_CloseButton;

        private readonly List<Tab> m_Tabs = new();

        private Tab m_SelectedTab;
        private InventoryPanel m_InventoryPanel;
        private InventoryComponent m_InventoryComponent;
        private EquipmentComponent m_EquipmentComponent;
        private InventoryComponent[] m_Inventories;

        public void Construct(InventoryPanel inventoryPanel, Character character, InventoryComponent[] inventories)
        {
            m_InventoryPanel = inventoryPanel;
            m_Inventories = inventories;

            m_InventoryComponent = character.Entity.GetComponent<InventoryComponent>();
            m_EquipmentComponent = character.Entity.GetComponent<EquipmentComponent>();

            m_StashInventoryPanel.Initialize(m_Inventories[0]);

            m_CloseButton.PointerClick += Hide;
            m_StashInventoryPanel.ItemRightClicked += OnStashInventoryItemRightClicked;
            m_StashInventoryPanel.ItemControlClicked += OnStashInventoryItemControlClicked;

            for (var i = 0; i < Stash.c_TabCount; i++)
            {
                var label = StringUtils.ToRomanNumeral(i + 1);

                if (Stash.Instance.Inventories[i].IsIngredientsStash)
                {
                    label = I18N.Instance.Translate("item_category_ingredients");
                }

                var tab = Instantiate(m_TabPrefab, m_TabContainer);
                tab.Construct(label);
                tab.Clicked += OnTabClicked;
                m_Tabs.Add(tab);
            }

            OnTabClicked(m_Tabs[0]);
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

        protected override void OnTerminate()
        {
            m_StashInventoryPanel.Terminate();

            m_CloseButton.PointerClick -= Hide;
            m_StashInventoryPanel.ItemRightClicked -= OnStashInventoryItemRightClicked;
            m_StashInventoryPanel.ItemControlClicked -= OnStashInventoryItemControlClicked;
        }

        private void OnTabClicked(Tab tab)
        {
            m_SelectedTab?.Deselect();
            m_SelectedTab = tab;
            m_SelectedTab.Select();

            m_StashInventoryPanel.ChangeInventory(m_Inventories[m_Tabs.IndexOf(tab)]);
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            Deposit?.Invoke(inventoryItem.Item, m_Tabs.IndexOf(m_SelectedTab));
        }

        private void OnStashInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            Withdraw?.Invoke(inventoryItem.Item, m_Tabs.IndexOf(m_SelectedTab));
        }

        private void OnStashInventoryItemRightClicked(InventoryItem inventoryItem)
        {
            if (m_InventoryComponent.MaybeUse(inventoryItem.Item))
            {
                return;
            }

            AudioManager.Instance.PlayItemPlace(inventoryItem.Item);
            m_EquipmentComponent.Equip(inventoryItem.Item);
        }
    }
}