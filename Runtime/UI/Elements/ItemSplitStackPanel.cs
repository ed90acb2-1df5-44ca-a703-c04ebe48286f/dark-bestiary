using System;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ItemSplitStackPanel : Singleton<ItemSplitStackPanel>
    {
        public event Action<Item, int> SplitStackConfirmed;
        public event Action SplitStackCancelled;

        [SerializeField] private Button m_PlusButton;
        [SerializeField] private Button m_MinusButton;
        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private TMP_InputField m_Input;

        private Item m_Item;

        private void Start()
        {
            m_Input.onValueChanged.AddListener(OnInputValueChanged);
            m_PlusButton.onClick.AddListener(OnPlusButtonClicked);
            m_MinusButton.onClick.AddListener(OnMinusButtonClicked);
            m_ConfirmButton.onClick.AddListener(OnConfirmButtonClicked);
            m_CancelButton.onClick.AddListener(OnCancelButtonClicked);

            Instance.Hide();
        }

        public void Show(Item item)
        {
            m_Item = item;
            m_Input.text = "1";

            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnConfirmButtonClicked()
        {
            SplitStackConfirmed?.Invoke(m_Item, int.Parse(m_Input.text));
            Hide();
        }

        private void OnCancelButtonClicked()
        {
            SplitStackCancelled?.Invoke();
            Hide();
        }

        private void OnPlusButtonClicked()
        {
            m_Input.text = Math.Min(int.Parse(m_Input.text) + 1, m_Item.StackCount).ToString();
        }

        private void OnMinusButtonClicked()
        {
            m_Input.text = Math.Max(int.Parse(m_Input.text) - 1, 1).ToString();
        }

        private void OnInputValueChanged(string value)
        {
            m_Input.text = Math.Min(int.Parse(m_Input.text), m_Item.StackCount).ToString();
        }
    }
}