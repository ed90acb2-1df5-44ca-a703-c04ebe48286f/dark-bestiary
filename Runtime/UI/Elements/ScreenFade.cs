using System;
using System.Collections;
using DarkBestiary.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ScreenFade : Singleton<ScreenFade>
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Image m_Overlay;
        [SerializeField] private GameObject m_LoadIndicator;

        public void To(Action action, bool isLoading = false)
        {
            StopAllCoroutines();
            StartCoroutine(FadeToAction(action, isLoading));
        }

        private IEnumerator FadeToAction(Action action, bool isLoading)
        {
            foreach (var poolableMonoBehaviour in UIManager.Instance.PopupContainer.GetComponentsInChildren<PoolableMonoBehaviour>())
            {
                poolableMonoBehaviour.Despawn();
            }

            yield return FadeInCoroutine();

            m_LoadIndicator.SetActive(isLoading);
            yield return new WaitForFixedUpdate();
            action();
            m_LoadIndicator.SetActive(false);
            yield return FadeOutCoroutine();
        }

        private IEnumerator FadeInCoroutine()
        {
            m_Animator.SetBool("FadeOut", false);
            yield return new WaitUntil(() => m_Overlay.color.a > 0.99f);
        }

        private IEnumerator FadeOutCoroutine()
        {
            m_Animator.SetBool("FadeOut", true);
            yield return new WaitUntil(() => m_Overlay.color.a < 0.01f);
        }
    }
}