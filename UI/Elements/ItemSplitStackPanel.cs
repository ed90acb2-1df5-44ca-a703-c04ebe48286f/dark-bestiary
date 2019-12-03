using System;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemSplitStackPanel : Singleton<ItemSplitStackPanel>
    {
        public event Payload<Item, int> SplitStackConfirmed;
        public event Payload SplitStackCancelled;

        [SerializeField] private Button plusButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_InputField input;

        private Item item;

        private void Start()
        {
            this.input.onValueChanged.AddListener(OnInputValueChanged);
            this.plusButton.onClick.AddListener(OnPlusButtonClicked);
            this.minusButton.onClick.AddListener(OnMinusButtonClicked);
            this.confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            this.cancelButton.onClick.AddListener(OnCancelButtonClicked);

            Instance.Hide();
        }

        public void Show(Item item)
        {
            this.item = item;
            this.input.text = "1";

            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnConfirmButtonClicked()
        {
            SplitStackConfirmed?.Invoke(this.item, int.Parse(this.input.text));
            Hide();
        }

        private void OnCancelButtonClicked()
        {
            SplitStackCancelled?.Invoke();
            Hide();
        }

        private void OnPlusButtonClicked()
        {
            this.input.text = Math.Min(int.Parse(this.input.text) + 1, this.item.StackCount).ToString();
        }

        private void OnMinusButtonClicked()
        {
            this.input.text = Math.Max(int.Parse(this.input.text) - 1, 1).ToString();
        }

        private void OnInputValueChanged(string value)
        {
            this.input.text = Math.Min(int.Parse(this.input.text), this.item.StackCount).ToString();
        }
    }
}