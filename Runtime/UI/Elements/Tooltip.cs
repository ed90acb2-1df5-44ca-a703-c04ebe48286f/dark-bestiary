using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class Tooltip : Singleton<Tooltip>
    {
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_Text;

        private RectTransform m_RectTransform;
        private RectTransform m_ParentRectTransform;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();

            Instance.Hide();
        }

        public void Show(string text, Vector3 position)
        {
            Show("", text, position);
        }

        public void Show(string title, string text, Vector3 position)
        {
            gameObject.SetActive(true);

            m_Title.gameObject.SetActive(!string.IsNullOrEmpty(title));
            m_Title.text = title;

            m_Text.gameObject.SetActive(!string.IsNullOrEmpty(text));
            m_Text.text = text;

            m_RectTransform.pivot = new Vector2(0.5f, 0);
            m_RectTransform.position = position;
        }

        public void Show(string text, RectTransform rect)
        {
            Show("", text, rect);
        }

        public void Show(string title, string text, RectTransform rect)
        {
            gameObject.SetActive(true);

            m_Title.gameObject.SetActive(!string.IsNullOrEmpty(title));
            m_Title.text = title;

            m_Text.gameObject.SetActive(!string.IsNullOrEmpty(text));
            m_Text.text = text;

            m_RectTransform.MoveTooltip(rect, m_ParentRectTransform);
            m_RectTransform.ClampPositionToParent();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}