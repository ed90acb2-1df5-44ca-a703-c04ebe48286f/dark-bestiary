using DarkBestiary.Items.Alchemy;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class AlchemyIngredient : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public IAlchemyIngredient Ingredient { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI countText;

        public void Construct(IAlchemyIngredient alchemyIngredient)
        {
            Ingredient = alchemyIngredient;
            this.icon.sprite = Resources.Load<Sprite>(alchemyIngredient.Icon);
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