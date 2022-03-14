using System.Collections;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TextPopup : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Construct(string text)
        {
            this.text.text = text;
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