using System;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class Notification : MonoBehaviour
    {
        public event Action Closed;

        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private bool m_DestroyOnClose;

        private void Start()
        {
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
        }

        private void OnCloseButtonPointerClick()
        {
            Closed?.Invoke();

            if (m_DestroyOnClose)
            {
                Destroy(gameObject);
            }
        }
    }
}