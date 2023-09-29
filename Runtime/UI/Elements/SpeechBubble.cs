using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class SpeechBubble : Singleton<SpeechBubble>, IPointerUpHandler
    {
        public event Action Hidden;

        [SerializeField] private TextMeshProUGUI m_Text;

        private readonly Vector3 m_Offset = new(0, 2, 0);

        private Camera m_MainCamera;
        private Transform m_Target;
        private bool m_IsTemporary;
        private float m_Duration;

        private void Start()
        {
            Instance.Hide();
        }

        public void Show(string text, Transform target, float duration)
        {
            m_IsTemporary = true;
            m_Duration = duration;

            OnShow(text, target);
        }

        public void Show(string text, Transform target)
        {
            m_IsTemporary = false;

            OnShow(text, target);
        }

        private void OnShow(string text, Transform target)
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);

            m_MainCamera = Camera.main;
            m_Text.text = text;
            m_Target = target;

            Update();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            gameObject.SetActive(false);
            Hidden?.Invoke();
        }

        private void Update()
        {
            transform.position = m_MainCamera.WorldToScreenPoint(m_Target.position + m_Offset);

            if (!m_IsTemporary)
            {
                return;
            }

            m_Duration -= Time.deltaTime;

            if (m_Duration <= 0)
            {
                m_IsTemporary = false;
                m_Duration = 0;
                Hide();
            }
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Hide();
        }
    }
}