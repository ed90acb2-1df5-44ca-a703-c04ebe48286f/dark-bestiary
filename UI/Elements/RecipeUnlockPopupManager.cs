using DarkBestiary.Items;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RecipeUnlockPopupManager : Singleton<RecipeUnlockPopupManager>
    {
        [SerializeField] private RecipeUnlockPopup recipeUnlockPopup;

        private MonoBehaviourPool<RecipeUnlockPopup> pool;

        private void Start()
        {
            this.pool = MonoBehaviourPool<RecipeUnlockPopup>.Factory(
                this.recipeUnlockPopup, UIManager.Instance.PopupContainer, 2);

            Character.RecipeUnlocked += OnRecipeUnlocked;
        }

        private void OnRecipeUnlocked(Recipe recipe)
        {
            this.pool.Spawn().Construct(recipe);
        }
    }
}