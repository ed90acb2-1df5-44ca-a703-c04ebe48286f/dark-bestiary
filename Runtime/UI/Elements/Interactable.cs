using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class Interactable : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IDropHandler
    {
        public event Action<GameObject> Dropped;
        public event Action PointerClick;
        public event Action PointerDown;
        public event Action PointerUp;
        public event Action PointerEnter;
        public event Action PointerExit;

        public bool Active
        {
            get => m_Active;
            set
            {
                if (m_Active == value)
                {
                    return;
                }

                m_Active = value;

                if (m_Active)
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

        private const float c_DoubleClickTimeout = 0.25f;

        private bool m_Active = true;

        private void Start()
        {
            if (Active)
            {
                Activate();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Active || !IsHovered)
            {
                return;
            }

            PointerClick?.Invoke();
            OnPointerClick();
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            if (!Active)
            {
                return;
            }

            PointerDown?.Invoke();
            OnPointerDown();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (!Active || !IsHovered)
            {
                return;
            }

            PointerUp?.Invoke();
            OnPointerUp();
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (!Active)
            {
                return;
            }

            IsHovered = true;
            OnPointerEnter();
            PointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            if (!Active)
            {
                return;
            }

            IsHovered = false;
            OnPointerExit();
            PointerExit?.Invoke();
        }

        public void OnDrop(PointerEventData pointer)
        {
            Dropped?.Invoke(pointer.pointerDrag);
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

        protected virtual void OnPointerClick()
        {
        }

        protected virtual void OnPointerUp()
        {
        }

        protected virtual void OnPointerDown()
        {
        }

        protected virtual void OnPointerEnter()
        {
        }

        protected virtual void OnPointerExit()
        {
        }

        protected void TriggerMouseUp()
        {
            PointerClick?.Invoke();
        }
    }
}