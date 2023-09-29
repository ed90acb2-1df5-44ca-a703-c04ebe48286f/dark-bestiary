using System.Collections;
using Anima2D;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class RendererExtensions
    {
        public static void FadeOut(this Renderer renderer, float duration)
        {
            Timer.Instance.StartCoroutine(FadeOutCoroutine(renderer, duration));
        }

        public static void FadeIn(this Renderer renderer, float duration)
        {
            Timer.Instance.StartCoroutine(FadeInCoroutine(renderer, duration));
        }

        private static IEnumerator FadeOutCoroutine(Renderer renderer, float duration)
        {
            while (renderer != null && renderer.material.color.a > 0)
            {
                var color = renderer.material.color;
                color.a -= Time.deltaTime / duration;
                renderer.material.color = color;
                yield return null;
            }
        }

        private static IEnumerator FadeInCoroutine(Renderer renderer, float duration)
        {
            renderer.material.color = renderer.material.color.With(a: 0);

            while (renderer != null && renderer.material.color.a < 1)
            {
                var color = renderer.material.color;
                color.a += Time.deltaTime / duration;
                renderer.material.color = color;
                yield return null;
            }
        }

        public static void FadeOut(this SpriteMeshInstance mesh, float duration)
        {
            Timer.Instance.StartCoroutine(FadeOutCoroutine(mesh, duration));
        }

        public static void FadeIn(this SpriteMeshInstance mesh, float duration)
        {
            Timer.Instance.StartCoroutine(FadeInCoroutine(mesh, duration));
        }

        private static IEnumerator FadeOutCoroutine(SpriteMeshInstance mesh, float duration)
        {
            while (mesh != null && mesh.color.a > 0)
            {
                var color = mesh.color;
                color.a -= Time.deltaTime / duration;
                mesh.color = color;
                yield return null;
            }
        }

        private static IEnumerator FadeInCoroutine(SpriteMeshInstance mesh, float duration)
        {
            mesh.color = mesh.color.With(a: 0);

            while (mesh != null && mesh.color.a < 1)
            {
                var color = mesh.color;
                color.a += Time.deltaTime / duration;
                mesh.color = color;
                yield return null;
            }
        }
    }
}