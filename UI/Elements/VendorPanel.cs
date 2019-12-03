using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class VendorPanel : MonoBehaviour, IDropHandler
    {
        public enum Category
        {
            Weapon,
            Armor,
            Miscellaneous,
            Buyout,
        }

        public event Payload<Item> ItemRightClicked;
        public event Payload<Item> ItemClicked;
        public event Payload<Item> ItemDroppedIn;

        private readonly List<ItemListRow> itemViews = new List<ItemListRow>();

        [SerializeField] private ItemListRow itemRowPrefab;
        [SerializeField] private Transform rowsContainer;
        [SerializeField] private VendorCategoryTab tabPrefab;
        [SerializeField] private Transform tabContainer;

        private VendorCategoryTab activeTab;

        public void Construct()
        {
            var categories = new List<Category>
            {
                Category.Armor,
                Category.Weapon,
                Category.Miscellaneous,
                Category.Buyout
            };

            foreach (var category in categories)
            {
                var tab = Instantiate(this.tabPrefab, this.tabContainer);
                tab.Clicked += OnTabClicked;
                tab.Construct(category);
            }

            OnTabClicked(this.tabContainer.GetComponentsInChildren<VendorCategoryTab>().First());
        }

        public void MarkExpensive(Item item)
        {
            this.itemViews.FirstOrDefault(row => row.Item.Equals(item))?.MarkDisabled();
        }

        public void MarkAffordable(Item item)
        {
            this.itemViews.FirstOrDefault(row => row.Item.Equals(item))?.MarkEnabled();
        }

        public void RefreshAssortment(List<Item> assortment)
        {
            foreach (var item in assortment)
            {
                var itemView = this.itemViews.FirstOrDefault(view => view.Item == item);

                if (itemView == null)
                {
                    CreateItemView(item);
                }
            }

            foreach (var itemView in this.itemViews.ToList())
            {
                if (assortment.Contains(itemView.Item))
                {
                    continue;
                }

                DestroyItemView(itemView);
            }

            OnTabClicked(this.activeTab);
        }

        private void CreateItemView(Item item)
        {
            var view = Instantiate(this.itemRowPrefab, this.rowsContainer);
            view.RightClicked += OnRowRightClicked;
            view.Clicked += OnRowClicked;
            view.Construct(item);
            this.itemViews.Add(view);
        }

        private void DestroyItemView(ItemListRow itemView)
        {
            itemView.RightClicked -= OnRowRightClicked;
            itemView.Clicked -= OnRowClicked;
            Destroy(itemView.gameObject);
            this.itemViews.Remove(itemView);
        }

        private void OnTabClicked(VendorCategoryTab tab)
        {
            if (this.activeTab != null)
            {
                this.activeTab.Deselect();
            }

            this.activeTab = tab;
            this.activeTab.Select();

            FilterStuff(this.activeTab.Category);
        }

        private void FilterStuff(Category category)
        {
            foreach (var itemView in this.itemViews)
            {
                var isWeapon = itemView.Item.IsWeapon;
                var isBuyout = itemView.Item.IsBuyout;
                var isArmor = itemView.Item.IsArmor;

                switch (category)
                {
                    case Category.Weapon:
                        itemView.gameObject.SetActive(isWeapon);
                        break;
                    case Category.Armor:
                        itemView.gameObject.SetActive(isArmor);
                        break;
                    case Category.Buyout:
                        itemView.gameObject.SetActive(isBuyout);
                        break;
                    case Category.Miscellaneous:
                        itemView.gameObject.SetActive(!isWeapon && !isArmor && !isBuyout);
                        break;
                    default:
                        itemView.gameObject.SetActive(true);
                        break;
                }
            }
        }

        public void OnDrop(PointerEventData pointer)
        {
            var inventoryItem = pointer.pointerDrag.GetComponent<InventoryItem>();

            if (inventoryItem == null)
            {
                return;
            }

            ItemDroppedIn?.Invoke(inventoryItem.Item);
        }

        private void OnRowRightClicked(ItemListRow row)
        {
            ItemRightClicked?.Invoke(row.Item);
        }

        private void OnRowClicked(ItemListRow row)
        {
            ItemClicked?.Invoke(row.Item);
        }
    }
}