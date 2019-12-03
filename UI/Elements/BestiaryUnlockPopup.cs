using System.Collections;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class BestiaryUnlockPopup : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Construct(UnitComponent unit)
        {
            this.nameText.text = unit.Name;
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