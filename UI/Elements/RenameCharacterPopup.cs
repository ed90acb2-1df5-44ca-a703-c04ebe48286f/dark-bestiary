using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RenameCharacterPopup : MonoBehaviour
    {
        public event Payload<string> Confirmed;

        [SerializeField] private TMP_InputField input;
        [SerializeField] private Interactable confirmButton;
        [SerializeField] private Interactable cancelButton;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            this.confirmButton.PointerClick += OnConfirmButtonPointerClick;
            this.cancelButton.PointerClick += OnCancelButtonPointerClick;
        }

        private void OnConfirmButtonPointerClick()
        {
            Confirmed?.Invoke(this.input.text);
            Hide();
        }

        private void OnCancelButtonPointerClick()
        {
            Hide();
        }
    }
}