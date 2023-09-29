using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Items.Transmutation.Recipes;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class TransmutationView : View, ITransmutationView
    {
        public event Action<Item> AddItem;
        public event Action<Item, int> AddItemIndex;
        public event Action<Item, Item> SwapItems;
        public event Action<Item> RemoveItem;
        public event Action Combine;

        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_CombineButton;
        [SerializeField] private InventoryItemSlot m_SlotPrefab;
        [SerializeField] private Transform m_SlotContainer;
        [SerializeField] private Interactable m_OpenRecipesButton;
        [SerializeField] private Interactable m_CloseRecipesButton;
        [SerializeField] private GameObject m_RecipesPanel;
        [SerializeField] private AlchemyRecipe m_AlchemyRecipePrefab;
        [SerializeField] private Transform m_AlchemyRecipeContainer;
        [SerializeField] private Sprite m_CrossIcon;
        [SerializeField] private Sprite m_UnknownIcon;
        [SerializeField] private Image m_RecipeIcon;
        [SerializeField] private TextMeshProUGUI m_RecipeText;
        [SerializeField] private GameObject m_ParticlesPrefab;

        private readonly List<InventoryItemSlot> m_Slots = new();

        private InventoryPanel m_InventoryPanel;

        public void Construct(InventoryPanel inventoryPanel, List<Item> items, List<ITransmutationRecipe> recipes)
        {
            m_InventoryPanel = inventoryPanel;

            foreach (var item in items)
            {
                var slot = Instantiate(m_SlotPrefab, m_SlotContainer);
                slot.ChangeItem(item);
                slot.ItemDroppedIn += OnItemDroppedIn;
                slot.ItemDroppedOut += OnItemDroppedOut;
                slot.InventoryItem.RightClicked += OnSlotItemRightClicked;
                m_Slots.Add(slot);
            }

            foreach (var recipe in recipes)
            {
                var recipeView = Instantiate(m_AlchemyRecipePrefab, m_AlchemyRecipeContainer);
                recipeView.Construct(recipe);
            }

            m_OpenRecipesButton.PointerClick += OnOpenRecipesButtonPointerClick;
            m_CloseRecipesButton.PointerClick += OnCloseRecipesButtonPointerClick;
            m_CombineButton.PointerClick += OnCombineButtonPointerClick;
            m_CloseButton.PointerClick += Hide;

            ClearResult();
        }

        protected override void OnTerminate()
        {
            m_OpenRecipesButton.PointerClick -= OnOpenRecipesButtonPointerClick;
            m_CloseRecipesButton.PointerClick -= OnCloseRecipesButtonPointerClick;
            m_CombineButton.PointerClick -= OnCombineButtonPointerClick;
            m_CloseButton.PointerClick -= Hide;
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

        private void OnOpenRecipesButtonPointerClick()
        {
            m_RecipesPanel.gameObject.SetActive(true);
        }

        private void OnCloseRecipesButtonPointerClick()
        {
            m_RecipesPanel.gameObject.SetActive(false);
        }

        public void OnSuccess()
        {
            AudioManager.Instance.PlayAlchemyCombine();
            Instantiate(m_ParticlesPrefab, m_SlotContainer.parent).DestroyAsVisualEffect();
        }

        public void Block(Item item)
        {
            m_InventoryPanel.Block(item);
        }

        public void Unblock(Item item)
        {
            m_InventoryPanel.Unblock(item);
        }

        public void ShowResult(Recipe recipe)
        {
            m_RecipeIcon.color = m_RecipeIcon.color.With(a: 1);
            m_RecipeIcon.sprite = Resources.Load<Sprite>(recipe.Item.Icon);
            m_RecipeText.text = $"<size=125%><smallcaps>{recipe.Item.Name}</smallcaps></size>\n{recipe.Item.ConsumeDescription}";
        }

        public void ShowResult(string name, string description)
        {
            m_RecipeIcon.color = m_RecipeIcon.color.With(a: 1);
            m_RecipeIcon.sprite = m_UnknownIcon;
            m_RecipeText.text = $"<size=125%><smallcaps>{name}</smallcaps></size>\n{description}";
        }

        public void ShowImpossibleCombination()
        {
            m_RecipeIcon.color = m_RecipeIcon.color.With(a: 1);
            m_RecipeIcon.sprite = m_CrossIcon;
            m_RecipeText.text = I18N.Instance.Get("ui_impossible_combination");
        }

        public void ClearResult()
        {
            m_RecipeIcon.color = m_RecipeIcon.color.With(a: 0);
            m_RecipeText.text = "";
        }

        public void RefreshItems(List<Item> items)
        {
            var index = 0;

            foreach (var item in items)
            {
                m_Slots[index].ChangeItem(item);
                index++;
            }
        }

        private void OnCombineButtonPointerClick()
        {
            Combine?.Invoke();
        }

        private void OnSlotItemRightClicked(InventoryItem inventoryItem)
        {
            RemoveItem?.Invoke(inventoryItem.Item);
        }

        private void OnInventoryItemControlClicked(InventoryItem inventoryItem)
        {
            AddItem?.Invoke(inventoryItem.Item);
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            var containing = m_Slots.FirstOrDefault(s => s.InventoryItem.Item == data.InventoryItem.Item);

            if (containing == null)
            {
                AddItemIndex?.Invoke(data.InventoryItem.Item, m_Slots.IndexOf(data.InventorySlot));
                return;
            }

            SwapItems?.Invoke(containing.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
        }

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            if (m_Slots.Any(s => s == data.InventorySlot))
            {
                return;
            }

            // Note: to prevent Empty item dropping into inventory slot
            Timer.Instance.WaitForFixedUpdate(() => RemoveItem?.Invoke(data.InventoryItem.Item));
        }
    }
}