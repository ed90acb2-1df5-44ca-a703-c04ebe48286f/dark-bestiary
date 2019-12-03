using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TavernFood : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public event Payload<TavernFood> Clicked;

        public Food Food { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;
        [SerializeField] private Image checkmark;
        [SerializeField] private TextMeshProUGUI priceText;

        public void Initialize(Food food)
        {
            Food = food;
            Food.Applied += OnFoodAppliedOrRemoved;
            Food.Removed += OnFoodAppliedOrRemoved;

            this.icon.sprite = Resources.Load<Sprite>(food.Icon);
            this.priceText.text = food.Price.ToString();

            OnFoodAppliedOrRemoved(Food);
        }

        public void Terminate()
        {
            Food.Applied -= OnFoodAppliedOrRemoved;
            Food.Removed -= OnFoodAppliedOrRemoved;
        }

        private void OnFoodAppliedOrRemoved(Food food)
        {
            this.checkmark.color = this.checkmark.color.With(a: food.IsApplied ? 1 : 0);
        }

        public void Select()
        {
            this.outline.color = this.outline.color.With(a: 1);
        }

        public void Deselect()
        {
            this.outline.color = this.outline.color.With(a: 0);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(
                Food.Name,
                Food.Description.ToString(CharacterManager.Instance.Character.Entity),
                GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}