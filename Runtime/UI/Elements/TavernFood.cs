using System;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TavernFood : PoolableMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public event Action<TavernFood> Clicked;

        public Food Food { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Outline;
        [SerializeField] private Image m_Checkmark;
        [SerializeField] private TextMeshProUGUI m_PriceText;

        public void Initialize(Food food)
        {
            Food = food;
            Food.Applied += OnFoodAppliedOrRemoved;
            Food.Removed += OnFoodAppliedOrRemoved;

            m_Icon.sprite = Resources.Load<Sprite>(food.Icon);
            m_PriceText.text = food.Price.ToString();

            OnFoodAppliedOrRemoved(Food);
        }

        public void Terminate()
        {
            Food.Applied -= OnFoodAppliedOrRemoved;
            Food.Removed -= OnFoodAppliedOrRemoved;
        }

        private void OnFoodAppliedOrRemoved(Food food)
        {
            m_Checkmark.color = m_Checkmark.color.With(a: food.IsApplied ? 1 : 0);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(
                Food.Name,
                Food.Description.ToString(Game.Instance.Character.Entity),
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