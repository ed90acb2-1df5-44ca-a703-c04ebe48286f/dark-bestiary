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
        [SerializeField] private TextMeshProUGUI m_TitlePrefab;
        [SerializeField] private TextMeshProUGUI m_TextPrefab;
        [SerializeField] private TextMeshProUGUI m_PhrasePrefab;
        [SerializeField] private DialogOption m_OptionPrefab;
        [SerializeField] private GameObject m_SpacePrefab;

        public string Title { get; private set; }

        private bool m_IsHovered;
        private CanvasGroup m_CanvasGroup;
        private RectTransform m_RectTransform;

        private void Start()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
            Instance.Hide();
        }

        public Dialog AddTitle(string title)
        {
            Title = title;
            Instantiate(m_TitlePrefab, transform).text = title;
            return this;
        }

        public Dialog AddText(string text)
        {
            Instantiate(m_TextPrefab, transform).text = text;
            return this;
        }

        public Dialog AddSpace()
        {
            Instantiate(m_SpacePrefab, transform);
            return this;
        }

        public Dialog AddOption(string text, Action callback = null)
        {
            return AddOption(text, callback, Color.white);
        }

        public Dialog AddOption(string text, Action callback, Color color)
        {
            var option = Instantiate(m_OptionPrefab, transform);
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
            m_CanvasGroup.alpha = 0;

            gameObject.SetActive(true);

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);

                m_RectTransform.position = position;
                m_RectTransform.ClampPositionToParent();

                m_CanvasGroup.alpha = 1;
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
            m_IsHovered = true;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_IsHovered = false;
        }

        private void Update()
        {
            if (m_IsHovered)
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