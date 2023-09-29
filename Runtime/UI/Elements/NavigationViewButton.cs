using DarkBestiary.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class NavigationViewButton : Interactable
    {
        [SerializeField] private string m_TooltipKey;
        [SerializeField] private Image m_Outline;

        private KeyCode m_KeyCode = KeyCode.None;

        public void ChangeHotkey(KeyCode keyCode)
        {
            m_KeyCode = keyCode;
        }

        public void Highlight()
        {
            m_Outline.gameObject.SetActive(true);
        }

        public void Unhighlight()
        {
            m_Outline.gameObject.SetActive(false);
        }

        protected override void OnPointerEnter()
        {
            var keyCodePrefix = m_KeyCode == KeyCode.None ? "" : $"[{EnumTranslator.Translate(m_KeyCode)}] ";

            Tooltip.Instance.Show($"{keyCodePrefix}{I18N.Instance.Get(m_TooltipKey)}", GetComponent<RectTransform>());
        }

        protected override void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(m_KeyCode) || EventSystem.current.currentSelectedGameObject != null)
            {
                return;
            }

            TriggerMouseUp();
        }
    }
}