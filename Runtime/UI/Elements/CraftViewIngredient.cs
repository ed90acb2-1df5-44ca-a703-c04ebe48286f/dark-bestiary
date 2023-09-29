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
        [SerializeField] private InventoryItem m_Item;
        [SerializeField] private Image m_FadeImage;
        [SerializeField] private TextMeshProUGUI m_ItemCountText;

        public RecipeIngredient Ingredient { get; private set; }

        private RectTransform m_RectTransform;
        private InventoryComponent m_Inventory;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        public void Construct(RecipeIngredient ingredient, InventoryComponent inventory)
        {
            Ingredient = ingredient;

            m_Inventory = inventory;
            m_FadeImage.color = new Color(0, 0, 0, 0);

            m_Item.IsDraggable = false;
            m_Item.Change(ingredient.Item);
            m_Item.OverwriteStackCount(0);

            Refresh();
        }

        public void Refresh()
        {
            var carryingCount = m_Inventory.GetCount(Ingredient.Item.Id);

            m_ItemCountText.text = $"{carryingCount} / {Ingredient.Count}";
            m_FadeImage.color = carryingCount >= Ingredient.Count ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 0.5f);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            ItemTooltip.Instance.Show(Ingredient.Item, m_RectTransform);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            ItemTooltip.Instance.Hide();
        }
    }
}