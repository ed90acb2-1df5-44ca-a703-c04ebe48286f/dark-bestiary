using System;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary
{
    public class InteractableObject : MonoBehaviour
    {
        public event Action MouseDown;
        public event Action MouseUp;
        public event Action MouseEnter;
        public event Action MouseExit;

        [SerializeField] private Vector3 m_TooltipOffset;

        private string m_TooltipName;
        private string m_TooltipInfo;
        private bool m_ShowTooltip;

        private bool m_IsHovered;

        public void Construct(string tooltipName = "", string tooltipInfo = "")
        {
            m_TooltipName = tooltipName;
            m_TooltipInfo = tooltipInfo;
            m_ShowTooltip = !string.IsNullOrEmpty(m_TooltipName) || !string.IsNullOrEmpty(m_TooltipInfo);
        }

        private void Start()
        {
            OnMouseExit();
        }

        private void OnDisable()
        {
            if (!m_IsHovered)
            {
                return;
            }

            OnMouseExit();
        }

        private void OnMouseEnter()
        {
            m_IsHovered = true;

            if (UIManager.Instance.IsGameFieldBlockedByUI())
            {
                return;
            }

            OnPointerEnter();

            MouseEnter?.Invoke();

            if (m_ShowTooltip)
            {
                ObjectTooltip.Instance.Show(
                    m_TooltipName,
                    m_TooltipInfo,
                    Camera.main.WorldToScreenPoint(transform.position + m_TooltipOffset)
                );
            }
        }

        private void OnMouseExit()
        {
            m_IsHovered = false;

            OnPointerExit();

            MouseExit?.Invoke();

            if (m_ShowTooltip)
            {
                ObjectTooltip.Instance.Hide();
            }
        }

        private void OnMouseDown()
        {
            if (UIManager.Instance.IsGameFieldBlockedByUI())
            {
                return;
            }

            OnPointerDown();

            MouseDown?.Invoke();

            if (m_ShowTooltip)
            {
                ObjectTooltip.Instance.Hide();
            }
        }

        private void OnMouseUpAsButton()
        {
            if (!m_IsHovered || UIManager.Instance.IsGameFieldBlockedByUI())
            {
                return;
            }

            OnPointerUp();

            MouseUp?.Invoke();
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
    }
}