using System;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class RenameCharacterPopup : MonoBehaviour
    {
        public event Action<string> Confirmed;

        [SerializeField] private TMP_InputField m_Input;
        [SerializeField] private Interactable m_ConfirmButton;
        [SerializeField] private Interactable m_CancelButton;

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
            m_ConfirmButton.PointerClick += OnConfirmButtonPointerClick;
            m_CancelButton.PointerClick += OnCancelButtonPointerClick;
        }

        private void OnConfirmButtonPointerClick()
        {
            Confirmed?.Invoke(m_Input.text);
            Hide();
        }

        private void OnCancelButtonPointerClick()
        {
            Hide();
        }
    }
}