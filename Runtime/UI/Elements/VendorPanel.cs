using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class VendorPanel : MonoBehaviour, IDropHandler
    {
        public enum Category
        {
            All,
            Weapon,
            Armor,
            Miscellaneous,
            Buyout,
        }

        public event Action<Item> ItemRightClicked;
        public event Action<Item> ItemClicked;
        public event Action<Item> ItemDroppedIn;

        private readonly List<ItemListRow> m_ItemViews = new();

        [SerializeField] private RectTransform m_Content;
        [SerializeField] private ItemListRow m_ItemRowPrefab;
        [SerializeField] private Transform m_RowsContainer;
        [SerializeField] private VendorCategoryTab m_TabPrefab;
        [SerializeField] private Transform m_TabContainer;

        private VendorCategoryTab m_ActiveTab;

        public void Construct(List<Category> categories)
        {
            foreach (var category in categories)
            {
                var tab = Instantiate(m_TabPrefab, m_TabContainer);
                tab.Clicked += OnTabClicked;
                tab.Construct(category);
            }

            if (categories.Count < 2)
            {
                m_TabContainer.parent.gameObject.SetActive(false);
                m_Content.offsetMax = Vector2.zero;
                m_Content.offsetMin = new Vector2(0, 50);
                return;
            }

            OnTabClicked(m_TabContainer.GetComponentsInChildren<VendorCategoryTab>().First());
        }

        public void MarkExpensive(Item item)
        {
            m_ItemViews.FirstOrDefault(row => row.Item.Equals(item))?.MarkDisabled();
        }

        public void MarkAffordable(Item item)
        {
            m_ItemViews.FirstOrDefault(row => row.Item.Equals(item))?.MarkEnabled();
        }

        public void RefreshAssortment(List<Item> assortment)
        {
            foreach (var item in assortment)
            {
                var itemView = m_ItemViews.FirstOrDefault(view => view.Item == item);

                if (itemView == null)
                {
                    CreateItemView(item);
                }
            }

            foreach (var itemView in m_ItemViews.ToList())
            {
                if (assortment.Contains(itemView.Item))
                {
                    continue;
                }

                DestroyItemView(itemView);
            }

            if (m_ActiveTab != null)
            {
                OnTabClicked(m_ActiveTab);
            }
        }

        private void CreateItemView(Item item)
        {
            var view = Instantiate(m_ItemRowPrefab, m_RowsContainer);
            view.RightClicked += OnRowRightClicked;
            view.Clicked += OnRowClicked;
            view.Construct(item);
            m_ItemViews.Add(view);
        }

        private void DestroyItemView(ItemListRow itemView)
        {
            itemView.RightClicked -= OnRowRightClicked;
            itemView.Clicked -= OnRowClicked;
            Destroy(itemView.gameObject);
            m_ItemViews.Remove(itemView);
        }

        private void OnTabClicked(VendorCategoryTab tab)
        {
            if (m_ActiveTab != null)
            {
                m_ActiveTab.Deselect();
            }

            m_ActiveTab = tab;
            m_ActiveTab.Select();

            FilterStuff(m_ActiveTab.Category);
        }

        private void FilterStuff(Category? category)
        {
            foreach (var itemView in m_ItemViews)
            {
                var isWeapon = itemView.Item.IsWeapon;
                var isBuyout = itemView.Item.IsBuyout;
                var isArmor = itemView.Item.IsArmor;

                switch (category)
                {
                    case Category.All:
                        itemView.gameObject.SetActive(true);
                        break;
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