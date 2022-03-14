using DarkBestiary.Items.Transmutation.Ingredients;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class AlchemyIngredient : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ITransmutationIngredient Ingredient { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;

        public void Construct(ITransmutationIngredient transmutationIngredient)
        {
            Ingredient = transmutationIngredient;
            this.icon.sprite = Resources.Load<Sprite>(transmutationIngredient.Icon);
            this.countText.text = $"x{Ingredient.Count}";
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(Ingredient.Name, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }
    }
}