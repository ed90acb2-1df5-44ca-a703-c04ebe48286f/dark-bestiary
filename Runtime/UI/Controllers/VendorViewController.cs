using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class VendorViewController : ViewController<IVendorView>
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly List<VendorPanel.Category> m_Categories;
        private readonly InventoryComponent m_Inventory;
        private readonly CurrenciesComponent m_Currencies;
        private readonly List<Item> m_Buyback = new();

        private List<Item> m_Assortment;

        public VendorViewController(IVendorView view, IItemRepository itemRepository, List<VendorPanel.Category> categories) : base(view)
        {
            m_ItemRepository = itemRepository;

            m_Inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
            m_Currencies = Game.Instance.Character.Entity.GetComponent<CurrenciesComponent>();
            m_Assortment = RandomizeVendorAssortment();
            m_Categories = categories;
        }

        protected override void OnInitialize()
        {
            foreach (var currency in m_Currencies.Currencies)
            {
                currency.Changed += OnCurrencyChanged;
            }

            View.SellJunk += OnSellJunk;
            View.SellingItem += OnSellingItem;
            View.BuyingItem += OnBuyingItem;
            View.Construct(Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel(), m_Categories);

            UpdateView();
        }

        protected override void OnTerminate()
        {
            foreach (var currency in m_Currencies.Currencies)
            {
                currency.Changed -= OnCurrencyChanged;
            }
        }

        public void ClearBuyback()
        {
            m_Buyback.Clear();
        }

        public void ChangeAssortment(List<Item> assortment)
        {
            m_Assortment = assortment;

            foreach (var item in m_Assortment)
            {
                item.ChangeOwner(m_Inventory.gameObject);
                item.PriceMultiplier = 3;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            RefreshAssortment();
            BlockExpensiveItems();
        }

        private void BlockExpensiveItems()
        {
            foreach (var item in m_Assortment)
            {
                if (m_Currencies.HasEnough(item.GetPrice()))
                {
                    View.MarkAffordable(item);
                    continue;
                }

                View.MarkExpensive(item);
            }
        }

        private List<Item> RandomizeVendorAssortment()
        {
            var items = m_ItemRepository.FindGambleable()
                .Shuffle()
                .Take(15)
                .OrderByDescending(item => item.Type.Type)
                .ToList();

            foreach (var item in items)
            {
                item.ChangeOwner(m_Inventory.gameObject);
                item.PriceMultiplier = 3;
            }

            return items;
        }

        private void RefreshAssortment()
        {
            View.RefreshAssortment(m_Assortment.Concat(m_Buyback).ToList());
        }

        private void OnBuyingItem(Item item)
        {
            try
            {
                m_Inventory.Buy(item);

                if (m_Assortment.Contains(item))
                {
                    m_Assortment.Remove(item);
                    item.PriceMultiplier = 1;
                }
                else
                {
                    m_Buyback.Remove(item);
                }

                UpdateView();
            }
            catch (InsufficientCurrencyException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnSellJunk()
        {
            foreach (var junk in m_Inventory.Items.Where(item => item.Type?.Type == ItemTypeType.Junk).ToList())
            {
                m_Inventory.Sell(junk);
            }

            UpdateView();
        }

        private void OnSellingItem(Item item)
        {
            m_Inventory.Sell(item);

            if (!item.IsJunk)
            {
                m_Buyback.Add(item);
            }

            UpdateView();
        }

        private void OnCurrencyChanged(Currency currency)
        {
            BlockExpensiveItems();
        }
    }
}