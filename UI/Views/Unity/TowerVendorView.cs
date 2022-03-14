using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class TowerVendorView : View, ITowerVendorView
    {
        public event Payload<Item> ItemSelected;
        public event Payload<Item> ItemDeselected;
        public event Payload ContinueButtonClicked;

        [SerializeField] private TowerVendorItemView itemPrefab;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private ItemTooltip tooltip;
        [SerializeField] private Interactable continueButton;

        private TowerVendorItemView selectedItem;

        public void Construct(List<Item> items)
        {
            this.tooltip.DisplayPrice = false;
            this.tooltip.Initialize();

            foreach (var itemInfo in items)
            {
                var itemView = Instantiate(this.itemPrefab, this.itemContainer);
                itemView.Clicked += OnItemViewClicked;
                itemView.Construct(itemInfo);
            }

            MaybeClickOnFirstItem();
        }

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
            ContinueButtonClicked?.Invoke();
        }

        private void OnItemViewClicked(TowerVendorItemView itemView)
        {
            MaybeSelectItem(itemView);
        }

        private void MaybeClickOnFirstItem()
        {
            var firstItemView = this.itemContainer.GetComponentInChildren<TowerVendorItemView>();

            if (firstItemView == null)
            {
                return;
            }

            OnItemViewClicked(firstItemView);
        }

        private void MaybeSelectItem(TowerVendorItemView itemView)
        {
            if (this.selectedItem != null)
            {
                this.selectedItem.Deselect();
                ItemDeselected?.Invoke(this.selectedItem.Item);
            }

            if (this.selectedItem == itemView)
            {
                this.selectedItem = null;
                this.tooltip.Hide();
                return;
            }

            this.selectedItem = itemView;
            this.selectedItem.Select();

            this.tooltip.Show(this.selectedItem.Item);

            ItemSelected?.Invoke(itemView.Item);
        }
    }
}