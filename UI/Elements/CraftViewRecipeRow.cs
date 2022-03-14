using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CraftViewRecipeRow : PoolableMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Payload<CraftViewRecipeRow> Clicked;

        [SerializeField] private Image icon;
        [SerializeField] private Image fade;
        [SerializeField] private Image outline;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemCountText;

        public Recipe Recipe { get; private set; }
        private InventoryComponent inventory;

        public void Construct(Recipe recipe, InventoryComponent inventory)
        {
            Recipe = recipe;

            this.inventory = inventory;
            this.itemNameText.text = recipe.Item.ColoredName;
            this.icon.sprite = Resources.Load<Sprite>(recipe.Item.Icon);

            Refresh(Recipe);
            Deselect();
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void Refresh(Recipe recipe)
        {
            var craftableItemsCount = this.inventory.GetCraftableItemsCount(recipe);
            this.itemCountText.text = craftableItemsCount > 0 ? $"({craftableItemsCount})" : "";

            this.fade.color = this.fade.color.With(a: this.inventory.HasEnoughIngredients(Recipe) ? 0 : 0.5f);
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