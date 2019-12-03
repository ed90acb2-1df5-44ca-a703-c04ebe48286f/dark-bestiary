using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ConfirmationWindow : Singleton<ConfirmationWindow>
    {
        public event Payload Confirmed;
        public event Payload Cancelled;

        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI confirmText;
        [SerializeField] private Interactable dangerButton;
        [SerializeField] private Interactable successButton;
        [SerializeField] private Interactable cancelButton;

        private void Start()
        {
            this.dangerButton.PointerUp += OnConfirmButtonPointerUp;
            this.dangerButton.gameObject.SetActive(false);

            this.successButton.PointerUp += OnConfirmButtonPointerUp;
            this.successButton.gameObject.SetActive(false);

            this.cancelButton.PointerUp += OnCancelButtonPointerUp;

            Instance.Hide();
        }

        private void OnConfirmButtonPointerUp()
        {
            Confirmed?.Invoke();
            Hide();
        }

        private void OnCancelButtonPointerUp()
        {
            Cancelled?.Invoke();
            Hide();
        }

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
    }
}