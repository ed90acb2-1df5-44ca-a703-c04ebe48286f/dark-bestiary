using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class UiErrorFrame : Singleton<UiErrorFrame>
    {
        [SerializeField] private UiMessagesFrameMessage messagePrefab;
        [SerializeField] private Transform messageContainer;

        public void Push(string text)
        {
            if (SettingsManager.Instance.DisableErrorMessages)
            {
                return;
            }

            var message = Instantiate(this.messagePrefab, this.messageContainer);
            message.transform.SetSiblingIndex(0);
            message.Initialize(text);
        }
    }
}