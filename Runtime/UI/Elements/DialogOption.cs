using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class DialogOption : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public event Action<DialogOption> Clicked;

        public Action Callback { get; private set; }

        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private int m_NormalFontSize;
        [SerializeField] private int m_PressedFontSize;
        [SerializeField] private int m_HoverFontSize;

        private bool m_IsHovered;

        public void Construct(string text, Color color, Action callback)
        {
            Callback = callback;

            m_Text.text = text;
            m_Text.color = color;
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            m_Text.fontSize = m_PressedFontSize;
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (!m_IsHovered)
            {
                return;
            }

            m_Text.fontSize = m_NormalFontSize;

            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            m_Text.fontSize = m_HoverFontSize;
            m_IsHovered = true;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            m_Text.fontSize = m_NormalFontSize;
            m_IsHovered = false;
        }
    }
}