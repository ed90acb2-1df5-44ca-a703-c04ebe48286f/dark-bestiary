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

            this.rectTransform.pivot = new Vector2(
                rect.position.x > (float) Screen.width / 2 ? 1 : 0,
                rect.position.y > (float) Screen.height / 2 ? 1 : 0);

            this.rectTransform.position = rect.position + new Vector3(
                                              rect.rect.width /
                                              2 * (Mathf.Approximately(this.rectTransform.pivot.x, 0) ? 1 : -1)
                                                * this.parentRectTransform.localScale.x,
                                              rect.rect.height /
                                              2 * (Mathf.Approximately(this.rectTransform.pivot.y, 1) ? 1 : -1)
                                                * this.parentRectTransform.localScale.y
                                          );

            this.rectTransform.ClampPositionToParent();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}