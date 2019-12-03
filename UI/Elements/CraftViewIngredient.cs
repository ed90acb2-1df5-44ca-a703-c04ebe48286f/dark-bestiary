using DarkBestiary.Components;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class CraftViewIngredient : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private InventoryItem item;
        [SerializeField] private Image fadeImage;
        [SerializeField] private TextMeshProUGUI itemCountText;

        public RecipeIngredient Ingredient { get; private set; }

        private RectTransform rectTransform;
        private InventoryComponent inventory;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
        }

        public void Construct(RecipeIngredient ingredient, InventoryComponent inventory)
        {
            Ingredient = ingredient;

            this.inventory = inventory;
            this.fadeImage.color = new Color(0, 0, 0, 0);

            this.item.IsDraggable = false;
            this.item.Change(ingredient.Item);
            this.item.OverwriteStackCount(0);

            Refresh();
        }

        public void Refresh()
        {
            var carryingCount = this.inventory.GetCount(Ingredient.Item.Id);

            this.itemCountText.text = $"{carryingCount} / {Ingredient.Count}";
            this.fadeImage.color = carryingCount >= Ingredient.Count ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 0.5f);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            ItemTooltip.Instance.Show(Ingredient.Item, this.rectTransform);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }
    }
}