using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TalkEncounterView : View, ITalkEncounterView
    {
        public event Payload Continue;

        [SerializeField] private Interactable continueButton;

        protected override void OnInitialize()
        {
            this.continueButton.PointerUp += OnContinueButtonPointerUp;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerUp -= OnContinueButtonPointerUp;
        }

        private void OnContinueButtonPointerUp()
        {
            Continue?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnContinueButtonPointerUp();
            }
        }
    }
}