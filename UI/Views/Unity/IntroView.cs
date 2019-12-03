using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class IntroView : View, IIntroView
    {
        public event Payload Continue;

        public string Text
        {
            get => this.text.text;
            set => this.text.text = value;
        }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Interactable continueButton;

        protected override void OnInitialize()
        {
            this.continueButton.PointerUp += OnContinueButtonClicked;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerUp -= OnContinueButtonClicked;
        }

        private void OnContinueButtonClicked()
        {
            Continue?.Invoke();
        }
    }
}