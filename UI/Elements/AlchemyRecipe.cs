using DarkBestiary.Items.Alchemy;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class AlchemyRecipe : MonoBehaviour
    {
        public IAlchemyRecipe Recipe { get; private set; }

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private AlchemyIngredient ingredientPrefab;
        [SerializeField] private Transform ingredientContainer;

        public void Construct(IAlchemyRecipe alchemyRecipe)
        {
            Recipe = alchemyRecipe;

            this.nameText.text = alchemyRecipe.Name;

            foreach (var ingredient in alchemyRecipe.Ingredients)
            {
                var ingredientView = Instantiate(this.ingredientPrefab, this.ingredientContainer);
                ingredientView.Construct(ingredient);
            }
        }
    }
}