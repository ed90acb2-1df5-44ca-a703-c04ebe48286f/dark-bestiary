using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Events;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class ItemUpgradeView : View, IItemUpgradeView, IHideOnEscape
    {
        public event Action<Item> ItemPlaced;
        public event Action<Item> ItemRemoved;
        public event Action UpgradeButtonClicked;

        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_ButtonText;
        [SerializeField] private Image m_ButtonIcon;
        [SerializeField] private TextMeshProUGUI m_ItemName;
        [SerializeField] private InventoryItemSlot m_ItemSlot;
        [SerializeField] private CraftViewIngredient m_IngredientPrefab;
        [SerializeField] private Transform m_IngredientContainer;
        [SerializeField] private Interactable m_UpgradeButton;
        [SerializeField] private Interactable m_CloseButton;

        private readonly List<CraftViewIngredient> m_IngredientViews = new();

        private bool m_RequiresUpdate;
        private Item m_Item;
        private InventoryPanel m_InventoryPanel;
        private InventoryComponent m_CharacterInventory;
        private InventoryComponent m_IngredientInventory;
        private List<RecipeIngredient> m_Ingredients;

        public void Construct(InventoryPanel inventoryPanel, InventoryComponent characterInventory, InventoryComponent ingredientInventory)
        {
            m_InventoryPanel = inventoryPanel;
            m_CharacterInventory = characterInventory;
            m_IngredientInventory = ingredientInventory;

            m_ItemSlot.ChangeItem(m_CharacterInventory.CreateEmptyItem());

            m_InventoryPanel.ItemControlClicked += OnInventoryItemControlClicked;

            m_ItemSlot.ItemDroppedIn += OnItemDroppedIn;
            m_ItemSlot.ItemDroppedOut += OnItemDroppedOut;
            m_ItemSlot.InventoryItem.RightClicked += OnItemRightClicked;

            m_UpgradeButton.PointerClick += OnUpgradeButtonClicked;
            m_CloseButton.PointerClick += Hide;

            m_ButtonIcon.gameObject.SetActive(false);
            m_ButtonText.text = I18N.Instance.Translate("ui_apply");
        }

        protected override void OnTerminate()
        {
            m_InventoryPanel.ItemControlClicked -= OnInventoryItemControlClicked;
        }

        protected override void OnHidden()
        {
            OnItemRightClicked(m_ItemSlot.InventoryItem);
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

        public void ChangeTitle(string title)
        {
            m_Title.text = title;
        }

        public void Refresh(Item item, List<RecipeIngredient> ingredients)
        {
            m_Item = item;
            m_Ingredients = ingredients;
            m_RequiresUpdate = true;
        }

        public void RefreshCost(Currency cost)
        {
            if (cost == null)
            {
                m_ButtonIcon.gameObject.SetActive(false);
                m_ButtonText.text = I18N.Instance.Translate("ui_apply");
                m_ButtonText.margin = Vector4.zero;
            }
            else
            {
                m_ButtonIcon.gameObject.SetActive(true);
                m_ButtonIcon.sprite = Resources.Load<Sprite>(cost.Icon);
                m_ButtonText.text = cost.Amount.ToString();
                m_ButtonText.margin = new Vector4(42, 0, 0, 0);
            }
        }

        public void Cleanup()
        {
            m_ItemName.text = "";
            m_ItemSlot.InventoryItem.Change(m_CharacterInventory.CreateEmptyItem());

            foreach (var element in m_IngredientContainer.GetComponentsInChildren<InventoryItem>())
            {
                Destroy(element.gameObject);
            }
        }

        private void CreateIngredients(List<RecipeIngredient> ingredients)
        {
            foreach (var ingredientView in m_IngredientViews)
            {
                Destroy(ingredientView.gameObject);
            }

            m_IngredientViews.Clear();

            foreach (var ingredient in ingredients)
            {
                var ingredientView = Instantiate(m_IngredientPrefab, m_IngredientContainer);
                ingredientView.Construct(ingredient, m_IngredientInventory);
                m_IngredientViews.Add(ingredientView);
            }
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            ItemPlaced?.Invoke(inventoryItem.Item);
        }

        private void OnItemRightClicked(InventoryItem item)
        {
            if (item.Item.IsEmpty)
            {
                return;
            }

            ItemRemoved?.Invoke(item.Item);
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            ItemPlaced?.Invoke(data.InventoryItem.Item);
        }

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            ItemRemoved?.Invoke(data.InventoryItem.Item);
        }

        private void OnUpgradeButtonClicked()
        {
            UpgradeButtonClicked?.Invoke();
        }

        private void Update()
        {
            if (!m_RequiresUpdate)
            {
                return;
            }

            m_RequiresUpdate = false;

            Cleanup();

            m_ItemName.text = m_Item.IsEmpty ? "" : m_Item.ColoredName;
            m_ItemSlot.InventoryItem.Change(m_Item);

            CreateIngredients(m_Ingredients);
        }
    }
}