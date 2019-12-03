using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class Interactable : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IDragHandler
    {
        public event Payload PointerUp;
        public event Payload PointerDown;
        public event Payload PointerEnter;
        public event Payload PointerExit;
        public event Payload DoubleClicked;

        public bool Active
        {
            get => this.active;
            set
            {
                if (this.active == value)
                {
                    return;
                }

                this.active = value;

                if (this.active)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }
        }

        public bool IsHovered { get; private set; }

        private const float DoubleClickTimeout = 0.25f;

        private bool active = true;
        private float lastClickTime;

        private void Start()
        {
            if (Active)
            {
                Activate();
            }
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            if (!Active)
            {
                return;
            }

            OnPointerDown();
            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (!Active || !this.IsHovered)
            {
                return;
            }

            OnPointerUp();
            PointerUp?.Invoke();

            if (Time.time - this.lastClickTime < DoubleClickTimeout)
            {
                DoubleClicked?.Invoke();
                return;
            }

            this.lastClickTime = Time.time;
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (!Active)
            {
                return;
            }

            this.IsHovered = true;
            OnPointerEnter();
            PointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            if (!Active)
            {
                return;
            }

            this.IsHovered = false;
            OnPointerExit();
            PointerExit?.Invoke();
        }

        public void OnDrag(PointerEventData pointer)
        {
        }

        private void Activate()
        {
            foreach (var selectable in GetComponents<Selectable>())
            {
                selectable.interactable = true;
            }

            OnActivate();
        }

        private void Deactivate()
        {
            foreach (var selectable in GetComponents<Selectable>())
            {
                selectable.interactable = false;
            }

            OnDeactivate();
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnDeactivate()
        {
        }

        protected virtual void OnPointerEnter()
        {
        }

        protected virtual void OnPointerExit()
        {
        }

        protected virtual void OnPointerDown()
        {
        }

        protected virtual void OnPointerUp()
        {
        }

        protected void TriggerMouseUp()
        {
            PointerUp?.Invoke();
        }
    }
}