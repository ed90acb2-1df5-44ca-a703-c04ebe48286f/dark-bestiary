using System;
using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class UiErrorFrame : Singleton<UiErrorFrame>
    {
        [SerializeField] private UiMessagesFrameMessage m_MessagePrefab;
        [SerializeField] private Transform m_MessageContainer;
        [SerializeField] private TMP_InputField m_TextInput;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private GameObject m_TextContainer;

        private void Start()
        {
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
        }

        private void OnCloseButtonPointerClick()
        {
            m_TextContainer.gameObject.SetActive(false);
        }

        public void ShowMessage(string text)
        {
            if (SettingsManager.Instance.DisableErrorMessages)
            {
                return;
            }

            var message = Instantiate(m_MessagePrefab, m_MessageContainer);
            message.transform.SetSiblingIndex(0);
            message.Initialize(text);
        }

        public void ShowException(Exception exception)
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);

            m_TextInput.text = $"{exception.Message}\n\n{exception.StackTrace}";
            m_TextContainer.gameObject.SetActive(true);
        }
    }
}