using System.Collections;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class RecipeUnlockPopup : PoolableMonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Construct(Recipe recipe)
        {
            this.nameText.text = recipe.Item.ColoredName;
            this.icon.sprite = Resources.Load<Sprite>(recipe.Item.Icon);
        }

        protected override void OnSpawn()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            this.canvasGroup.alpha = 1;

            yield return new WaitForSecondsRealtime(5.0f);

            this.canvasGroup.FadeOut(1);

            yield return new WaitForSecondsRealtime(1.0f);

            Despawn();
        }
    }
}