using DarkBestiary.Items.Transmutation.Recipes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class AlchemyRecipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public ITransmutationRecipe Recipe { get; private set; }

        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private AlchemyIngredient m_IngredientPrefab;
        [SerializeField] private Transform m_IngredientContainer;

        public void Construct(ITransmutationRecipe transmutationRecipe)
        {
            Recipe = transmutationRecipe;

            m_NameText.text = transmutationRecipe.Name;

            foreach (var ingredient in transmutationRecipe.Ingredients)
            {
                var ingredientView = Instantiate(m_IngredientPrefab, m_IngredientContainer);
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