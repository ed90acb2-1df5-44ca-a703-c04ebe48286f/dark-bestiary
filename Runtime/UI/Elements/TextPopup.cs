using System.Collections;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class TextPopup : PoolableMonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        public void Construct(string text)
        {
            m_Text.text = text;
        }

        protected override void OnSpawn()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            m_CanvasGroup.alpha = 1;

            yield return new WaitForSecondsRealtime(5.0f);

            m_CanvasGroup.FadeOut(1);

            yield return new WaitForSecondsRealtime(1.0f);

            Despawn();
        }
    }
}