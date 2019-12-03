using System;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class TavernFoodSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public event Payload<TavernFoodSlot> Clicked;

        public FoodType Type { get; private set; }
        public Food Food { get; private set; }

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;
        [SerializeField] private Image checkmark;
        [SerializeField] private Sprite entreeIcon;
        [SerializeField] private Sprite dessertIcon;
        [SerializeField] private Sprite drinkIcon;

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
                this.icon.color = this.icon.color.With(a: 0.5f);
                this.icon.sprite = GetDefaultSprite();
                this.checkmark.color = this.checkmark.color.With(a: 0);
                return;
            }

            Food.Applied += OnFoodAppliedOrRemoved;
            Food.Removed += OnFoodAppliedOrRemoved;

            this.icon.color = this.icon.color.With(a: 1);
            this.icon.sprite = Resources.Load<Sprite>(food.Icon);

            OnFoodAppliedOrRemoved(Food);
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

        private Sprite GetDefaultSprite()
        {
            switch (Type)
            {
                case FoodType.Entree:
                    return this.entreeIcon;
                case FoodType.Dessert:
                    return this.dessertIcon;
                case FoodType.Drink:
                    return this.drinkIcon;
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