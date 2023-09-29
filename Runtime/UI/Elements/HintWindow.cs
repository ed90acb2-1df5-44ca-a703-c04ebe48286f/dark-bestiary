using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class HintWindow : MonoBehaviour
    {
        [SerializeField] private Interactable m_CloseButton;

        private void Start()
        {
            m_CloseButton.PointerClick += OnCloseButtonClicked;
        }

        private void OnCloseButtonClicked()
        {
            m_CloseButton.PointerClick -= OnCloseButtonClicked;
            Destroy(gameObject);
        }
    }
}