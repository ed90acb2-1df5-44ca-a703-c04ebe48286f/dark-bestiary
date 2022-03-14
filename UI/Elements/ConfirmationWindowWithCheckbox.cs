using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ConfirmationWindowWithCheckbox : Singleton<ConfirmationWindowWithCheckbox>
    {
        public event Payload<bool> Confirmed;
        public event Payload<bool> Cancelled;

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI confirmText;
        [SerializeField] private Interactable dangerButton;
        [SerializeField] private Interactable successButton;
        [SerializeField] private Interactable cancelButton;
        [SerializeField] private Toggle checkbox;

        public void Show(string text, string confirm, bool success = false)
        {
            this.messageText.text = text;
            this.confirmText.text = confirm;

            this.dangerButton.gameObject.SetActive(!success);
            this.successButton.gameObject.SetActive(success);

            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            this.checkbox.isOn = false;

            this.dangerButton.PointerClick += OnConfirmButtonPointerClick;
            this.dangerButton.gameObject.SetActive(false);

            this.successButton.PointerClick += OnConfirmButtonPointerClick;
            this.successButton.gameObject.SetActive(false);

            this.cancelButton.PointerClick += OnCancelButtonPointerClick;

            Instance.Hide();
        }

        private void OnConfirmButtonPointerClick()
        {
            Confirmed?.Invoke(this.checkbox.isOn);
            Hide();
        }

        private void OnCancelButtonPointerClick()
        {
            Cancelled?.Invoke(this.checkbox.isOn);
            Hide();
        }
    }
}