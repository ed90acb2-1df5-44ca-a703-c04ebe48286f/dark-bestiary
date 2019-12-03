using System;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class Dialog : Singleton<Dialog>, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI titlePrefab;
        [SerializeField] private TextMeshProUGUI textPrefab;
        [SerializeField] private TextMeshProUGUI phrasePrefab;
        [SerializeField] private DialogOption optionPrefab;
        [SerializeField] private GameObject spacePrefab;

        public string Title { get; private set; }

        private bool isHovered;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        private void Start()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();
            this.rectTransform = GetComponent<RectTransform>();
            Instance.Hide();
        }

        public Dialog AddTitle(string title)
        {
            Title = title;
            Instantiate(this.titlePrefab, transform).text = title;
            return this;
        }

        public Dialog AddText(string text)
        {
            Instantiate(this.textPrefab, transform).text = text;
            return this;
        }

        public Dialog AddPhrase(string text)
        {
            Instantiate(this.phrasePrefab, transform).text = text;
            return this;
        }

        public Dialog AddSpace()
        {
            Instantiate(this.spacePrefab, transform);
            return this;
        }

        public Dialog AddOption(string text, Action callback = null)
        {
            return AddOption(text, callback, Color.white);
        }

        public Dialog AddOption(string text, Action callback, Color color)
        {
            var option = Instantiate(this.optionPrefab, transform);
            option.Clicked += OnOptionClicked;
            option.Construct(text, color, callback);

            return this;
        }

        public void Show()
        {
            Show(Input.mousePosition);
        }

        public void Show(Vector3 position)
        {
            this.canvasGroup.alpha = 0;

            gameObject.SetActive(true);

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);

                this.rectTransform.position = position;
                this.rectTransform.ClampPositionToParent();

                this.canvasGroup.alpha = 1;
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public Dialog Clear()
        {
            foreach (var child in GetComponentsInChildren<Transform>())
            {
                if (gameObject.Equals(child.gameObject))
                {
                    continue;
                }

                Destroy(child.gameObject);
            }

            return this;
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            this.isHovered = true;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            this.isHovered = false;
        }

        private void Update()
        {
            if (this.isHovered)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }

        private void OnOptionClicked(DialogOption option)
        {
            option.Callback?.Invoke();
            Hide();
        }
    }
}