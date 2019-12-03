using System;
using System.Collections;
using UnityEngine;

namespace DarkBestiary.Visuals
{
    public class Lightning : MonoBehaviour, ILightning
    {
        [SerializeField] private bool wiggle;

        private bool isAttached;
        private Transform origin;
        private Transform target;

        private float counter;
        private Color endColor;
        private Color startColor;
        private LineRenderer lineRenderer;

        public void Initialize(Transform origin, Transform target)
        {
            Initialize(origin.position, target.position);
            Attach(origin, target);
        }

        public void Initialize(Vector3 origin, Vector3 target)
        {
            this.lineRenderer = GetComponent<LineRenderer>();
            UpdatePosition(origin, target);

            if (this.wiggle)
            {
                WiggleWiggle();
            }
        }

        private void UpdatePosition(Vector3 origin, Vector3 target)
        {
            this.lineRenderer.positionCount = 2;
            this.lineRenderer.SetPosition(0, origin);
            this.lineRenderer.SetPosition(this.lineRenderer.positionCount - 1, target);
        }

        public void Attach(Transform origin, Transform target)
        {
            this.origin = origin;
            this.target = target;
            this.isAttached = true;
        }

        public void FadeOut(float duration)
        {
            StartCoroutine(FadeOutCoroutine(0, duration));
        }

        public void FadeOut(float delay, float duration)
        {
            StartCoroutine(FadeOutCoroutine(delay, duration));
        }

        public void Destroy(float delay)
        {
            Destroy(gameObject, delay);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private IEnumerator FadeOutCoroutine(float delay, float duration)
        {
            yield return new WaitForSeconds(delay);

            this.startColor = this.lineRenderer.startColor;
            this.endColor = this.lineRenderer.endColor;

            for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
            {
                this.startColor.a = Mathf.Lerp(this.startColor.a, 0, t);
                this.endColor.a = Mathf.Lerp(this.endColor.a, 0, t);

                this.lineRenderer.startColor = this.startColor;
                this.lineRenderer.endColor = this.endColor;
                yield return null;
            }
        }

        private void WiggleWiggle()
        {
            var positionA = this.lineRenderer.GetPosition(0);
            var positionB = this.lineRenderer.GetPosition(this.lineRenderer.positionCount - 1);

            var direction = (positionB - positionA).normalized;
            var distance = (positionB - positionA).magnitude;

            this.lineRenderer.positionCount = Math.Max((int) (distance * 2f), 2);

            var step = distance / this.lineRenderer.positionCount;

            for (var i = 0; i < this.lineRenderer.positionCount; i++)
            {
                var offset = new Vector3(RNG.Range(-0.33f, 0.33f), RNG.Range(-0.5f, 0.5f));
                var position = positionA + i * step * direction + offset;

                this.lineRenderer.SetPosition(i, position);
            }

            this.lineRenderer.SetPosition(0, positionA);
            this.lineRenderer.SetPosition(this.lineRenderer.positionCount - 1, positionB);
        }

        private void Update()
        {
            if (this.isAttached)
            {
                UpdatePosition(this.origin.transform.position, this.target.transform.position);
            }

            if (!this.wiggle)
            {
                return;
            }

            this.counter += Time.deltaTime;

            if (this.counter <= 0.1f)
            {
                return;
            }

            this.counter = 0;

            WiggleWiggle();
        }
    }
}