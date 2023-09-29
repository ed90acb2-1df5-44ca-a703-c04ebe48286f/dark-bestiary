using System;
using DarkBestiary.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TavernFoodSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public event Action<TavernFoodSlot> Clicked;

        public FoodType Type { get; private set; }
        public Food Food { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Outline;
        [SerializeField] private Image m_Checkmark;
        [SerializeField] private Sprite m_EntreeIcon;
        [SerializeField] private Sprite m_DessertIcon;
        [SerializeField] private Sprite m_DrinkIcon;

        public void Initialize(FoodType type, Food food = null)
        {
            Type = type;
            Change(food);
        }

        public void Terminate()
        {
            if (Food == null)
            {
                return;
            }

            Food.Applied -= OnFoodAppliedOrRemoved;
            Food.Removed -= OnFoodAppliedOrRemoved;
        }

        public void Change(Food food)
        {
            if (Food != null)
            {
                Food.Applied -= OnFoodAppliedOrRemoved;
                Food.Removed -= OnFoodAppliedOrRemoved;
            }

            Food = food;

            if (Food == null)
            {
                m_Icon.color = m_Icon.color.With(a: 0.5f);
                m_Icon.sprite = GetDefaultSprite();
                m_Checkmark.color = m_Checkmark.color.With(a: 0);
                return;
            }

            Food.Applied += OnFoodAppliedOrRemoved;
            Food.Removed += OnFoodAppliedOrRemoved;

            m_Icon.color = m_Icon.color.With(a: 1);
            m_Icon.sprite = Resources.Load<Sprite>(food.Icon);

            OnFoodAppliedOrRemoved(Food);
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

        private Sprite GetDefaultSprite()
        {
            switch (Type)
            {
                case FoodType.Entree:
                    return m_EntreeIcon;
                case FoodType.Dessert:
                    return m_DessertIcon;
                case FoodType.Drink:
                    return m_DrinkIcon;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (Food == null)
            {
                return;
            }

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