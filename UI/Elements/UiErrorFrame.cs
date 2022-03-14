using System;
using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class UiErrorFrame : Singleton<UiErrorFrame>
    {
        [SerializeField] private UiMessagesFrameMessage messagePrefab;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private TMP_InputField textInput;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private GameObject textContainer;

        private void Start()
        {
            this.closeButton.PointerClick += OnCloseButtonPointerClick;
        }

        private void OnCloseButtonPointerClick()
        {
            this.textContainer.gameObject.SetActive(false);
        }

        public void ShowMessage(string text)
        {
            if (SettingsManager.Instance.DisableErrorMessages)
            {
                return;
            }

            var message = Instantiate(this.messagePrefab, this.messageContainer);
            message.transform.SetSiblingIndex(0);
            message.Initialize(text);
        }

        public void ShowException(Exception exception)
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);

            this.textInput.text = $"{exception.Message}\n\n{exception.StackTrace}";
            this.textContainer.gameObject.SetActive(true);
        }
    }
}