using System;
using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TowerVendorView : View, ITowerVendorView
    {
        public event Action<Item> ItemSelected;
        public event Action<Item> ItemDeselected;
        public event Action ContinueButtonClicked;

        [SerializeField] private TowerVendorItemView m_ItemPrefab;
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private ItemTooltip m_Tooltip;
        [SerializeField] private Interactable m_ContinueButton;

        private TowerVendorItemView m_SelectedItem;

        public void Construct(List<Item> items)
        {
            m_Tooltip.DisplayPrice = false;
            m_Tooltip.Initialize();

            foreach (var itemInfo in items)
            {
                var itemView = Instantiate(m_ItemPrefab, m_ItemContainer);
                itemView.Clicked += OnItemViewClicked;
                itemView.Construct(itemInfo);
            }

            MaybeClickOnFirstItem();
        }

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
            ContinueButtonClicked?.Invoke();
        }

        private void OnItemViewClicked(TowerVendorItemView itemView)
        {
            MaybeSelectItem(itemView);
        }

        private void MaybeClickOnFirstItem()
        {
            var firstItemView = m_ItemContainer.GetComponentInChildren<TowerVendorItemView>();

            if (firstItemView == null)
            {
                return;
            }

            OnItemViewClicked(firstItemView);
        }

        private void MaybeSelectItem(TowerVendorItemView itemView)
        {
            if (m_SelectedItem != null)
            {
                m_SelectedItem.Deselect();
                ItemDeselected?.Invoke(m_SelectedItem.Item);
            }

            if (m_SelectedItem == itemView)
            {
                m_SelectedItem = null;
                m_Tooltip.Hide();
                return;
            }

            m_SelectedItem = itemView;
            m_SelectedItem.Select();

            m_Tooltip.Show(m_SelectedItem.Item);

            ItemSelected?.Invoke(itemView.Item);
        }
    }
}