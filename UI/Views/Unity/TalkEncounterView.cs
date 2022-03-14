using DarkBestiary.Managers;
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
            this.continueButton.PointerClick += OnContinueButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonPointerClick;
        }

        private void OnContinueButtonPointerClick()
        {
            Continue?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyBindings.Get(KeyType.EndTurn)))
            {
                OnContinueButtonPointerClick();
            }
        }
    }
}