using System.Collections;
using DarkBestiary.Utility;
using UnityEngine;

namespace DarkBestiary.Extensions
{
    public static class TransformExtensions
    {
        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            transform.rotation = QuaternionUtility.LookRotation2D(target - transform.position);
        }

        public static void Shrink(this Transform transform, float targetScale, float duration)
        {
            Timer.Instance.StartCoroutine(ShrinkCoroutine(transform, targetScale, duration));
        }

        public static void Expand(this Transform transform, float targetScale, float duration)
        {
            Timer.Instance.StartCoroutine(ExpandCoroutine(transform, targetScale, duration));
        }

        private static IEnumerator ShrinkCoroutine(Transform transform, float targetScale, float duration)
        {
            while (transform.localScale.x > targetScale)
            {
                var step = Time.deltaTime / duration;

                transform.localScale = new Vector3(
                    transform.localScale.x - step,
                    transform.localScale.y - step,
                    transform.localScale.z - step
                );

                yield return null;
            }
        }

        private static IEnumerator ExpandCoroutine(Transform transform, float targetScale, float duration)
        {
            while (transform.localScale.x < targetScale)
            {
                var step = Time.deltaTime / duration;

                transform.localScale = new Vector3(
                    transform.localScale.x + step,
                    transform.localScale.y + step,
                    transform.localScale.z + step);

                yield return null;
            }
        }

        public static void ChangeAnchors(this RectTransform transform, Vector2 anchorMin, Vector2 anchorMax)
        {
            var origin = transform.position;
            transform.anchorMin = anchorMin;
            transform.anchorMax = anchorMax;
            transform.position = origin;
        }

        public static void ChangePivot(this RectTransform rectTransform, Vector2 pivot)
        {
            var deltaPivot = rectTransform.pivot - pivot;
            var deltaPosition = new Vector3(
                deltaPivot.x * rectTransform.rect.size.x,
                deltaPivot.y * rectTransform.rect.size.y);

            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public static void ClampPositionToParent(this RectTransform transform)
        {
            transform.ClampPositionToParent(transform.parent.GetComponent<RectTransform>());
        }

        public static void MoveTooltip(this RectTransform transform, RectTransform target, RectTransform parent)
        {
            transform.pivot = new Vector2(
                target.position.x > (float) Screen.width / 2 ? 1 : 0,
                target.position.y > (float) Screen.height / 2 ? 1 : 0);

            transform.position = target.position + new Vector3(
                                     target.rect.width * target.pivot.x *
                                     (Mathf.Approximately(transform.pivot.x, 0) ? 1 : -1) *
                                     parent.localScale.x,

                                     target.rect.height * target.pivot.y *
                                     (Mathf.Approximately(transform.pivot.y, 1) ? 1 : -1) *
                                     parent.localScale.y
                                 );
        }


        public static void ClampPositionToParent(this RectTransform transform, RectTransform parent)
        {
            var clamped = transform.localPosition;

            var minPosition = parent.rect.min - transform.rect.min;
            var maxPosition = parent.rect.max - transform.rect.max;

            clamped.x = Mathf.Clamp(transform.localPosition.x, minPosition.x, maxPosition.x);
            clamped.y = Mathf.Clamp(transform.localPosition.y, minPosition.y, maxPosition.y);

            transform.localPosition = clamped;
        }
    }
}