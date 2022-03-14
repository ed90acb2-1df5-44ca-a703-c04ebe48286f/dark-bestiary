using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TowerConfirmationView : View, ITowerConfirmationView
    {
        public event Payload ContinueButtonClicked;
        public event Payload ReturnToTownButtonClicked;

        [SerializeField] private InventoryItem itemPrefab;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private Interactable continueButton;
        [SerializeField] private Interactable returnToTownButton;

        public void Construct(List<Item> items)
        {
            foreach (var item in items)
            {
                var itemView = Instantiate(this.itemPrefab, this.itemContainer);
                itemView.Change(item);
                itemView.IsDraggable = false;
            }
        }

        protected override void OnInitialize()
        {
            this.continueButton.PointerClick += OnContinueButtonPointerClick;
            this.returnToTownButton.PointerClick += OnReturnToTownButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonPointerClick;
            this.returnToTownButton.PointerClick -= OnReturnToTownButtonPointerClick;
        }

        private void OnContinueButtonPointerClick()
        {
            if (SettingsManager.Instance.Data.DoNotShowTowerConfirmation)
            {
                ContinueButtonClicked?.Invoke();
                return;
            }

            ConfirmationWindowWithCheckbox.Instance.Cancelled += OnConfirmationCancelled;
            ConfirmationWindowWithCheckbox.Instance.Confirmed += OnConfirmationConfirmed;
            ConfirmationWindowWithCheckbox.Instance.Show(
                I18N.Instance.Get("ui_confirm_tower"),
                I18N.Instance.Get("ui_confirm")
            );
        }

        private void OnReturnToTownButtonPointerClick()
        {
            ReturnToTownButtonClicked?.Invoke();
        }

        private void OnConfirmationConfirmed(bool doNotShowAgain)
        {
            ContinueButtonClicked?.Invoke();
            OnConfirmationCancelled(doNotShowAgain);
        }

        private void OnConfirmationCancelled(bool doNotShowAgain)
        {
            SettingsManager.Instance.Data.DoNotShowTowerConfirmation = doNotShowAgain;

            ConfirmationWindowWithCheckbox.Instance.Cancelled -= OnConfirmationCancelled;
            ConfirmationWindowWithCheckbox.Instance.Confirmed -= OnConfirmationConfirmed;
        }
    }
}