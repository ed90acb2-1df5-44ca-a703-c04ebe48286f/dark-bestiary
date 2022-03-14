using System;
using System.Collections;
using DarkBestiary.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ScreenFade : Singleton<ScreenFade>
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Image overlay;
        [SerializeField] private GameObject loadIndicator;

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

            this.loadIndicator.SetActive(isLoading);
            yield return new WaitForFixedUpdate();
            action();
            this.loadIndicator.SetActive(false);
            yield return FadeOutCoroutine();
        }

        private IEnumerator FadeInCoroutine()
        {
            this.animator.SetBool("FadeOut", false);
            yield return new WaitUntil(() => this.overlay.color.a > 0.99f);
        }

        private IEnumerator FadeOutCoroutine()
        {
            this.animator.SetBool("FadeOut", true);
            yield return new WaitUntil(() => this.overlay.color.a < 0.01f);
        }
    }
}