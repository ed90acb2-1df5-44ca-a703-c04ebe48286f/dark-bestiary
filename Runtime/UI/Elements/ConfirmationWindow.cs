using System;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ConfirmationWindow : Singleton<ConfirmationWindow>
    {
        public event Action Confirmed;
        public event Action Cancelled;

        [SerializeField] private TextMeshProUGUI m_MessageText;
        [SerializeField] private TextMeshProUGUI m_ConfirmText;
        [SerializeField] private Interactable m_DangerButton;
        [SerializeField] private Interactable m_SuccessButton;
        [SerializeField] private Interactable m_CancelButton;

        public void Show(string text, string confirm, bool success = false)
        {
            m_MessageText.text = text;
            m_ConfirmText.text = confirm;

            m_DangerButton.gameObject.SetActive(!success);
            m_SuccessButton.gameObject.SetActive(success);

            gameObject.SetActive(true);
        }

        protected void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            m_DangerButton.PointerClick += OnConfirmButtonPointerClick;
            m_DangerButton.gameObject.SetActive(false);

            m_SuccessButton.PointerClick += OnConfirmButtonPointerClick;
            m_SuccessButton.gameObject.SetActive(false);

            m_CancelButton.PointerClick += OnCancelButtonPointerClick;

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