using System;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CraftViewRecipeRow : PoolableMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<CraftViewRecipeRow> Clicked;

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Fade;
        [SerializeField] private Image m_Outline;
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private TextMeshProUGUI m_ItemCountText;

        public Recipe Recipe { get; private set; }
        private InventoryComponent m_Inventory;

        public void Construct(Recipe recipe, InventoryComponent inventory)
        {
            Recipe = recipe;

            m_Inventory = inventory;
            m_ItemNameText.text = recipe.Item.ColoredName;
            m_Icon.sprite = Resources.Load<Sprite>(recipe.Item.Icon);

            Refresh(Recipe);
            Deselect();
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void Refresh(Recipe recipe)
        {
            var craftableItemsCount = m_Inventory.GetCraftableItemsCount(recipe);
            m_ItemCountText.text = craftableItemsCount > 0 ? $"({craftableItemsCount})" : "";

            m_Fade.color = m_Fade.color.With(a: m_Inventory.HasEnoughIngredients(Recipe) ? 0 : 0.5f);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            ItemTooltip.Instance.Show(Recipe.Item, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}