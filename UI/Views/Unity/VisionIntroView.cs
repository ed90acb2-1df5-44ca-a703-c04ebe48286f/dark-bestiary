using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionIntroView : View, IVisionIntroView
    {
        public event Payload Continue;

        [SerializeField] private Interactable continueButton;

        protected override void OnInitialize()
        {
            this.continueButton.PointerClick += OnContinueButtonClicked;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonClicked;
        }

        private void OnContinueButtonClicked()
        {
            Continue?.Invoke();
        }
    }
}