using DarkBestiary.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class NavigationViewButton : Interactable
    {
        [SerializeField] private string tooltipKey;
        [SerializeField] private Image outline;

        private KeyCode keyCode = KeyCode.None;

        public void ChangeHotkey(KeyCode keyCode)
        {
            this.keyCode = keyCode;
        }

        public void Highlight()
        {
            this.outline.gameObject.SetActive(true);
        }

        public void Unhighlight()
        {
            this.outline.gameObject.SetActive(false);
        }

        protected override void OnPointerEnter()
        {
            var keyCodePrefix = this.keyCode == KeyCode.None ? "" : $"[{EnumTranslator.Translate(this.keyCode)}] ";

            Tooltip.Instance.Show($"{keyCodePrefix}{I18N.Instance.Get(this.tooltipKey)}", GetComponent<RectTransform>());
        }

        protected override void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(this.keyCode) || EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }

            TriggerMouseUp();
        }
    }
}