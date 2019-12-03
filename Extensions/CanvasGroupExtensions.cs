using System.Collections;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class CanvasGroupExtensions
    {
        public static void FadeOut(this CanvasGroup canvasGroup, float duration)
        {
            Timer.Instance.StartCoroutine(FadeOutCoroutine(canvasGroup, duration));
        }

        public static void FadeIn(this CanvasGroup canvasGroup, float duration)
        {
            Timer.Instance.StartCoroutine(FadeInCoroutine(canvasGroup, duration));
        }

        private static IEnumerator FadeOutCoroutine(CanvasGroup canvasGroup, float duration)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime / duration;
                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        private static IEnumerator FadeInCoroutine(CanvasGroup canvasGroup, float duration)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime / duration;
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }
}