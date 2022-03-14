using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class Notification : MonoBehaviour
    {
        public event Payload Closed;

        [SerializeField] private Interactable closeButton;
        [SerializeField] private bool destroyOnClose;

        private void Start()
        {
            this.closeButton.PointerClick += OnCloseButtonPointerClick;
        }

        private void OnCloseButtonPointerClick()
        {
            Closed?.Invoke();

            if (this.destroyOnClose)
            {
                Destroy(gameObject);
            }
        }
    }
}