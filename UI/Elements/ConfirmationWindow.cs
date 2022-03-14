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

        public void Show(string text, string confirm, bool success = false)
        {
            this.messageText.text = text;
            this.confirmText.text = confirm;

            this.dangerButton.gameObject.SetActive(!success);
            this.successButton.gameObject.SetActive(success);

            gameObject.SetActive(true);
        }

        protected void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            this.dangerButton.PointerClick += OnConfirmButtonPointerClick;
            this.dangerButton.gameObject.SetActive(false);

            this.successButton.PointerClick += OnConfirmButtonPointerClick;
            this.successButton.gameObject.SetActive(false);

            this.cancelButton.PointerClick += OnCancelButtonPointerClick;

            Instance.Hide();
        }

        private void OnConfirmButtonPointerClick()
        {
            Confirmed?.Invoke();
            Hide();
        }

        private void OnCancelButtonPointerClick()
        {
            Cancelled?.Invoke();
            Hide();
        }
    }
}