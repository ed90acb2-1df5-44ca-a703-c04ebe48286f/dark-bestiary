using System;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TalkEncounterView : View, ITalkEncounterView
    {
        public event Action Continue;

        [SerializeField] private Interactable m_ContinueButton;

        protected override void OnInitialize()
        {
            m_ContinueButton.PointerClick += OnContinueButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            m_ContinueButton.PointerClick -= OnContinueButtonPointerClick;
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