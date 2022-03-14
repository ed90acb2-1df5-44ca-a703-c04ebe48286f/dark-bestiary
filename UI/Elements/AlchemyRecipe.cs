using DarkBestiary.Items.Transmutation.Recipes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class AlchemyRecipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ITransmutationRecipe Recipe { get; private set; }

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private AlchemyIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientContainer;

        public void Construct(ITransmutationRecipe transmutationRecipe)
        {
            Recipe = transmutationRecipe;

            this.nameText.text = transmutationRecipe.Name;

            foreach (var ingredient in transmutationRecipe.Ingredients)
            {
                var ingredientView = Instantiate(this.ingredientPrefab, this.ingredientContainer);
                ingredientView.Construct(ingredient);
            }
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            Tooltip.Instance.Show(Recipe.Name, Recipe.Description, GetComponent<RectTransform>());
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            Tooltip.Instance.Hide();
        }
    }
}