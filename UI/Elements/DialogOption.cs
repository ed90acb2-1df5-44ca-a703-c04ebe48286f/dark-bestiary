using System;
using DarkBestiary.Messaging;
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
        public event Payload<DialogOption> Clicked;

        public Action Callback { get; private set; }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int normalFontSize;
        [SerializeField] private int pressedFontSize;
        [SerializeField] private int hoverFontSize;

        private bool isHovered;

        public void Construct(string text, Color color, Action callback)
        {
            Callback = callback;

            this.text.text = text;
            this.text.color = color;
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            this.text.fontSize = this.pressedFontSize;
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (!this.isHovered)
            {
                return;
            }

            this.text.fontSize = this.normalFontSize;

            Clicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            this.text.fontSize = this.hoverFontSize;
            this.isHovered = true;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            this.text.fontSize = this.normalFontSize;
            this.isHovered = false;
        }
    }
}