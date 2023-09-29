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
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private GameObject[] m_Stars;

        public void Construct(Mastery mastery)
        {
            m_NameText.text = mastery.Name;

            foreach (var star in m_Stars)
            {
                star.gameObject.SetActive(false);
            }

            foreach (var star in m_Stars.Take(mastery.Experience.Level - 1))
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
            m_CanvasGroup.alpha = 1;

            yield return new WaitForSecondsRealtime(5.0f);

            m_CanvasGroup.FadeOut(1);

            yield return new WaitForSecondsRealtime(1.0f);

            Despawn();
        }
    }
}