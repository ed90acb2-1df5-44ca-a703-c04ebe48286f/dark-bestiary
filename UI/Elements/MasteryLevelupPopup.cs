using System.Collections;
using System.Linq;
using DarkBestiary.Extensions;
using DarkBestiary.Masteries;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class MasteryLevelupPopup : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject[] stars;

        public void Construct(Mastery mastery)
        {
            this.nameText.text = mastery.Name;

            foreach (var star in this.stars)
            {
                star.gameObject.SetActive(false);
            }

            foreach (var star in this.stars.Take(mastery.Experience.Level))
            {
                star.gameObject.SetActive(true);
            }
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