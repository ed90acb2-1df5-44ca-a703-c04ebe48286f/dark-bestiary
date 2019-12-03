using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class NavigationViewButton : Interactable
    {
        [SerializeField] private KeyCode hotkey;
        [SerializeField] private string tooltipKey;
        [SerializeField] private Image outline;

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
            Tooltip.Instance.Show(
                $"[{KeyCodes.GetLabel(this.hotkey)}] {I18N.Instance.Get(this.tooltipKey)}",
                GetComponent<RectTransform>());
        }

        protected override void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(this.hotkey) || EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }

            TriggerMouseUp();
        }
    }
}