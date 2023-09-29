using System;
using System.Collections;
using UnityEngine;

namespace DarkBestiary.Visuals
{
    public class Lightning : MonoBehaviour, ILightning
    {
        [SerializeField] private bool m_Wiggle;

        private bool m_IsAttached;
        private Transform m_Origin;
        private Transform m_Target;

        private float m_Counter;
        private Color m_EndColor;
        private Color m_StartColor;
        private LineRenderer m_LineRenderer;

        public void Initialize(Transform origin, Transform target)
        {
            Initialize(origin.position, target.position);
            Attach(origin, target);
        }

        public void Initialize(Vector3 origin, Vector3 target)
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            UpdatePosition(origin, target);

            if (m_Wiggle)
            {
                WiggleWiggle();
            }
        }

        private void UpdatePosition(Vector3 origin, Vector3 target)
        {
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.SetPosition(0, origin);
            m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, target);
        }

        public void Attach(Transform origin, Transform target)
        {
            m_Origin = origin;
            m_Target = target;
            m_IsAttached = true;
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

            m_StartColor = m_LineRenderer.startColor;
            m_EndColor = m_LineRenderer.endColor;

            for (var t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
            {
                m_StartColor.a = Mathf.Lerp(m_StartColor.a, 0, t);
                m_EndColor.a = Mathf.Lerp(m_EndColor.a, 0, t);

                m_LineRenderer.startColor = m_StartColor;
                m_LineRenderer.endColor = m_EndColor;
                yield return null;
            }
        }

        private void WiggleWiggle()
        {
            var positionA = m_LineRenderer.GetPosition(0);
            var positionB = m_LineRenderer.GetPosition(m_LineRenderer.positionCount - 1);

            var direction = (positionB - positionA).normalized;
            var distance = (positionB - positionA).magnitude;

            m_LineRenderer.positionCount = Math.Max((int) (distance * 2f), 2);

            var step = distance / m_LineRenderer.positionCount;

            for (var i = 0; i < m_LineRenderer.positionCount; i++)
            {
                var offset = new Vector3(Rng.Range(-0.33f, 0.33f), Rng.Range(-0.5f, 0.5f));
                var position = positionA + i * step * direction + offset;

                m_LineRenderer.SetPosition(i, position);
            }

            m_LineRenderer.SetPosition(0, positionA);
            m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, positionB);
        }

        private void Update()
        {
            if (m_IsAttached)
            {
                UpdatePosition(m_Origin.transform.position, m_Target.transform.position);
            }

            if (!m_Wiggle)
            {
                return;
            }

            m_Counter += Time.deltaTime;

            if (m_Counter <= 0.1f)
            {
                return;
            }

            m_Counter = 0;

            WiggleWiggle();
        }
    }
}