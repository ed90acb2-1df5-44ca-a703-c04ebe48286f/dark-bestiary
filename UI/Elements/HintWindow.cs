using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class HintWindow : MonoBehaviour
    {
        [SerializeField] private Interactable closeButton;

        private void Start()
        {
            this.closeButton.PointerClick += OnCloseButtonClicked;
        }

        private void OnCloseButtonClicked()
        {
            this.closeButton.PointerClick -= OnCloseButtonClicked;
            Destroy(gameObject);
        }
    }
}