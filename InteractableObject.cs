using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary
{
    public class InteractableObject : MonoBehaviour
    {
        public event Payload MouseDown;
        public event Payload MouseUp;
        public event Payload MouseEnter;
        public event Payload MouseExit;

        [SerializeField] private Vector3 tooltipOffset;

        private string tooltipName;
        private string tooltipInfo;
        private bool showTooltip;

        private bool isHovered;

        public void Construct(string tooltipName = "", string tooltipInfo = "")
        {
            this.tooltipName = tooltipName;
            this.tooltipInfo = tooltipInfo;
            this.showTooltip = !string.IsNullOrEmpty(this.tooltipName) || !string.IsNullOrEmpty(this.tooltipInfo);
        }

        private void Start()
        {
            OnMouseExit();
        }

        private void OnDisable()
        {
            if (!this.isHovered)
            {
                return;
            }

            OnMouseExit();
        }

        private void OnMouseEnter()
        {
            this.isHovered = true;

            if (UIManager.Instance.IsGameFieldBlockedByUI())
            {
                return;
            }

            OnPointerEnter();

            MouseEnter?.Invoke();

            if (this.showTooltip)
            {
                ObjectTooltip.Instance.Show(
                    this.tooltipName,
                    this.tooltipInfo,
                    Camera.main.WorldToScreenPoint(transform.position + this.tooltipOffset)
                );
            }
        }

        private void OnMouseExit()
        {
            this.isHovered = false;

            OnPointerExit();

            MouseExit?.Invoke();

            if (this.showTooltip)
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

            if (this.showTooltip)
            {
                ObjectTooltip.Instance.Hide();
            }
        }

        private void OnMouseUpAsButton()
        {
            if (!this.isHovered || UIManager.Instance.IsGameFieldBlockedByUI())
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