using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class Tooltip : Singleton<Tooltip>
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI text;

        private RectTransform rectTransform;
        private RectTransform parentRectTransform;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();

            Instance.Hide();
        }

        public void Show(string text, Vector3 position)
        {
            Show("", text, position);
        }

        public void Show(string title, string text, Vector3 position)
        {
            gameObject.SetActive(true);

            this.title.gameObject.SetActive(!string.IsNullOrEmpty(title));
            this.title.text = title;

            this.text.gameObject.SetActive(!string.IsNullOrEmpty(text));
            this.text.text = text;

            this.rectTransform.pivot = new Vector2(0.5f, 0);
            this.rectTransform.position = position;
        }

        public void Show(string text, RectTransform rect)
        {
            Show("", text, rect);
        }

        public void Show(string title, string text, RectTransform rect)
        {
            gameObject.SetActive(true);

            this.title.gameObject.SetActive(!string.IsNullOrEmpty(title));
            this.title.text = title;

            this.text.gameObject.SetActive(!string.IsNullOrEmpty(text));
            this.text.text = text;

            this.rectTransform.MoveTooltip(rect, this.parentRectTransform);
            this.rectTransform.ClampPositionToParent();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}